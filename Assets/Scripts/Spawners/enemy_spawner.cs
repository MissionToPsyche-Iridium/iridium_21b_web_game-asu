using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_spawner : MonoBehaviour
{
    //Entities
    public GameObject Player;
    public GameObject EnemyType1;
    public GameObject EnemyType2;
    public GameObject EnemyType3;
    public GameObject EnemyType4;

    //Spawnable Powerups
    public GameObject pushBackPU;
    public GameObject dashPU;
    public GameObject dodgePU;
    public GameObject damagePU;
    public GameObject fireRatePU;
    public GameObject healthPU;

    //Node Maps
    public GameObject nodeMap;
    public GameObject quadrant1;
    public GameObject quadrant2;
    public GameObject quadrant3;
    public GameObject quadrant4;
    public Node[] nodeList = new Node[6]; //will dynamically set to the nearest nodemap / quadrant
    private GameObject nearestMap;

    //Spawn Logic
    private List<int> validSpawns = new List<int> { 0, 1, 2, 3, 4, 5 };
    private int currentInvalid = -1;
    private GameObject[] enemies;
    private float randTimer = 0;
    private float randSpawnrate = .5f;
    private float randX;
    private float randY;

    //Distance
    private float nodeMapDist;
    private float quad1Dist;
    private float quad2Dist;
    private float quad3Dist;
    private float quad4Dist;
    private GameObject[] maps = new GameObject[5];

    //Wave info
    private int numEnemies = 0;
    private int bossEnemies = 0;
    private int waveNumber = 1; // Start from wave 1
    private int maxWaveEnemies = 10;
    private int maxBossEnemies = 0;
    private bool waiting = false;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("PlayerTag");
        nodeMap = GameObject.FindGameObjectWithTag("node_map");
        quadrant1 = GameObject.FindGameObjectWithTag("quad1");
        quadrant2 = GameObject.FindGameObjectWithTag("quad2");
        quadrant3 = GameObject.FindGameObjectWithTag("quad3");
        quadrant4 = GameObject.FindGameObjectWithTag("quad4");
        maps[0] = nodeMap;
        maps[1] = quadrant1;
        maps[2] = quadrant2;
        maps[3] = quadrant3;
        maps[4] = quadrant4;
        establishNodes(nodeMap); //initially setting the spawn points to the central nodemap
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
        /*
         -Find nearest map
         -Set valid spawns to the points on the nearest map
         -Exclude nearest node
        */
        getNearestNodeMap();
        establishNodes(nearestMap);
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
        //Debug.Log(numEnemies + " " + maxWaveEnemies + " " + enemies.Length + " " + bossEnemies);
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

            checkpoint();
            Debug.Log("Entering Wave: " + waveNumber);
        }
    }

    public void StartWave(int waveNum)
    {
        waveNumber = waveNum;
        waiting = false; // Resume spawning
        Debug.Log("Starting Wave: " + waveNumber);
    }



    void spawnEnemy()
    {
        getRandCoord();
        float randomEnemy = UnityEngine.Random.Range(0, calcEnemyRange() + 1);
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

    void establishNodes(GameObject map)
    {
        int i = 0;
        nearestMap = map;
        foreach (Transform node in nearestMap.transform)
        {
            nodeList[i] = new Node(node, 0, 0);
            i++;
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

    void getNearestNodeMap()
    {
        nearestMap = maps[0];
        float currDist;
        float smallestDist = Vector3.Distance(Player.transform.position, nearestMap.transform.position);
        for(int i = 0; i < maps.Length; i++)
        {
            currDist = Vector3.Distance(Player.transform.position, maps[i].transform.position);
            if (smallestDist > currDist)
            {
                nearestMap = maps[i];
                smallestDist = Vector3.Distance(Player.transform.position, nearestMap.transform.position);
            }
        }
        Debug.Log(nearestMap.transform.position.x + " " + nearestMap.transform.position.y);
        
    }

    void checkpoint()
    {
        //replace spawns with the parts of the ship once we get there
        switch (waveNumber)
        {
            case 3:
                Instantiate(dodgePU, new Vector3(0, 0, 1), transform.rotation);
                break;
            case 6:
                Instantiate(dashPU, new Vector3(0, 0, 1), transform.rotation);
                break;
            case 9:
                Instantiate(pushBackPU, new Vector3(0, 0, 1), transform.rotation);
                break;
        }
        //Spawning the powerups
        Instantiate(healthPU, new Vector3(1, -1, 1), transform.rotation);
        Instantiate(damagePU, new Vector3(-1, -1, 1), transform.rotation);
        Instantiate(fireRatePU, new Vector3(0, 1, 1), transform.rotation);
        waiting = true;
        StartCoroutine(WaitBetweenWaves());
    }

    //calculates which enemies to spawn depending on wave number
    int calcEnemyRange()
    {
        int range_max;
        range_max = waveNumber / 3;
        if (range_max > 3)
        {
            range_max = 3;
        }
        return range_max;
    }

    //temporarily reverting until i figure out how to implement timer
    IEnumerator WaitBetweenWaves()
    {
        //waiting = true; // Pause enemy spawning
        //Debug.Log("Wave Complete! Clearing all enemies before next wave...");

        // Destroy all remaining enemies

        yield return new WaitForSeconds(10f); // Increased break time to 10 seconds
        waiting = false;
        //FindObjectOfType<Timer>().TriggerNextWave(); // Start next wave
    }
}
