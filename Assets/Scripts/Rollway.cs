using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class Rollway : MonoBehaviour {

    [Header("Position Info")]
    [Range(0, 1)]
    public float xPos = 0.5f;
    [Range(0, 1)]
    public float zPos = 0.5f;
    [Range(0, 1)]
    public float xLength = 0.2f;
    [Range(0, 1)]
    public float zLength = 0.9f;
    public int smoothIntensity = 35;

    [Header("Prefab")]
    public GameObject rollwayPrefab;

    int size;

    private TerrainData t;
    private float height = 0.25f;
    private bool rotateAround90;

    Queue<RollwayThreadInfo> rollwayQueue = new Queue<RollwayThreadInfo>();
    
    public void Start()
    {
        rotateAround90 = UnityEngine.Random.Range(0, 100) < 50;
        if (gameObject.name != "Map Magic" && (UnityEngine.Random.Range(0,50) < 5 || gameObject.name == "Terrain 0,0"))
        {
            t = gameObject.GetComponent<Terrain>().terrainData;
            size = t.heightmapResolution;
            float[,] map = t.GetHeights(0, 0, size, size);

            RequestRollwayData(map, OnRollwayDataReceived, gameObject.name != "Terrain 0,0" && rotateAround90);
        }
    }

    private void Update()
    {
        if(rollwayQueue.Count > 0)
        {
            for (int i = 0; i < rollwayQueue.Count; i++)
            {
                RollwayThreadInfo threadInfo = rollwayQueue.Dequeue();
                threadInfo.callback(threadInfo.map);
            }
        }
    }

    void OnRollwayDataReceived(float[,] map)
    {
        t.SetHeights(0, 0, map);
        if (gameObject.name != "Terrain 0,0")
        {
            GameObject g = Instantiate(rollwayPrefab);
            float terrainSize = transform.parent.GetComponent<MapMagic.MapMagic>().terrainSize / 2;
            float terrainHeight = transform.parent.GetComponent<MapMagic.MapMagic>().terrainHeight;
            g.transform.position = new Vector3(transform.position.x + terrainSize, map[size / 2, size / 2] * terrainHeight + 1f, transform.position.z + terrainSize);
            g.transform.parent = transform;
            if (rotateAround90)
                g.transform.Rotate(0, 90, 0);
        }
    }

    void RequestRollwayData(float[,] map, Action<float[,]> callback, bool isRotated)
    {
        ThreadStart threadStart = delegate {
            RollwayDataThread(map, callback, isRotated);
        };
        new Thread(threadStart).Start();
    }

    void RollwayDataThread(float[,] map, Action<float[,]> callback, bool isRotated)
    {
        GenerateRollway(map, isRotated);
        lock(rollwayQueue) {
            rollwayQueue.Enqueue(new RollwayThreadInfo(callback, map));
        }
    }

    float[,] GenerateRollway(float[,] map, bool isRotated)
    {
        Vector2 mid = new Vector2(zPos * size, xPos * size);

        if (isRotated)
        {
            float tmp = xLength;
            xLength = zLength;
            zLength = tmp;
        }

        float xMax = mid.x + zLength * size / 2f;
        float xMin = mid.x - zLength * size / 2f;
        float yMax = mid.y + xLength * size / 2f;
        float yMin = mid.y - xLength * size / 2f;

        int c = 0;
        float allHeight = 0;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if ((x > xMin && x < xMax && y > yMin && y < yMax))
                {
                    c++;
                    allHeight += map[x, y];
                }
            }
        }

        height = allHeight / c;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if ((x > xMin && x < xMax && y > yMin && y < yMax))
                {
                    map[x, y] = height;
                }
            }
        }

        return SmoothTerrain(map, smoothIntensity, 0);
    }

    float[,] SmoothTerrain(float[,] map, int times, int iteration)
    {
        print(iteration);
        if (times == iteration)
        {
            return map;
        }

        Vector2 mid = new Vector2(zPos * size, xPos * size);

        int xMax = Mathf.FloorToInt(mid.x + zLength * size / 2f);
        int xMin = Mathf.CeilToInt(mid.x - zLength * size / 2f);
        int yMax = Mathf.FloorToInt(mid.y + xLength * size / 2f);
        int yMin = Mathf.CeilToInt(mid.y - xLength * size / 2f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (((Mathf.Abs(x - xMin) < iteration || Mathf.Abs(x - xMax) < iteration) && (y < yMax + iteration) && y > yMin - iteration) ||
                    ((Mathf.Abs(y - yMin) < iteration || Mathf.Abs(y - yMax) < iteration) && (x < xMax + iteration) && x > xMin - iteration))
                {
                    if (!(x < 1 || x >= size - 1 || y < 1 || y >= size - 1))
                        map[x, y] = SmoothAround(x, y, map);
                }
            }
        }

        return SmoothTerrain(map, times, iteration + 1);
    }

    float SmoothAround(int x, int y, float[,] map)
    {
        float s = 0;
        int width = 1;
        int c = 0;

        for (int dy = -width; dy <= width; dy++)
        {
            for (int dx = -width; dx <= width; dx++)
            {
                int xx = x + dx;
                int yy = y + dy;

                if (!(x < 1 || x >= size - 1 || y < 1 || y >= size - 1))
                {
                    s += map[x + dx, y + dy];
                    c++;
                }
            }
        }

        return s /= c;
    }

    struct RollwayThreadInfo
    {
        public readonly Action<float[,]> callback;
        public readonly float[,] map;

        public RollwayThreadInfo(Action<float[,]> callback, float[,] map)
        {
            this.callback = callback;
            this.map = map;
        }
    }
}
