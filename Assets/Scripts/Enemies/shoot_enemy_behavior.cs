using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shoot_enemy_behavior : MonoBehaviour
{
    public GameObject Player;
    public GameObject Projectile;
    public GameObject nodeMap;
    public Node[] nodeList = new Node[6];
    public float speed = 5.0f;
    private Node currentNode;
    private float lastSeenX;
    private float lastSeenY;
    private int[] nodeSeq;
    private float time_between_shot = 0.8f;
    private bool hasTarget = false;
    private bool patrolling = false;
    private bool shooting = false;
    // Start is called before the first frame update

    void Start()
    {
        lastSeenX = transform.position.x;
        lastSeenY = transform.position.y;
        Player = GameObject.FindGameObjectWithTag("PlayerTag");
        nodeMap = GameObject.FindGameObjectWithTag("node_map");
        establishNodes();
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(transform.position, Player.transform.position);
        float step = speed * Time.deltaTime;
        //If distance from player = 6
        if (dist > 6)
        {
            if (hasLineOfSight(Player.transform, "PlayerTag"))
            {
                moveTowardsPlayer(step);
            }
            // Once line of sight breaks, move towards where the player was last seen
            else if (hasTarget)
            {
                moveTowardsLast(step);
            }

            // If the enemy reaches the last seen and it still doesnt have line of sight, no longer has target
            if (transform.position.x < lastSeenX + 2 &&
                transform.position.x > lastSeenX - 2 &&
                transform.position.y < lastSeenY + 2 && 
                transform.position.y > lastSeenY - 2 &&
                !hasLineOfSight(Player.transform, "PlayerTag"))
            {
                hasTarget = false;
            }
            // If the enemy doesnt have a target
            if (!hasTarget && !patrolling)
            {
                moveToRoute(step);
            }

            //move between nodes
            if (patrolling)
            {
                patrol(step);
            }
        } 
        else if (!shooting)
        {
            //shoot
            shooting = true;
            Instantiate(Projectile, transform.position, Quaternion.identity);
            StartCoroutine(WaitAfterShot());
        }
    }

    void moveTowardsPlayer(float step)
    {
        hasTarget = true;
        patrolling = false;
        speed = 5.0f;
        transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, step);
    }

    void moveTowardsLast(float step)
    {
        speed = 6.0f;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(lastSeenX, lastSeenY, 0), step);
    }

    void moveToRoute(float step)
    {
        Node start = findNearestSeenNode();
        transform.position = Vector3.MoveTowards(transform.position, start.node_obj.position, step);
        if (transform.position.x < start.node_obj.position.x + 2 &&
            transform.position.x > start.node_obj.position.x - 2 &&
            transform.position.y < start.node_obj.position.y + 2 &&
            transform.position.y > start.node_obj.position.y - 2)
        {
            currentNode = start;
            patrolling = true;
        }
    }

    void patrol(float step)
    {
        speed = 7.0f;
        transform.position = Vector3.MoveTowards(transform.position, currentNode.node_obj.position, step);
        if (transform.position.x < currentNode.node_obj.position.x + 2 &&
            transform.position.x > currentNode.node_obj.position.x - 2 &&
            transform.position.y < currentNode.node_obj.position.y + 2 &&
            transform.position.y > currentNode.node_obj.position.y - 2)
        {
            currentNode = currentNode.next;
        }
    }

    IEnumerator WaitAfterShot()
    {
        yield return new WaitForSeconds(time_between_shot);
        shooting = false;
    }

    bool hasLineOfSight(Transform target, string tag)
    {
        bool ret = false;

        // Perform the Linecast and get all hits
        RaycastHit2D[] hits = Physics2D.LinecastAll(transform.position, target.position);
        foreach (RaycastHit2D hit in hits)
        {
            // Skip the enemy's own collider
            if (hit.collider.gameObject == this.gameObject)
            {
                continue;
            }

            // Check if the hit collider is the player
            if (hit.collider.gameObject.CompareTag(tag))
            {
                Debug.DrawLine(transform.position, target.position, Color.red);
                if (tag == "PlayerTag")
                {
                    lastSeenX = target.position.x;
                    lastSeenY = target.position.y;
                }
                ret = true;
                break;
            }
            else
            {
                // Line of sight is blocked by another object
                Debug.DrawLine(transform.position, target.position, Color.blue);
                break;
            }
        }

        return ret;
    }

    void establishNodes()
    {
        int i = 0;
        foreach (Transform node in nodeMap.transform)
        {
            nodeList[i] = new Node(node, nodeMap.transform.position.x, nodeMap.transform.position.y);
            i++;
        }

        for (int j = 0; j < 6; j++)
        {
            if ((j + 1) == 6)
            {
                nodeList[j].next = nodeList[0];
            } else
            {
                nodeList[j].next = nodeList[j+1];
            }
            
        }
    }

    Node findNearestSeenNode()
    {
        Node ret_node = nodeList[1];
        float temp_dist;
        float dist = 1000000f;
        foreach (Node node in nodeList)
        {
            if (hasLineOfSight(node.node_obj, "node"))
            {
                temp_dist = Vector3.Distance(node.node_obj.position, transform.position);
                if (temp_dist < dist)
                {
                    dist = temp_dist;
                    ret_node = node;
                }
            }
        }
        return ret_node;
    }
}
