using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dash_enemy_script : MonoBehaviour
{
    public GameObject Player;
    public float speed = 2.0f;
    private Vector3 dash_location;
    private float lastSeenX;
    private float lastSeenY;
    public float dashSpeed = 8.0f;
    public float waitTimeAfterDash = 3.0f; // Time to wait after dashing
    public float[][] nodeList = new float[][] { new float[] { 6.0f, -6.0f }, new float[] { -8.0f, 9.0f }, new float[] { 7.0f, 4.0f } };
    private int[] nodeSeq;
    private int currentNode = 0;
    private bool hasTarget = false;
    private bool isDashing = false;
    private bool isWaiting = false;
    
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
        if (isWaiting) return; // Stop movement while waiting

        float step = speed * Time.deltaTime;
        float distance = Vector3.Distance(transform.position, Player.transform.position);

        // Check if enemy should dash
        if (!isDashing && distance <= 4.0f)
        {
            if (hasLineOfSight())
            {
                hasTarget = true;
                dash_location = Player.transform.position; // Capture the player's position for dashing
            }
            else
            {
                dash_location = new Vector3(lastSeenX, lastSeenY, 0);
            }

            isDashing = true;
        }

        // If dashing, move towards the captured location at dash speed
        if (isDashing)
        {
            transform.position = Vector3.MoveTowards(transform.position, dash_location, dashSpeed * Time.deltaTime);
            

            // Stop dashing when reaching the target
            if (Vector3.Distance(transform.position, dash_location) < 0.1f)
            {
                StartCoroutine(WaitAfterDash());
            }
        }
        else
        {
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
            if (transform.position.x == lastSeenX && transform.position.y == lastSeenY && !hasLineOfSight())
            {
                hasTarget = false;
                nodeSeq = generateNodeSequence();
            }
            // If the enemy doesnt have a target
            if (!hasTarget && nodeSeq != null)
            {
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
    }

    IEnumerator WaitAfterDash()
    {
        isDashing = false; // Stop dashing
        isWaiting = true;  // Prevent movement

        yield return new WaitForSeconds(waitTimeAfterDash);

        isWaiting = false; // Resume movement
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
