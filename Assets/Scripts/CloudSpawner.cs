using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    public GameObject[] clouds;
    public Transform mustang;
    public float distance, spawnRange, minSpawnHeight, minSpawnTime, maxSpawnTime;
    public int maxClouds;

    private float timeUntilNextSpawn;

	// Use this for initialization
	void Start ()
    {
        timeUntilNextSpawn = Random.Range(minSpawnTime, maxSpawnTime);
	}
	
	// Update is called once per frame
	void Update ()
    {
        int cloudCount = transform.childCount;

        if(cloudCount < maxClouds && timeUntilNextSpawn <= 0)
        {
            Vector3 forward = mustang.GetComponent<Rigidbody>().velocity.normalized;
            Vector3 right = mustang.right;
            Vector3 up = Vector3.Cross(forward, right).normalized;
            Vector3 spawnPosition = mustang.position + distance * forward + Random.Range(-spawnRange, spawnRange) * up + Random.Range(-spawnRange, spawnRange) * right;
            if(spawnPosition.y >= minSpawnHeight)
            {
                int cloudIndex = Random.Range(0, clouds.Length);
                GameObject cloud = Instantiate(clouds[cloudIndex], spawnPosition, Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
                Cloud c = cloud.AddComponent<Cloud>();
                c.maxDistanceFromCenter = distance + spawnRange;
                c.center = mustang;
                float size = clouds[cloudIndex].transform.localScale.x;
                c.transform.localScale = Random.Range(0.5f * size, 1 * size) * Vector3.one;
            }
            timeUntilNextSpawn = Random.Range(minSpawnTime, maxSpawnTime);
        }

        if (timeUntilNextSpawn > 0)
            timeUntilNextSpawn -= Time.deltaTime;
	}
}
