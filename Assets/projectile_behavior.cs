using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile_behavior : MonoBehaviour
{
    public GameObject Player;
    private Vector3 dest;
    private float speed = 9.0f;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("PlayerTag");
        dest = Player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float step = speed * Time.deltaTime;
        if (transform.position == dest)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, dest, step);
        }
    }
}
