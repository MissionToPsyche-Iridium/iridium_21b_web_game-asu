using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Created by Robert DeLucia Jr. during Sprint 1
// Updated to have boost mechanic during Sprint 2
// Updated to have jump forward mechanic during Sprint 3
// Updated to rotate sprite based on movement direction during Sprint 5
public class PlayerMovement : MonoBehaviour
{
    public CoinManager cm;
    public float moveSpeed = 5f;
    public float speedBoostMultiplier = 1.5f;  // speed boost grants extra velocity
    public float boostDuration = 2f;         // boost is 2s long
    public float boostCooldown = 2f;         // cooldown is 2s long
    public float boostAvailabilityDuration = 30f; // how long the gear power-up gives the user the ability to boost
    public float dashAvailabilityDuration = 30f;
    public float dashDistance = 2.5f;        // how far to jump forward on double press
    public float doublePressThreshold = 0.3f; // max time between double presses
    public float dashCooldown = 2f;          // 2-second cooldown for dashing

    private bool isBoosting = false;
    private bool isOnCooldown = false;
    private bool isDashOnCooldown = false;   // flag to track if dashing is on cooldown
    private bool canUseBoost = false;
    private bool canUseDash = false;

    private float lastPressTimeX = -1f;      // track last press time for X-axis
    private float lastPressTimeY = -1f;      // track last press time for Y-axis

    private Vector3 lastNonZeroMovement = Vector3.zero; // Tracks the last non-zero movement for rotation
    public Animator animator;

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveX, moveY, 0).normalized;
        if (!animator.GetBool("Death")) {
            transform.position +=  moveSpeed * Time.deltaTime * movement;

            direction(movement);
        }

        // Rotate the sprite based on movement
        //RotateSprite(movement);

        // Will speed boost if not on cooldown and not currently boosting
        if (Input.GetKeyDown(KeyCode.Space) && canUseBoost && !isBoosting && !isOnCooldown)
        {
            StartCoroutine(SpeedBoost());
        }

        // Handle X-axis jump forward
        if (Input.GetButtonDown("Horizontal") && !isDashOnCooldown)
        {
            if (Time.time - lastPressTimeX < doublePressThreshold) // checks if keys were pressed rapidly in succession
            {
                Dash(Vector3.right * Mathf.Sign(Input.GetAxisRaw("Horizontal")));
            }
            lastPressTimeX = Time.time;
        }

        // Handle Y-axis jump forward
        if (Input.GetButtonDown("Vertical") && canUseDash && !isDashOnCooldown)
        {
            if (Time.time - lastPressTimeY < doublePressThreshold)
            {
                Dash(Vector3.up * Mathf.Sign(Input.GetAxisRaw("Vertical")));
            }
            lastPressTimeY = Time.time;
        }
    }

    // Function to dash the player forward
    void Dash(Vector3 direction)
    {
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

    // Speed boost method
    IEnumerator SpeedBoost()
    {
        isBoosting = true;
        moveSpeed *= speedBoostMultiplier;

        yield return new WaitForSeconds(boostDuration);  // Give boost for 2 seconds

        moveSpeed /= speedBoostMultiplier;  // After 2 seconds, revert to normal speed
        isBoosting = false;

        StartCoroutine(SpeedBoostCooldown());  // Start cooldown coroutine for speed boost
    }

    // Speed boost cooldown method
    IEnumerator SpeedBoostCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(boostCooldown);  // Cooldown is 2 seconds long
        isOnCooldown = false;
    }

    IEnumerator BoostAvailability()
    {
        canUseBoost = true;
        Debug.Log("Speed Boost activated! You have 30 seconds to use it.");

        yield return new WaitForSeconds(boostAvailabilityDuration);

        canUseBoost = false;
        Debug.Log("Speed Boost availability ended.");
    }
    IEnumerator DashAvailability()
    {
        canUseDash = true;
        Debug.Log("Dash activated! You have 30 seconds to use it.");

        yield return new WaitForSeconds(dashAvailabilityDuration);

        canUseDash = false;
        Debug.Log("Dash Availability Ended.");
    }

    // Handle collisions between trigger objects and player object
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            cm.coinCount++;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Gear"))
        {
            StartCoroutine(BoostAvailability());
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Rocket"))
        {
            StartCoroutine(DashAvailability());
            Destroy(other.gameObject);
        }
    }
    void direction(Vector3 move) {

        if (move.sqrMagnitude > 0) {
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
}