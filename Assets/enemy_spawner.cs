using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_spawner : MonoBehaviour
{
    public GameObject Player;
    public GameObject Enemy;
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
            randXNeg = (float)Random.Range(-12, -6) + Player.transform.position.x;
            randYNeg = (float)Random.Range(-8, -4) + Player.transform.position.y;
            randXPos = (float)Random.Range(6, 12) + Player.transform.position.x;
            randYPos = (float)Random.Range(4, 8) + Player.transform.position.y;

            randX = Random.Range(0,2) == 1 ? randXNeg : randXPos;
            randY = Random.Range(0,2) == 1 ? randYNeg : randYPos;

            Debug.Log(randX + " " + randY);
            Instantiate(Enemy, new Vector3(randX, randY, 1), transform.rotation);
            numEnemies++;
            randTimer = 0;
        }

    }
}
