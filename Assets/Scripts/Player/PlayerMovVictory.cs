using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class PlayerMovVictory : MonoBehaviour {
    private Health health;
    private int partsCollected = 0;
    public int curHealth;
    public int maxHealth;

    private AutoShooter autoShooterScript;

    private Projectile projectileScript;

    private Rigidbody2D rigidBody;

    public CoinManager cm;
    public float moveSpeed = 8f;
    private float originalMoveSpeed;
    public float speedBoostMultiplier = 1.5f;  // Speed boost grants extra velocity
    public float boostDuration = 5f;            // Boost lasts for 2 seconds
    public float boostCooldown = 2f;            // Boost cooldown is 2 seconds
    public float boostAvailabilityDuration = 30f; // Duration the boost power-up is available
    public float dashAvailabilityDuration = 30f;
    public float fireRateBoostDuration = 30f;
    public float dashDistance = 4f;
    public float doublePressThreshold = 0.3f;
    public float dashCooldown = 2f;
    public int damageFactor = 1;


    public GameObject abilityEffectPrefab;        // object to spawn for pushback animation

    private float lastPressTimeX = -1f;           // Track last press time for X-axis
    private float lastPressTimeY = -1f;           // Track last press time for Y-axis

    private Vector3 lastNonZeroMovement = Vector3.zero; // Tracks the last non-zero movement for rotation
    public Animator animator;

    public CinemachineVirtualCamera vcam;  // Reference to the Cinemachine virtual camera
    public float zoomOutSize = 7f;         // Zoomed-out size
    public float zoomInSize = 5f;          // Normal size
    public float zoomDuration = 0.5f;

    
   


    private void Start() {
        
        health = GetComponent<Health>();
        autoShooterScript = GetComponent<AutoShooter>();
        projectileScript = FindObjectOfType<Projectile>();
        rigidBody = GetComponent<Rigidbody2D>();
        
        originalMoveSpeed = moveSpeed;

    }

    void Update() {

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(moveX, moveY, 0).normalized;
        if (!animator.GetBool("death")) {
            transform.position += moveSpeed * Time.deltaTime * movement;
            AnimDirection(movement);
        }

     

        // Dash checking
        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)) {
            Vector3 dashDirection = new Vector3(
                    Input.GetAxisRaw("Horizontal"),
                    Input.GetAxisRaw("Vertical"),
                    0
                ).normalized;


        }


        
    }



    // Handle collisions between trigger objects and player object
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("basic_enemy") || other.gameObject.CompareTag("enemy_projectile")) {
            health.DamagePlayer(10 + damageFactor);
        }
        

       
    }
    void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.CompareTag("Slow")) {
            moveSpeed = originalMoveSpeed;
        }
    }
    /**
     * Need this function for animation
     */
    void AnimDirection(Vector3 move) {
        if (move.sqrMagnitude > 0) {
            animator.SetFloat("x", move.x);
            animator.SetFloat("y", move.y);
        }
    }



}