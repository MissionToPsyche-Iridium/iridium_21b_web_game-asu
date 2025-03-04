using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_spawner : MonoBehaviour
{
    public GameObject Player;
    public GameObject EnemyType1;
    public GameObject EnemyType2;
    public GameObject EnemyType3;
    public GameObject EnemyType4;
    public float levelMaxY; // Edit to change the range in
    public float levelMinY; // which enemies spawn
    public float levelMaxX; // --------
    public float levelMinX; // --------
    private GameObject[] enemies;
    private float randTimer = 0;
    private float randSpawnrate = .5f;
    private float randX;
    private float randY;
    private float randYPos, randXPos;
    private float randYNeg, randXNeg;
    private int numEnemies = 0;
    private int bossEnemies = 0;
    private int waveNumber = 5; // Start from wave 1
    private int maxWaveEnemies = 10;
    private int maxBossWaveEnemies = 1;
    private bool isBossWave = false;

    void Start()
    {
        Debug.Log("Entering Wave: " + waveNumber);
    }

    void Update()
    {
        randomSpawner();
    }

    void randomSpawner()
    {
        // Update the enemies array
        enemies = GameObject.FindGameObjectsWithTag("basic_enemy");

        if (waveNumber % 5 != 0)
        {
            // Regular wave
            if (randTimer < randSpawnrate)
            {
                randTimer += Time.deltaTime;
            }
            else if (numEnemies < maxWaveEnemies)
            {
                spawnEnemy();
                randTimer = 0;
            }

            // Move on to the next wave
            if (numEnemies == maxWaveEnemies && enemies.Length == 0)
            {
                numEnemies = 0;
                maxWaveEnemies += 5;
                waveNumber++;
                Debug.Log("Entering Wave: " + waveNumber);
            }
        }
        else
        {
            // Boss wave
            if (!isBossWave)
            {
                spawnBoss();
                isBossWave = true;
            }

            // Move on to the next wave
            if (bossEnemies == maxBossWaveEnemies && enemies.Length == 0)
            {
                bossEnemies = 0;
                maxBossWaveEnemies += 1;
                waveNumber++;
                isBossWave = false;
                Debug.Log("Entering Wave: " + waveNumber);
            }
        }
    }

    void spawnEnemy()
    {
        getRandCoord();

        float randomEnemy = UnityEngine.Random.Range(0, 3);
        switch (randomEnemy)
        {
            case 0:
                Instantiate(EnemyType1, new Vector3(randX, randY, 1), transform.rotation);
                break;
            case 1:
                Instantiate(EnemyType2, new Vector3(randX, randY, 1), transform.rotation);
                break;
            case 2:
                Instantiate(EnemyType3, new Vector3(randX, randY, 1), transform.rotation);
                break;
            default:
                break;
        }
        numEnemies++;
    }

    void spawnBoss()
    {
        getRandCoord();
        Instantiate(EnemyType4, new Vector3(randX, randY, 1), transform.rotation);
        bossEnemies++;
    }

    void getRandCoord()
    {
        randXNeg = (float)UnityEngine.Random.Range(levelMinX, Player.transform.position.x);
        randYNeg = (float)UnityEngine.Random.Range(levelMinY, Player.transform.position.y);
        randXPos = (float)UnityEngine.Random.Range(Player.transform.position.x, levelMaxX);
        randYPos = (float)UnityEngine.Random.Range(Player.transform.position.y, levelMaxY);
        randX = UnityEngine.Random.Range(0, 2) == 1 ? randXNeg : randXPos;
        randY = UnityEngine.Random.Range(0, 2) == 1 ? randYNeg : randYPos;
    }
}