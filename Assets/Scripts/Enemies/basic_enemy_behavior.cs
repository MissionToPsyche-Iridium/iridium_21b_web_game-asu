using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basic_enemy_behavior : MonoBehaviour
{
    public GameObject Player;
    public bool hasTarget = false;
    private float lastSeenX;
    private float lastSeenY;
    private float lastEnemyX;
    private float lastEnemyY;
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
            GameObject nearest = findNearestEnemy();
            if (nearest != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, nearest.transform.position, step);
            }
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

    GameObject findNearestEnemy()
    {
        GameObject nearest = null;
        float minDist = float.MaxValue;

        enemyObjects = GameObject.FindGameObjectsWithTag("basic_enemy");

        foreach (GameObject enemy in enemyObjects)
        {
            if (enemy == this.gameObject)
                continue;

            basic_enemy_behavior enemyBehavior = enemy.GetComponent<basic_enemy_behavior>();
            if (enemyBehavior != null && enemyBehavior.hasTarget)
            {
                float dist = Vector3.Distance(transform.position, enemy.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = enemy;
                }
            }
        }

        return nearest;
    }

    bool isMoving(float x, float y)
    {
        if (Mathf.Approximately(x, lastEnemyX) && Mathf.Approximately(y, lastEnemyY))
        {
            return false;
        }
        else
        {
            lastEnemyX = x;
            lastEnemyY = y;
            return true;
        }
    }
}