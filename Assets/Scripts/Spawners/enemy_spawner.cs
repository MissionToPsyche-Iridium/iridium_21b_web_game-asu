using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_spawner : MonoBehaviour
{
    public GameObject Player;
    public GameObject Enemy;
    public float levelMaxY;
    public float levelMinY;
    public float levelMaxX;
    public float levelMinX;
    private float randTimer = 0;
    private float randSpawnrate = .7f;
    private float randX;
    private float randY;
    private float randYPos, randXPos;
    private float randYNeg, randXNeg;
    private int numEnemies = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        randomSpawner();
    }

    void randomSpawner()
    {
        if (randTimer < randSpawnrate)
        {
            randTimer += Time.deltaTime;
        }
        else if (numEnemies < 20)
        {
            randXNeg = (float)UnityEngine.Random.Range(levelMinX, Player.transform.position.x);
            randYNeg = (float)UnityEngine.Random.Range(levelMinY, Player.transform.position.y);
            randXPos = (float)UnityEngine.Random.Range(Player.transform.position.x, levelMaxX);
            randYPos = (float)UnityEngine.Random.Range(Player.transform.position.y, levelMaxY);
            randX = UnityEngine.Random.Range(0,2) == 1 ? randXNeg : randXPos;
            randY = UnityEngine.Random.Range(0,2) == 1 ? randYNeg : randYPos;

            Instantiate(Enemy, new Vector3(randX, randY, 1), transform.rotation);
            numEnemies++;
            randTimer = 0;
        }

    }
}
