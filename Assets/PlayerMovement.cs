using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Created by Robert DeLucia Jr. during Sprint 1
// Updated to have boost mechanic during Sprint 2
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float speedBoostMultiplier = 2f;  // speed boost grants double velocity
    public float boostDuration = 2f;         // boost is 2s long
    public float boostCooldown = 2f;         // cooldown is 2s long
    private bool isBoosting = false;         
    private bool isOnCooldown = false;       

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveY = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        Vector3 movement = new Vector3(moveX, moveY, 0);
        transform.position += movement;

        // if spacebar is pressed and not currently boosting and not on cooldown, start boost
        if (Input.GetKeyDown(KeyCode.Space) && !isBoosting && !isOnCooldown)
        {
            StartCoroutine(SpeedBoost());
        }
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
}
