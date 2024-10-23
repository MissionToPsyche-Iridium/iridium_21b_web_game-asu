using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    public GameObject Player;
    public GameObject asteroid;
    public GameObject areaOfEffect;
    public GameObject areaOfEffectRand;
    public float height = 20;
    private float timer = 0;
    private float randTimer = 0;
    private float spawnrate = 1;
    private float randSpawnrate = .7f;
    private float randX;
    private float randY;
    // Start is called before the first frame update
    void Start()
    { 

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y + height, Player.transform.position.z);
        if (timer < spawnrate )
        {
            timer = timer + Time.deltaTime;
        }
        else
        {
            Instantiate(asteroid, transform.position, transform.rotation);

            //Instantiate(asteroid, new Vector3(randX, randY + height, 1), transform.rotation);
            timer = 0;
            
        }
        randomSpawner();
    }

    void randomSpawner()
    {
        if (randTimer < randSpawnrate)
        {
            randTimer += Time.deltaTime;
        }
        else
        {
            randX = (float)Random.Range(0, 6) + Player.transform.position.x;
            randY = (float)Random.Range(0, 2) + Player.transform.position.y;

            Instantiate(asteroid, new Vector3(randX, randY + height, 1), transform.rotation);
            randTimer = 0;
        }

    }
}
