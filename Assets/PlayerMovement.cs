using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Created by Robert DeLucia Jr. during Sprint 1
// Updated to have boost mechanic during Sprint 2
// Updated to have jump forward mechanic during Sprint 3
public class PlayerMovement : MonoBehaviour
{
    public CoinManager cm;
    public float moveSpeed = 5f;
    public float speedBoostMultiplier = 2f;  // speed boost grants double velocity
    public float boostDuration = 2f;         // boost is 2s long
    public float boostCooldown = 2f;         // cooldown is 2s long
    public float dashDistance = 2.5f;        // how far to jump forward on double press
    public float doublePressThreshold = 0.3f; // max time between double presses
    public float dashCooldown = 2f;          // 2-second cooldown for dashing

    private bool isBoosting = false;
    private bool isOnCooldown = false;
    private bool isDashOnCooldown = false;   // flag to track if dashing is on cooldown

    private float lastPressTimeX = -1f;      // track last press time for X-axis
    private float lastPressTimeY = -1f;      // track last press time for Y-axis

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveY = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        Vector3 movement = new Vector3(moveX, moveY, 0);
        transform.position += movement;

        // Will speed boost if not on cooldown and not currently boosting
        if (Input.GetKeyDown(KeyCode.Space) && !isBoosting && !isOnCooldown)
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
        if (Input.GetButtonDown("Vertical") && !isDashOnCooldown)
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

    // Handle collisions between coins and player object
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            cm.coinCount++;
            Destroy(other.gameObject);
        }
    }
}