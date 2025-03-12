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
    public GameObject nodeMap;
    public float levelMaxY; // Edit to change the range in
    public float levelMinY; // which enemies spawn
    public float levelMaxX; // --------
    public float levelMinX; // --------
    public Node[] nodeList = new Node[6];
    private List<int> validSpawns = new List<int>();
    private int currentInvalid = -1;
    private GameObject[] enemies;
    private float randTimer = 0;
    private float randSpawnrate = .5f;
    private float randX;
    private float randY;
    private int numEnemies = 0;
    private int bossEnemies = 0;
    private int waveNumber = 1; // Start from wave 1
    private int maxWaveEnemies = 10;
    private int maxBossWaveEnemies = 1;
    private bool isBossWave = false;

    void Start()
    {
        establishNodes();
        Debug.Log("Entering Wave: " + waveNumber);
    }

    void Update()
    {
        randomSpawner();
        findNearestSeenNode();
    }

    void randomSpawner()
    {
        // Update the enemies array
        enemies = GameObject.FindGameObjectsWithTag("basic_enemy");

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
        /*
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
        */
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
        //getRandCoord();
        Instantiate(EnemyType4, new Vector3(randX, randY, 1), transform.rotation);
        bossEnemies++;
    }

    void getRandCoord()
    {
        /*
        randXNeg = (float)UnityEngine.Random.Range(levelMinX, Player.transform.position.x);
        randYNeg = (float)UnityEngine.Random.Range(levelMinY, Player.transform.position.y);
        randXPos = (float)UnityEngine.Random.Range(Player.transform.position.x, levelMaxX);
        randYPos = (float)UnityEngine.Random.Range(Player.transform.position.y, levelMaxY);
        randX = UnityEngine.Random.Range(0, 2) == 1 ? randXNeg : randXPos;
        randY = UnityEngine.Random.Range(0, 2) == 1 ? randYNeg : randYPos;
        */
        int index = (int)UnityEngine.Random.Range(0, 5);
        Debug.Log(index);
        randX = nodeList[validSpawns[index]].x;
        randY = nodeList[validSpawns[index]].y;
    }

    void establishNodes()
    {
        int i = 0;
        foreach (Transform node in nodeMap.transform)
        {
            nodeList[i] = new Node(node, nodeMap.transform.position.x, nodeMap.transform.position.y);
            i++;
        }

        for (int j = 0; j < 6; j++)
        {
            if ((j + 1) == 6)
            {
                nodeList[j].next = nodeList[0];
            }
            else
            {
                nodeList[j].next = nodeList[j + 1];
            }
        }
        validSpawns.Add(0);
        validSpawns.Add(1);
        validSpawns.Add(2);
        validSpawns.Add(3);
        validSpawns.Add(4);
        validSpawns.Add(5);
    }

    bool hasLineOfSight(Transform target, string tag)
    {
        bool ret = false;

        // Perform the Linecast and get all hits
        RaycastHit2D[] hits = Physics2D.LinecastAll(Player.transform.position, target.position);
        foreach (RaycastHit2D hit in hits)
        {
            // Skip the enemy's own collider
            if (hit.collider.gameObject == Player)
            {
                continue;
            }

            // Check if the hit collider is the player
            if (hit.collider.gameObject.CompareTag(tag))
            {
                Debug.DrawLine(Player.transform.position, target.position, Color.green);
                ret = true;
                break;
            }
            else
            {
                // Line of sight is blocked by another object
                Debug.DrawLine(Player.transform.position, target.position, Color.blue);
                break;
            }
        }

        return ret;
    }

    Node findNearestSeenNode()
    {
        Node node;
        Node ret_node = nodeList[1];
        int ret_index = 0;
        float temp_dist;
        float dist = 1000000f;
        for (int i = 0; i < nodeList.Length; i++)
        {
            node = nodeList[i];
            if (hasLineOfSight(node.node_obj, "node"))
            {
                temp_dist = Vector3.Distance(node.node_obj.position, Player.transform.position);
                if (temp_dist < dist)
                {
                    dist = temp_dist;
                    ret_node = node;
                    ret_index = i;
                }
            }
        }

        //Modifying list to only hold indexes of valid spawns
        if (currentInvalid != -1)
        {
            validSpawns.Add(currentInvalid);
        }
        validSpawns.Remove(ret_index);
        currentInvalid = ret_index;
        Debug.Log(validSpawns[0] + " " + validSpawns[1] + " " + validSpawns[2] + " " + validSpawns[3] + " " + validSpawns[4]);
        return ret_node;
    }
}