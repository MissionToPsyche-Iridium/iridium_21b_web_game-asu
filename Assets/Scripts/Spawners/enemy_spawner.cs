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
    public float levelMaxY;//Edit to change the range in
    public float levelMinY;//which enemies spawn
    public float levelMaxX;//--------
    public float levelMinX;//--------
    private GameObject[] enemies;
    private float randTimer = 0;
    private float randSpawnrate = .7f;
    private float randX;
    private float randY;
    private float randYPos, randXPos;
    private float randYNeg, randXNeg;
    private int numEnemies = 0;
    private int waveNumber = 1;
    private int maxWaveEnemies = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("basic_enemy");
        randomSpawner();
    }

    void randomSpawner()
    {
        if (randTimer < randSpawnrate)
        {
            randTimer += Time.deltaTime;
        }
        else if (numEnemies < maxWaveEnemies)
        {
            spawnEnemy();
            randTimer = 0;
        }

        //Move on to next wave
        if (numEnemies == maxWaveEnemies && enemies.Length == 0)
        {
            numEnemies = 0;
            maxWaveEnemies += 5;
            waveNumber++;
            Debug.Log("Entering Wave: " + waveNumber);
        }
    }

    void spawnEnemy()
    {
        getRandCoord();
        float randomEnemy = UnityEngine.Random.Range(0, 3);
        switch(randomEnemy)
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
