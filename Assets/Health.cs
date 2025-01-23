// Written by Robert Delucia Jr. 11/11/24 for Sprint 4

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Health : MonoBehaviour
{
    public int curHealth = 0;
    public int maxHealth = 100;
    public HealthBar healthBar;
    public Animator animator;
    void Start()
    {
        curHealth = maxHealth;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DamagePlayer(10);
        }
        if(Input.GetKeyDown(KeyCode.LeftControl)) {

            DamagePlayer(maxHealth);
        }
        
    }
    public void DamagePlayer(int damage)
    {
        curHealth -= damage;
        healthBar.SetHealth(curHealth);

        if (curHealth <= 0)
        {
            
            animator.SetBool("Death", true);
            
            StartCoroutine(WaitAndRestart(2f));

        }
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("You Died!");
    }
    private IEnumerator WaitAndRestart(float waitTime) {
        yield return new WaitForSeconds(waitTime);  // Wait for the specified time
       // DespawnPlayer();
        
        RestartGame();  // Restart the game after the wait
    }
  
    
    void OnDeathAnimationEnd() {

        MakeTransparent(); // Destroy the game object once the death animation is finished

    }
    public void MakeTransparent() {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) {
            Color color = spriteRenderer.color;
            color.a = 0f;  // Set alpha to 0 for full transparency
            spriteRenderer.color = color;
        }

        // Optionally, disable the collider if needed
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) {
            collider.enabled = false;
        }
    }

}