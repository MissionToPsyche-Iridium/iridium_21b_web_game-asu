using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

// Created by Robert DeLucia Jr. during Sprint 1
// Updated to have boost mechanic during Sprint 2
// Updated to have jump forward mechanic during Sprint 3
// Updated to rotate sprite based on movement direction during Sprint 5
// Updated to include push-back power-up integration
public class PlayerMovement : MonoBehaviour
{

    private Health health;
    public int curHealth;
    public int maxHealth;

    private AutoShooter autoShooterScript;

    public Projectile projectileScript;

    public CoinManager cm;
    public float moveSpeed = 5f;
    public float speedBoostMultiplier = 1.5f;  // Speed boost grants extra velocity
    public float boostDuration = 5f;            // Boost lasts for 2 seconds
    public float boostCooldown = 2f;            // Boost cooldown is 2 seconds
    public float boostAvailabilityDuration = 30f; // Duration the boost power-up is available
    public float dashAvailabilityDuration = 30f;
    public float fireRateBoostDuration = 30f;
    public float dashDistance = 2.5f;           // Distance covered during a dash
    public float doublePressThreshold = 0.3f;   // Max time between double presses for dash
    public float dashCooldown = 1f;              // 1-second cooldown for dashing
    public int damageFactor = 1;

    // --- Push-Back Power-Up Variables ---
    public float pushBackRadius = 5f;            // Radius within which enemies are pushed
    public float pushBackForce = 500f;           // Force applied to enemies
    public float pushBackCooldown = 2f;          // Cooldown between push-back activations
    public float pushBackAvailabilityDuration = 30f; // Duration the push-back ability is available

    private bool isBoosting = false;
    private bool isOnCooldown = false;
    private bool isDashOnCooldown = false;        // Flag to track if dashing is on cooldown
    private bool canUseBoost = false;
    private bool canUseDash = false;

    // --- Push-Back State Flags ---
    private bool canUsePushBack = false;
    private bool isPushBackOnCooldown = false;

    private float lastPressTimeX = -1f;           // Track last press time for X-axis
    private float lastPressTimeY = -1f;           // Track last press time for Y-axis

    private Vector3 lastNonZeroMovement = Vector3.zero; // Tracks the last non-zero movement for rotation
    public Animator animator;

    public CinemachineVirtualCamera vcam;  // Reference to the Cinemachine virtual camera
    public float zoomOutSize = 7f;         // Zoomed-out size
    public float zoomInSize = 5f;          // Normal size
    public float zoomDuration = 0.5f;

    public float coinAttractionRadius = 3f;  // Radius within which coins are attracted
    public float coinAttractionSpeed = 3f;

    private void Start()
    {
        health = GetComponent<Health>();
        autoShooterScript = GetComponent<AutoShooter>();
        projectileScript = FindObjectOfType<Projectile>();
    }

    void Update()
    {
 
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(moveX, moveY, 0).normalized;
        if (!animator.GetBool("death"))
        {
            transform.position += moveSpeed * Time.deltaTime * movement;
            directionAnim(movement);
        }

        // Handle Speed Boost Activation
        if (Input.GetKeyDown(KeyCode.Space) && canUseBoost && !isBoosting && !isOnCooldown)
        {
            
            StartCoroutine(SpeedBoost());
            
        }

        // Handle X-axis Dash
        if (Input.GetButtonDown("Horizontal") && canUseDash && !isDashOnCooldown)
        {
            if (Time.time - lastPressTimeX < doublePressThreshold) // Checks for rapid double press
            {
                Dash(Vector3.right * Mathf.Sign(Input.GetAxisRaw("Horizontal")));
                //animator.SetTrigger("isDashing");
            }
            lastPressTimeX = Time.time;
            
        }

        // Handle Y-axis Dash
        if (Input.GetButtonDown("Vertical") && canUseDash && !isDashOnCooldown)
        {
            if (Time.time - lastPressTimeY < doublePressThreshold)
            {
                Dash(Vector3.up * Mathf.Sign(Input.GetAxisRaw("Vertical")));
               // animator.SetTrigger("isDashing");
            }
            lastPressTimeY = Time.time;
            
        }

        // --- Push-Back Activation ---
        if (Input.GetKeyDown(KeyCode.P) && canUsePushBack && !isPushBackOnCooldown)
        {
           Debug.Log("Pushback activated");
           ActivatePushBack();
        }

        AttractCoins();
    }

    void AttractCoins()
    {
        int layerMask = LayerMask.GetMask("Ignore Raycast");
        // Find all coins within the attraction radius
        Collider2D[] coinColliders = Physics2D.OverlapCircleAll(transform.position, coinAttractionRadius, layerMask);

        foreach (Collider2D coinCollider in coinColliders)
        {
            if (coinCollider.CompareTag("Coin"))
            {
                // Gradually move the coin towards the player
                coinCollider.transform.position = Vector3.MoveTowards(
                    coinCollider.transform.position,
                    transform.position,
                    coinAttractionSpeed * Time.deltaTime
                );
            }
        }
    }

    // Function to dash the player forward
    void Dash(Vector3 direction)
    {
        directionAnim(direction);
        

        animator.SetTrigger("isDashing");
        
        transform.position += direction * dashDistance;
        
        
        StartCoroutine(DashCooldown());  // Start dash cooldown after each dash
    }

    // Dash cooldown coroutine
    IEnumerator DashCooldown()
    {
        isDashOnCooldown = true;                // Set dash on cooldown
        
        yield return new WaitForSeconds(dashCooldown);  // Wait for the cooldown duration
        isDashOnCooldown = false;               // Dash is no longer on cooldown
    }

    // Speed boost coroutine
    IEnumerator SpeedBoost()
    {
        isBoosting = true;
        animator.SetBool("isSpeedBoosting", true);
        moveSpeed *= speedBoostMultiplier;

        StartCoroutine(ZoomCamera(zoomOutSize, zoomDuration));

        yield return new WaitForSeconds(boostDuration);  // Boost lasts for specified duration

        StartCoroutine(ZoomCamera(zoomInSize, zoomDuration));

        moveSpeed /= speedBoostMultiplier;  // Revert to normal speed
        isBoosting = false;
        animator.SetBool("isSpeedBoosting", false);


        StartCoroutine(SpeedBoostCooldown());  // Start cooldown coroutine for speed boost
    }

    IEnumerator ZoomCamera(float targetSize, float duration)
    {
        float startSize = vcam.m_Lens.OrthographicSize;  // Use Cinemachine lens size
        float time = 0;

        while (time < duration)
        {
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        vcam.m_Lens.OrthographicSize = targetSize;
    }

    // Speed boost cooldown coroutine
    IEnumerator SpeedBoostCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(boostCooldown);  // Cooldown duration
        isOnCooldown = false;
    }

    // Boost availability coroutine
    IEnumerator BoostAvailability()
    {
        canUseBoost = true;
        Debug.Log("Speed Boost activated! You have 30 seconds to use it.");

        yield return new WaitForSeconds(boostAvailabilityDuration);

        canUseBoost = false;
        Debug.Log("Speed Boost availability ended.");
    }

    // Dash availability coroutine
    IEnumerator DashAvailability()
    {
        canUseDash = true;
        Debug.Log("Dash activated! You have 30 seconds to use it.");

        yield return new WaitForSeconds(dashAvailabilityDuration);

        canUseDash = false;
        Debug.Log("Dash Availability Ended.");
    }

    // --- Push-Back Availability Coroutine ---
    IEnumerator PushBackAvailability()
    {
        canUsePushBack = true;
        Debug.Log("Push-Back activated! You have 30 seconds to use it.");

        yield return new WaitForSeconds(pushBackAvailabilityDuration);

        canUsePushBack = false;
        Debug.Log("Push-Back availability ended.");
    }

    // --- Push-Back Cooldown Coroutine ---
    IEnumerator PushBackCooldown()
    {
        isPushBackOnCooldown = true;
        yield return new WaitForSeconds(pushBackCooldown);  // Cooldown duration
        isPushBackOnCooldown = false;
    }

    IEnumerator FireRateAvailability()
    {
        autoShooterScript.fireRate = 5f;
        yield return new WaitForSeconds(fireRateBoostDuration);
        autoShooterScript.fireRate = 2.5f;
    }


    // Function to activate push-back
    void ActivatePushBack()
    {
        //Debug.Log("ActivatePushBack called");

        // Find all colliders within the pushBackRadius
        int layerMask = LayerMask.GetMask("Ignore Raycast");
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, pushBackRadius, layerMask);
        //Debug.Log($"Found {hitColliders.Length} colliders within radius");

        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("basic_enemy") ||
                collider.CompareTag("dash_enemy") ||
                collider.CompareTag("shoot_enemy"))
            {
                Rigidbody2D enemyRb = collider.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    Vector2 direction = (collider.transform.position - transform.position).normalized;
                    enemyRb.AddForce(direction * pushBackForce);
                }
                else
                {
                    Debug.LogWarning($"Enemy {collider.name} does not have a Rigidbody2D component.");
                }
            }
        }

        StartCoroutine(PushBackCooldown());
    }

    // Handle collisions between trigger objects and player object
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("basic_enemy") || other.gameObject.CompareTag("enemy_projectile"))
        {
            health.DamagePlayer(10 + damageFactor);
        }
        else if (other.gameObject.CompareTag("Coin"))
        {
            cm.coinCount++;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Rocket"))
        {
            StartCoroutine(BoostAvailability());
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Dash"))
        {
            StartCoroutine(DashAvailability());
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("PushBack")) // --- Handle Push-Back Power-Up ---
        {
            StartCoroutine(PushBackAvailability());
            Destroy(other.gameObject);
        }
        else if(other.gameObject.CompareTag("FireRate"))
        {
            //StartCoroutine(FireRateAvailability());
            autoShooterScript.fireRate += 2f;
            Destroy(GameObject.FindGameObjectWithTag("damage"));
            Destroy(GameObject.FindGameObjectWithTag("healthUp"));
            Destroy(other.gameObject);
        }
        else if(other.gameObject.CompareTag("damage"))
        {
            Debug.Log("DASSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS that good");
            Projectile.defaultDamageAmount += 25f;
            Debug.Log("New Damage: " + Projectile.defaultDamageAmount);
            Destroy(GameObject.FindGameObjectWithTag("healthUp"));
            Destroy(GameObject.FindGameObjectWithTag("FireRate"));
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("health"))
        {
            health.healPlayerToFull();
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("healthUp"))
        {
            health.maxHealth += 50;
            health.curHealth += 50;
            health.invincibilityTime += .25f;
            health.healthBar.SetHealth(health.curHealth);
            Destroy(GameObject.FindGameObjectWithTag("damage"));
            Destroy(GameObject.FindGameObjectWithTag("FireRate"));
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("AllStatsUp"))
        {
            //Damage
            Projectile.defaultDamageAmount += 25f;
            //Health
            health.maxHealth += 50;
            health.curHealth += 50;
            health.invincibilityTime += .25f;
            //FireRate
            autoShooterScript.fireRate += 2f;
            Debug.Log(Projectile.defaultDamageAmount + " " + health.maxHealth + " " + autoShooterScript.fireRate);

            Destroy(other.gameObject);
        }
    }

    void directionAnim(Vector3 move)
    {
        if (move.sqrMagnitude > 0)
        {
            animator.SetFloat("x", move.x);
            animator.SetFloat("y", move.y);
        }
    }

    // Rotate the sprite based on movement direction
    void RotateSprite(Vector3 movement)
    {
        if (movement != Vector3.zero) // Only rotate if there's movement
        {
            lastNonZeroMovement = movement; // Update the last non-zero movement
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg; // Calculate angle
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 180)); // Rotate sprite (offset to face "north")
        }
    }

    // draws pushback radius for visual indication
    private void OnDrawGizmosSelected()
    {

        // Draw Push-Back Radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pushBackRadius);
        
    }
}