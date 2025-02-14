using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f; // How fast the projectile travels
    public int damageAmount = 40; // Damage dealt to the enemy
    public float lifetime = 2f; // How long the projectile exists

    // Start is called before the first frame update
    void Start()
    {
        // Destroys the projectile after 'lifetime' seconds (safety net)
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move the projectile forward (assuming it's facing right in local space)
        // Adjust direction if needed
        transform.Translate(Vector3.right * speed * Time.deltaTime);
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
        else if (!other.CompareTag("PlayerTag"))
        {
            // If it hits a wall or another solid non-trigger object, destroy as well (optional)
            Destroy(gameObject);
        }
    }
}
