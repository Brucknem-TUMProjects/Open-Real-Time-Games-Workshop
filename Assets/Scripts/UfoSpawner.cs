using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UfoSpawner : MonoBehaviour
{
    public GameObject ufoPrefab;
    public GameObject bossPrefab;
    public GameObject spawnEffect;
    public Transform mustang;
    public int enemiesToSpawn;
    public float difficultyIncreasement;
    public int maxSimultaneousEnemyCount = 1;
    public float randomRadius, spawnDistance, spawnHeight, minSpawnTime, maxSpawnTime;

    private float timeUntilNextSpawn;
    private float difficulty;
    private float simultaneousEnemyCount;
    private bool bossSpawned = false;

    // Use this for initialization
    void Start()
    {
        timeUntilNextSpawn = Random.Range(minSpawnTime, maxSpawnTime);
        difficulty = simultaneousEnemyCount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemiesToSpawn > 0)
        {
            if (transform.childCount < Mathf.Min((int)simultaneousEnemyCount, maxSimultaneousEnemyCount))
            {
                if (timeUntilNextSpawn <= 0)
                {
                    SpawnEnemy();
                    timeUntilNextSpawn = Random.Range(minSpawnTime, maxSpawnTime);
                }
                else
                {
                    timeUntilNextSpawn -= Time.deltaTime;
                }
            }
        }
        else if (!bossSpawned && transform.childCount == 0)
        {
            bossSpawned = true;
            SpawnEndBoss();
        }
    }

    void SpawnEnemy()
    {
        enemiesToSpawn--;
        difficulty += difficultyIncreasement;
        simultaneousEnemyCount += 1.5f * difficultyIncreasement;
        Vector3 random = new Vector3(Random.Range(-randomRadius, randomRadius), Random.Range(-randomRadius, randomRadius), Random.Range(-randomRadius, randomRadius));
        Vector3 spawnPos = mustang.position + spawnDistance * mustang.forward + spawnHeight * mustang.up + random;
        GameObject ufo = Instantiate(ufoPrefab, spawnPos, Quaternion.identity, transform);
        Destroy(Instantiate(spawnEffect, ufo.transform, false), 5.0f);
        FlightAI flightAI = ufo.GetComponent<FlightAI>();
        UfoAI ufoAI = ufo.GetComponent<UfoAI>();
        // Adjust flight AI properties
        flightAI.follow = mustang;
        flightAI.movementSpeed *= difficulty;
        flightAI.basicTurnSpeed *= difficulty;
        // Adjust ufo AI properties
        ufoAI.player = mustang.GetComponent<Player>();
        ufoAI.target = mustang.GetComponentInChildren<Camera>().transform.parent;
        ufoAI.health *= difficulty;
        //ufoAI.lightDamage *= difficulty;
        ufoAI.lightAimSpeed *= difficulty;
        Debug.Log(ufo.name + " spawned");
    }

    void SpawnEndBoss ()
    {
        GameData.Instance.objective = "Rescue Ray Rice by defeating the boss";
        Vector3 random = new Vector3(Random.Range(-randomRadius, randomRadius), Random.Range(-randomRadius, randomRadius), Random.Range(-randomRadius, randomRadius));
        GameObject boss = Instantiate(bossPrefab, mustang.position + 2 * spawnDistance * mustang.forward + spawnHeight * mustang.up + random, Quaternion.identity, transform);
        FlightAI flightAI = boss.GetComponent<FlightAI>();
        UfoAI ufoAI = boss.GetComponent<UfoAI>();
        // Adjust flight AI properties
        flightAI.follow = mustang;
        // Adjust ufo AI properties
        ufoAI.player = mustang.GetComponent<Player>();
        ufoAI.target = mustang.GetComponentInChildren<Camera>().transform.parent;
    }
}
