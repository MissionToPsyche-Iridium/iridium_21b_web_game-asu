using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Created by Robert DeLucia Jr. during Sprint 1
// Updated to have boost mechanic during Sprint 2
public class PlayerMovement : MonoBehaviour
{
    public CoinManager cm;
    public float moveSpeed = 5f;
    public float speedBoostMultiplier = 2f;  // speed boost grants double velocity
    public float boostDuration = 2f;         // boost is 2s long
    public float boostCooldown = 2f;         // cooldown is 2s long
    public float dashDistance = 5f;          // how far to jump forward on double press
    public float doublePressThreshold = 0.3f; // max time between double presses

    private bool isBoosting = false;
    private bool isOnCooldown = false;

    private float lastPressTimeX = -1f;      // track last press time for X-axis
    private float lastPressTimeY = -1f;      // track last press time for Y-axis

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveY = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        Vector3 movement = new Vector3(moveX, moveY, 0);
        transform.position += movement;

        // Handle speed boost with spacebar
        if (Input.GetKeyDown(KeyCode.Space) && !isBoosting && !isOnCooldown)
        {
            StartCoroutine(SpeedBoost());
        }

        // Handle double press for "jump forward" on X-axis
        if (Input.GetButtonDown("Horizontal"))
        {
            if (Time.time - lastPressTimeX < doublePressThreshold)
            {
                Dash(Vector3.right * Mathf.Sign(Input.GetAxisRaw("Horizontal")));  // Dash in the X direction
            }
            lastPressTimeX = Time.time;
        }

        // Handle double press for "jump forward" on Y-axis
        if (Input.GetButtonDown("Vertical"))
        {
            if (Time.time - lastPressTimeY < doublePressThreshold)
            {
                Dash(Vector3.up * Mathf.Sign(Input.GetAxisRaw("Vertical")));  // Dash in the Y direction
            }
            lastPressTimeY = Time.time;
        }
    }

    // Function to dash the player forward
    void Dash(Vector3 direction)
    {
        transform.position += direction * dashDistance;
    }

    IEnumerator SpeedBoost()
    {
        isBoosting = true;
        moveSpeed *= speedBoostMultiplier;

        yield return new WaitForSeconds(boostDuration);  // give boost for 2 seconds

        moveSpeed /= speedBoostMultiplier;  // after 2 seconds is up, revert to normal speed
        isBoosting = false;

        StartCoroutine(SpeedBoostCooldown()); // cooldown coroutine
    }

    IEnumerator SpeedBoostCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(boostCooldown);  // cooldown is 2 seconds long
        isOnCooldown = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            cm.coinCount++;
            Destroy(other.gameObject);
        }
    }
}
