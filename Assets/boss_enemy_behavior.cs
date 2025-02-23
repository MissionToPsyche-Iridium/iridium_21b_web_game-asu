using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boss_enemy_behavior : MonoBehaviour
{
    public GameObject Player;
    public GameObject Projectile;
    private float speed = 3.0f;
    private float lastSeenX;
    private float lastSeenY;
    private float lastEnemyX;
    private float lastEnemyY;
    public float[][] nodeList = new float[][] { new float[] { 6.0f, -6.0f }, new float[] { -8.0f, 9.0f }, new float[] { 7.0f, 4.0f } };
    private int[] nodeSeq;
    private int currentNode;
    private float time_between_shot = 0.8f;
    private bool hasTarget = false;
    private bool shooting = false;
    // Start is called before the first frame update
    void Start()
    {
        lastSeenX = transform.position.x;
        lastSeenY = transform.position.y;
        Player = GameObject.FindGameObjectWithTag("PlayerTag");
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(transform.position, Player.transform.position);
        float step = speed * Time.deltaTime;
        //If distance from player = 6
        if (hasLineOfSight())
        {
            hasTarget = true;
            speed = 2.0f;
            transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, step);
        }
        // Once line of sight breaks, move towards where the player was last seen
        else if (hasTarget)
        {
            speed = 2.5f;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(lastSeenX, lastSeenY, 0), step);
        }

        // If the enemy reaches the last seen and it still doesnt have line of sight, no longer has target
        if (transform.position.x > lastSeenX - 2 && transform.position.x < lastSeenX + 2 &&
            transform.position.y > lastSeenY - 2 && transform.position.y < lastSeenY + 2 &&
            !hasLineOfSight())
        {
            hasTarget = false;
            nodeSeq = generateNodeSequence();
        }
        // If the enemy doesnt have a target
        if (!hasTarget && nodeSeq != null)
        {
            Debug.Log("Looking for player");
            //if player is at node select next node
            if (transform.position.x > nodeList[nodeSeq[currentNode]][0] - 2 &&
                transform.position.x < nodeList[nodeSeq[currentNode]][0] + 2 &&
                transform.position.y > nodeList[nodeSeq[currentNode]][1] - 2 &&
                transform.position.y < nodeList[nodeSeq[currentNode]][1] + 2)
            {
                currentNode++;
                if (currentNode == 3)
                {
                    currentNode = 0;
                }
            }
            //move towards current node
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(nodeList[nodeSeq[currentNode]][0], nodeList[nodeSeq[currentNode]][1]), step);
        }
    }

    IEnumerator WaitAfterShot()
    {
        yield return new WaitForSeconds(time_between_shot);
        shooting = false;
    }

    int[] generateNodeSequence()
    {
        Dictionary<float, int> index_distance_pair = new Dictionary<float, int>();
        float[] distance = new float[nodeList.Length];
        int[] indexes = new int[nodeList.Length];

        for (int i = 0; i < nodeList.Length; i++)
        {
            distance[i] = Vector3.Distance(transform.position, new Vector3(nodeList[i][0], nodeList[i][1]));
            index_distance_pair.Add(distance[i], i);
        }
        Array.Sort(distance);

        for (int i = 0; i < distance.Length; i++)
        {
            indexes[i] = index_distance_pair[distance[i]];
        }

        return indexes;
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
}
