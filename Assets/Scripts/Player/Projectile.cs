using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f; // How fast the projectile travels
    public int damageAmount = 50; // Damage dealt to the enemy
    public float lifetime = 2f; // How long the projectile exists
    private Vector3 moveDirection;

    // Start is called before the first frame update
    void Start()
    {
        // Destroys the projectile after 'lifetime' seconds (safety net)
        Destroy(gameObject, lifetime);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("basic_enemy");

        if (enemies.Length > 0)
        {
            GameObject nearestEnemy = null;
            float minDistance = Mathf.Infinity;
            Vector3 currentPos = transform.position;

            // Loop through enemies and find the closest one
            foreach (GameObject enemy in enemies)
            {
                float distance = Vector3.Distance(currentPos, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestEnemy = enemy;
                }
            }

            // If we found an enemy, calculate the normalized direction from our position to the enemy
            if (nearestEnemy != null)
            {
                moveDirection = (nearestEnemy.transform.position - currentPos).normalized;
                // Rotate the projectile so that its right side (local X-axis) points in the desired direction.
                float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
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
        // Adjust direction if needed
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
    }
}
