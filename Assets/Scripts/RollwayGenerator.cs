using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollwayGenerator : MonoBehaviour
{
    public MapMagic.MapMagic mapMagic;
    private int size;
    float[,] map;

    private int oldChildCount = 0;
    private Dictionary<string, Terrain> chunks = new Dictionary<string, Terrain>();

    private void Start()
    {
        size = mapMagic.resolution;
    }

    private void Update()
    {
        if(transform.childCount > oldChildCount)
        {
            foreach(Terrain t in transform.GetComponentsInChildren<Terrain>(true))
            {
                if (!chunks.ContainsKey(t.name))
                {
                    chunks.Add(t.name, t);
                    Debug.Log("Rollwaying");
                    Rollway r = t.gameObject.AddComponent<Rollway>();
                }
            }
        }
    }
}