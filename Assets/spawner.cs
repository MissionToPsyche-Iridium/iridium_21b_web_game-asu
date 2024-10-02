using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    public GameObject Player;
    public GameObject asteroid;
    public GameObject areaOfEffect;
    public float height = 20;
    public float timer = 0;
    public float spawnrate = 1;
    // Start is called before the first frame update
    void Start()
    {
        areaOfEffect = Instantiate(areaOfEffect, Player.transform.position, transform.rotation);
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
            areaOfEffect.transform.position = Player.transform.position;
            timer = 0;
        }
    }
}
