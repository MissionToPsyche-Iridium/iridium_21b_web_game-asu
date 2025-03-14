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
    public GameObject pushBackPU;
    public GameObject dashPU;
    public GameObject dodgePU;
    public GameObject nodeMap;
    public Node[] nodeList = new Node[6];
    private List<int> validSpawns = new List<int> { 0, 1, 2, 3, 4, 5};
    private int currentInvalid = -1;
    private GameObject[] enemies;
    private float randTimer = 0;
    private float randSpawnrate = .5f;
    private float randX;
    private float randY;
    private int numEnemies = 0;
    private int bossEnemies = 0;
    private int waveNumber = 2; // Start from wave 1
    private int maxWaveEnemies = 10;
    private int maxBossEnemies = 0;
    private bool waiting = false;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("PlayerTag");
        nodeMap = GameObject.FindGameObjectWithTag("node_map");
        establishNodes();
        Debug.Log("Entering Wave: " + waveNumber);
    }

    void Update()
    {
        if (waiting)
        {
            return;
        }
        else
        {
            randomSpawner();
        }

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
            if (waveNumber % 5 == 0 && bossEnemies < maxBossEnemies)
            {
                spawnBoss();
            }
            randTimer = 0;
        }


        // Move on to the next wave
        Debug.Log(numEnemies + " " + maxWaveEnemies + " " + enemies.Length);
        if (numEnemies == maxWaveEnemies && enemies.Length - bossEnemies == 0)
        {
            numEnemies = 0;
            bossEnemies = 0;
            maxWaveEnemies += 5;
            waveNumber++;
            if (waveNumber % 5 == 0)
            {
                maxBossEnemies += 1;
            }
            checkForCheckpoint();
            Debug.Log("Entering Wave: " + waveNumber);
        }
    }

    void spawnEnemy()
    {
        getRandCoord();
        float randomEnemy = UnityEngine.Random.Range(0, calcEnemyRange());
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
        int index = (int)UnityEngine.Random.Range(0, 5);
        randX = nodeList[validSpawns[index]].x;
        randY = nodeList[validSpawns[index]].y;
    }

    void establishNodes()
    {
        int i = 0;
        foreach (Transform node in nodeMap.transform)
        {
            nodeList[i] = new Node(node, 0, 0);
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
        return ret_node;
    }

    void checkForCheckpoint()
    {
        switch (waveNumber)
        {
            case 3:
                Instantiate(dodgePU, new Vector3(0, 0, 1), transform.rotation);
                waiting = true;
                StartCoroutine(WaitBetweenWaves());
                break;
            case 6:
                Instantiate(dashPU, new Vector3(0, 0, 1), transform.rotation);
                waiting = true;
                StartCoroutine(WaitBetweenWaves());
                break;
            case 9:
                Instantiate(pushBackPU, new Vector3(0, 0, 1), transform.rotation);
                waiting = true;
                StartCoroutine(WaitBetweenWaves());
                break;
        }
    }

    //calculates which enemies to spawn depending on wave number
    int calcEnemyRange()
    {
        int range_max;
        range_max = waveNumber / 5;
        if (range_max > 3)
        {
            range_max = 3;
        }
        return range_max;
    }

    IEnumerator WaitBetweenWaves()
    {
        yield return new WaitForSeconds(5.0f);
        waiting = false;
    }
}
