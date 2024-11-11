using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CoinManager cm;
    public float moveSpeed = 5f;
    public float speedBoostMultiplier = 2f;
    public float boostDuration = 2f;
    public float boostCooldown = 2f;
    public float dashDistance = 2.5f;
    public float doublePressThreshold = 0.3f;
    public float dashCooldown = 2f;

    private bool isBoosting = false;
    private bool isOnCooldown = false;
    private bool isDashOnCooldown = false;

    private float lastPressTimeX = -1f;
    private float lastPressTimeY = -1f;

    private Rigidbody2D rb;
    private Vector2 lastMovement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed;
        float moveY = Input.GetAxis("Vertical") * moveSpeed;

        lastMovement = new Vector2(moveX, moveY);

        // Only move if there’s no collision
        MovePlayer();

        if (Input.GetKeyDown(KeyCode.Space) && !isBoosting && !isOnCooldown)
        {
            StartCoroutine(SpeedBoost());
        }

        if (Input.GetButtonDown("Horizontal") && !isDashOnCooldown)
        {
            if (Time.time - lastPressTimeX < doublePressThreshold)
            {
                Dash(Vector3.right * Mathf.Sign(Input.GetAxisRaw("Horizontal")));
            }
            lastPressTimeX = Time.time;
        }

        if (Input.GetButtonDown("Vertical") && !isDashOnCooldown)
        {
            if (Time.time - lastPressTimeY < doublePressThreshold)
            {
                Dash(Vector3.up * Mathf.Sign(Input.GetAxisRaw("Vertical")));
            }
            lastPressTimeY = Time.time;
        }
    }

    void MovePlayer()
    {
        rb.MovePosition(rb.position + lastMovement * Time.fixedDeltaTime);
    }

    void Dash(Vector3 direction)
    {
        rb.MovePosition(rb.position + (Vector2)(direction * dashDistance));
        StartCoroutine(DashCooldown());
    }

    IEnumerator DashCooldown()
    {
        isDashOnCooldown = true;
        yield return new WaitForSeconds(dashCooldown);
        isDashOnCooldown = false;
    }

    IEnumerator SpeedBoost()
    {
        isBoosting = true;
        moveSpeed *= speedBoostMultiplier;

        yield return new WaitForSeconds(boostDuration);

        moveSpeed /= speedBoostMultiplier;
        isBoosting = false;

        StartCoroutine(SpeedBoostCooldown());
    }

    IEnumerator SpeedBoostCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(boostCooldown);
        isOnCooldown = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Boulder"))
        {
            // Stop the player on collision with the boulder
            lastMovement = Vector2.zero;
        }
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
