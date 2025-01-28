using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basic_enemy_behavior : MonoBehaviour
{
    public GameObject Player;
    private float speed = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("PlayerTag");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = Player.transform.position - transform.position;

        float step = speed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, step);
    }
}
