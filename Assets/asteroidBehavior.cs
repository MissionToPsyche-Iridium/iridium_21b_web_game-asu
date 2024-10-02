using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class asteroidBehavior : MonoBehaviour
{
    public float startingYPos;
    public float dropSpeed = 5.5f;
    public float destroyHeight;
    public GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("PlayerTag");
        destroyHeight = Player.transform.position.y;
    }


    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3.down) * dropSpeed * Time.deltaTime;
        if (transform.position.y <= destroyHeight)
        {
            Destroy(gameObject);
        }
    }
}
