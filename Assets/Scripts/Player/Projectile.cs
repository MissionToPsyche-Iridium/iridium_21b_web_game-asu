using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f; // How fast the projectile travels
    public static float defaultDamageAmount = 50f; // Damage dealt to the enemy
    public float lifetime = 2f; // How long the projectile exists
    public float detectionRadius = 10f; // Radius around the player to detect enemies
    private Vector3 moveDirection;

    public float damageAmount;

    // Start is called before the first frame update
    void Start()
    {
        damageAmount = defaultDamageAmount;
        // Destroys the projectile after 'lifetime' seconds (safety net)
        Destroy(gameObject, lifetime);

        // Find the player
        GameObject player = GameObject.FindGameObjectWithTag("PlayerTag");
        if (player == null)
        {
            Debug.LogWarning("No player found in the scene!");
            moveDirection = Vector3.up;
            return;
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("basic_enemy");

        // List to store enemies within detection radius
        List<GameObject> eligibleEnemies = new List<GameObject>();

        if (enemies.Length > 0)
        {
            // Filter enemies within detection radius of the player
            foreach (GameObject enemy in enemies)
            {
                float distanceToPlayer = Vector3.Distance(player.transform.position, enemy.transform.position);
                if (distanceToPlayer <= detectionRadius)
                {
                    eligibleEnemies.Add(enemy);
                }
            }

            // If we have eligible enemies, find the nearest one
            if (eligibleEnemies.Count > 0)
            {
                GameObject nearestEnemy = null;
                float minDistance = Mathf.Infinity;
                Vector3 currentPos = transform.position;

                // Loop through eligible enemies and find the closest one
                foreach (GameObject enemy in eligibleEnemies)
                {
                    float distance = Vector3.Distance(currentPos, enemy.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestEnemy = enemy;
                    }
                }

                // If we found an enemy, calculate the normalized directionAnim from our position to the enemy
                if (nearestEnemy != null)
                {
                    moveDirection = (nearestEnemy.transform.position - currentPos).normalized;
                    // Rotate the projectile so that its right side (local X-axis) points in the desired directionAnim.
                    float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                }
            }
        }

        if (moveDirection == Vector3.zero)
        {
            moveDirection = Vector3.up;
        }
    }

    void Update()
    {
        // Move the projectile forward (assuming it's facing right in local space)
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    // Called when the projectile collides with another object
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("basic_enemy"))
        {
            // Check for EnemyHealth component and apply damage
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount);
            }

            // Destroy the projectile after hitting the enemy
            Destroy(gameObject);
        }
        else if (other.CompareTag("wall"))
        {
            Destroy(gameObject);
        }
    }
}
