using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class basic_enemy_behavior : MonoBehaviour
{
    public GameObject Player;
    public GameObject nodeMap;
    public Node[] nodeList = new Node[6];
    private Node currentNode;
    private GameObject[] enemyObjects;
    private float lastSeenX;
    private float lastSeenY;
    public float speed = 3.0f;
    public static float speedFactor = 1.0f;
    private bool hasTarget = false;
    private bool patrolling = false;
    private bool takingDamage = false;
    private Rigidbody2D rb;
    private Vector2 lastVelocity;
    private EnemyHealth healthScript;
    public Animator animator;
    private Vector3 lastPosition;

    void Start()
    {
        lastSeenX = transform.position.x;
        lastSeenY = transform.position.y;
        speed = speed * speedFactor;
        Player = GameObject.FindGameObjectWithTag("PlayerTag");
        nodeMap = GameObject.FindGameObjectWithTag("node_map");
        rb = GetComponent<Rigidbody2D>();
        lastVelocity = rb.velocity;
        healthScript = GetComponent<EnemyHealth>();
        establishNodes();
    }

    void Update()
    {
        Vector2 velocityChange = rb.velocity - lastVelocity;

        //For pushback powerup
        if (velocityChange.magnitude > 0.01f) // If there is a change in velocity
        {
            if (!takingDamage)
            {
                StartCoroutine(DamageEnemy(healthScript));
                takingDamage = true;
            }
        }

        float step = speed * Time.deltaTime;

        // Move towards the player if there is line of sight
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

        if (patrolling)
        {
            patrol(step);
        }
        Vector3 move = (transform.position - lastPosition).normalized;

        // Pass the movement to directionAnim() to update animation
        directionAnim(move);

        // Update last position for the next frame
        lastPosition = transform.position;

        
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

    void moveTowardsPlayer(float step)
    {
        if(!animator.GetBool("death")) {
            speed = 3.0f * speedFactor;
            hasTarget = true;
            patrolling = false;
            transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, step);
        }
        
    }

    void moveTowardsLast(float step)
    {
        if (!animator.GetBool("death")) {
            speed = 4.0f * speedFactor;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(lastSeenX, lastSeenY, 0), step);
        }
    }

    void moveToRoute(float step)
    {
        if (!animator.GetBool("death")) {
            Node start = findNearestSeenNode();
            transform.position = Vector3.MoveTowards(transform.position, start.node_obj.position, step);
            if (transform.position.x < start.node_obj.position.x + 2 &&
                transform.position.x > start.node_obj.position.x - 2 &&
                transform.position.y < start.node_obj.position.y + 2 &&
                transform.position.y > start.node_obj.position.y - 2) {
                currentNode = start;
                patrolling = true;
            }
        }
    }

    void patrol(float step)
    {
        if (!animator.GetBool("death")) {
            speed = 7.0f * speedFactor;
            transform.position = Vector3.MoveTowards(transform.position, currentNode.node_obj.position, step);
            if (transform.position.x < currentNode.node_obj.position.x + 2 &&
                transform.position.x > currentNode.node_obj.position.x - 2 &&
                transform.position.y < currentNode.node_obj.position.y + 2 &&
                transform.position.y > currentNode.node_obj.position.y - 2) {
                currentNode = currentNode.next;
            }
        }
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
            }
            else
            {
                nodeList[j].next = nodeList[j + 1];
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

    IEnumerator DamageEnemy(EnemyHealth script)
    {
        float elapsed = 0f;
        float value = script.currentHealth;
        float startValue = value;
        while (elapsed < 20.0f)
        {

            if (script != null)
            {
                script.TakeDamage(15f);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        value = 0f; // Ensure it reaches exactly 0
        
        
    }

    void directionAnim(Vector3 move) {
        if (move.sqrMagnitude > 0) {
            animator.SetFloat("x", move.x);
            animator.SetFloat("y", move.y);
        }
    }

}