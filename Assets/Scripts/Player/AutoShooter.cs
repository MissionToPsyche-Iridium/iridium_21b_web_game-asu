using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoShooter : MonoBehaviour
{
    public GameObject projectilePrefab; // Assign your Projectile prefab here
    public Transform firePoint; // Assign an empty child transform on the player where projectiles spawn
    public float fireRate = 1f; // How many seconds between shots
    private float nextFireTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            FireProjectile();
            nextFireTime = Time.time + (1f / fireRate);
        }
    }

    void FireProjectile()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }
        else
        {
            Debug.LogWarning("Projectile prefab or FirePoint is not assigned on AutoShooter.");
        }
    }
}
