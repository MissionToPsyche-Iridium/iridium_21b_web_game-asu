using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class head_behavior : MonoBehaviour, IEnemyDeathHandler
{
    public GameObject Player;
    public GameObject nodeMap;
    public GameObject body;
    public Node[] nodeList = new Node[24];
    public int numSegments = 5;
    public static float speedFactor = 1.0f;
    public Vector3 target;
    public Animator animator;
    private string lastFacingDirection = "";
    private List<GameObject> segments = new List<GameObject>();
    private Node currentNode;
    private GameObject[] enemyObjects;
    private float lastSeenX;
    private float lastSeenY;
    private float speed = 4.0f;
    private bool hasTarget = false;
    private bool patrolling = false;
    private bool isSegment;

    public AudioClip deathSound;
    public AudioClip bossHitSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lastSeenX = transform.position.x;
        lastSeenY = transform.position.y;
        speed = speed * speedFactor;
        Player = GameObject.FindGameObjectWithTag("PlayerTag");
        nodeMap = GameObject.FindGameObjectWithTag("node_map");
        establishNodes();
        createSegments();
    }

    void Update()
    {
        Vector3 direction = (target - transform.position).normalized;
        determineAnimationState(direction);
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
        speed = 3.0f * speedFactor;
        hasTarget = true;
        patrolling = false;
        target = Player.transform.position;
        transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, step);
    }

    void moveTowardsLast(float step)
    {
        speed = 4.0f * speedFactor;
        target = new Vector3(lastSeenX, lastSeenY, 0);
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(lastSeenX, lastSeenY, 0), step);
    }

    void moveToRoute(float step)
    {
        Node start = findNearestSeenNode();
        target = start.node_obj.position;
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
        speed = 7.0f * speedFactor;
        target = currentNode.node_obj.position;
        transform.position = Vector3.MoveTowards(transform.position, currentNode.node_obj.position, step);
        if (transform.position.x < currentNode.node_obj.position.x + 2 &&
            transform.position.x > currentNode.node_obj.position.x - 2 &&
            transform.position.y < currentNode.node_obj.position.y + 2 &&
            transform.position.y > currentNode.node_obj.position.y - 2)
        {
            currentNode = currentNode.next;
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

        for (int j = 0; j < nodeList.Length; j++)
        {
            if ((j + 1) == nodeList.Length)
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

    void createSegments()
    {
        for (int i = 0; i < numSegments; i++)
        {
            GameObject newSeg = Instantiate(body, transform.position, Quaternion.identity);
            segments.Add(newSeg);
            body_follow segmentScript = newSeg.GetComponent<body_follow>();
            if (i == 0)
            {
                segmentScript.target = transform;
            }
            else
            {
                segmentScript.target = segments[i - 1].transform;
            }

            segmentScript.delay = segmentScript.delay * i;
        }
    }

    void determineAnimationState(Vector3 direction)
    {

        string newDirection = "";

        if (direction.x >= 0.923f)  // Right
            newDirection = "facingRight";
        else if (direction.x <= -0.923f)  // Left
            newDirection = "facingLeft";
        else if (direction.y >= 0.923f)  // Up
            newDirection = "facingUp";
        else if (direction.y <= -0.923f)  // Down
            newDirection = "facingDown";
        else if (direction.x > 0.382f && direction.y > 0.382f)  // Up-Right
            newDirection = "facingUpRight";
        else if (direction.x < -0.382f && direction.y > 0.382f)  // Up-Left
            newDirection = "facingUpLeft";
        else if (direction.x < -0.382f && direction.y < -0.382f)  // Down-Left
            newDirection = "facingDownLeft";
        else if (direction.x > 0.382f && direction.y < -0.382f)  // Down-Right
            newDirection = "facingDownRight";

        // Prevent re-triggering the same animation
        if (newDirection != lastFacingDirection)
        {
            animator.ResetTrigger(lastFacingDirection); // Clear previous trigger
            animator.SetTrigger(newDirection);
            lastFacingDirection = newDirection; // Update last known direction
        }
    }
    public void OnDeath() {
        this.enabled = false;
        if (audioSource != null && deathSound != null) {
            audioSource.PlayOneShot(deathSound);
        }
        animator.SetBool("death", true);
    }
    public void OnHit() {
        if (audioSource != null && deathSound != null) {
            audioSource.PlayOneShot(bossHitSound);
        }
    }
}