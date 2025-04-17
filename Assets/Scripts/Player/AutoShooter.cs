using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoShooter : MonoBehaviour {
    public GameObject projectilePrefab; // Assign your Projectile prefab here
    public Transform firePoint; // Assign an empty child transform on the player where projectiles spawn
    public float fireRate = 2.5f; // How many seconds between shots
    public float detectionRadius = 10f; // Radius to detect enemies
    private float nextFireTime = 0f;
    public Animator animator;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Time.time >= nextFireTime) {
            if (AreEnemiesInRange()) {
                FireProjectile();
                nextFireTime = Time.time + (1f / fireRate);
            }
        }
    }

    bool AreEnemiesInRange() {
        // Find all enemies with the basic_enemy tag
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("basic_enemy");

        // Check if any enemies are within the detection radius
        foreach (GameObject enemy in enemies) {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy <= detectionRadius) {
                return true; // At least one enemy is in range
            }
        }

        return false; // No enemies in range
    }

    void FireProjectile() {
        if (projectilePrefab != null && firePoint != null) {

            //Need to find which direction the object is facing to play the correct animation
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
            Vector3 movement = new Vector3(moveX, moveY, 0).normalized;
            directionAnim(movement);

           // animator.SetTrigger("isAttacking");

            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }
        else {
            Debug.LogWarning("Projectile prefab or FirePoint is not assigned on AutoShooter.");
        }
    }
    void directionAnim(Vector3 move) {
        if (move.sqrMagnitude > 0) {
            animator.SetFloat("x", move.x);
            animator.SetFloat("y", move.y);
        }
    }
}
