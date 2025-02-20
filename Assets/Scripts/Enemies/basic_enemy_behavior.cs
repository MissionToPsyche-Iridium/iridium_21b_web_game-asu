using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class basic_enemy_behavior : MonoBehaviour
{
    public GameObject Player;
    public bool hasTarget = false;
    public float[][] nodeList = new float[][]{ new float[]{ 6.0f, 6.0f }, new float[]{-8.0f, 9.0f }, new float[]{ 7.0f, 5.0f } };
    private float lastSeenX;
    private float lastSeenY;
    private float speed = 2.0f;
    private GameObject[] enemyObjects;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("PlayerTag");
    }

    void Update()
    {
        float step = speed * Time.deltaTime;

        // Move towards the player if there is line of sight
        if (hasLineOfSight())
        {
            hasTarget = true;
            speed = 2.0f;
            transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, step);
        }
        // Once line of sight breaks, move towards where the player was last seen
        else
        {
            hasTarget = false;
            speed = 2.5f;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(lastSeenX, lastSeenY, 0), step);
        }

        // If not moving, find the nearest enemy that has a target
        if (!isMoving(transform.position.x, transform.position.y))
        {
            Debug.Log("NOOOOOOOOOOOOOOOT MOOOOOOVING");
        }
    }

    bool hasLineOfSight()
    {
        bool ret = false;

        // Perform the Linecast and get all hits
        RaycastHit2D[] hits = Physics2D.LinecastAll(transform.position, Player.transform.position);

        foreach (RaycastHit2D hit in hits)
        {
            // Skip the enemy's own collider
            if (hit.collider.gameObject == this.gameObject)
            {
                continue;
            }

            // Check if the hit collider is the player
            if (hit.collider.gameObject.CompareTag("PlayerTag"))
            {
                Debug.DrawLine(transform.position, Player.transform.position, Color.red);
                lastSeenX = Player.transform.position.x;
                lastSeenY = Player.transform.position.y;
                ret = true;
                break;
            }
            else
            {
                // Line of sight is blocked by another object
                Debug.DrawLine(transform.position, Player.transform.position, Color.blue);
                break;
            }
        }

        return ret;
    }

    int[] generateNodeSequence()
    {
        Dictionary<float, int> index_distance_pair = new Dictionary<float, int>();
        float[] distance = new float[nodeList.Length];
        int[] indexes = new int[nodeList.Length];

        for (int i = 0; i < nodeList.Length; i++)
        {
            distance[i] = Vector3.Distance(transform.position, new Vector3(nodeList[i] [0], nodeList[i][1]));
            index_distance_pair.Add(distance[i], i);
        }
        Array.Sort(distance);

        for (int i = 0; i < distance.Length; i++)
        {
            indexes[i] = index_distance_pair[distance[i]];
        }

        return indexes;
    }

    bool isMoving(float x, float y)
    {
        if (Mathf.Approximately(x, lastSeenX) && Mathf.Approximately(y, lastSeenY))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}