using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 460f;
    public float currentHealth = 460f;
    public GameObject healthBar;
    public GameObject collectible;
    public GameObject pushBackPU;
    public GameObject dashPU;
    public GameObject dodgePU;
    public GameObject healthPack;
    private float newXScale;
    private Vector3 healthBar_Scale;
    private Vector3 healthBar_Pos;
    // Start is called before the first frame update
    void Start()
    {
        healthBar_Scale = healthBar.transform.localScale;
        healthBar_Pos = healthBar.transform.localPosition;
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return; // Prevent multiple death calls

        Debug.Log($"Projectile Damage: {amount}");
        Debug.Log($"Enemy Current Health Before Damage: {currentHealth}");

        currentHealth = Mathf.Max(currentHealth - amount, 0f);
        Debug.Log($"Enemy Current Health After Damage: {currentHealth}");

        // Update health bar
        newXScale = currentHealth / maxHealth;
        healthBar.transform.localScale = new Vector3(newXScale, healthBar_Scale.y, healthBar_Scale.z);
        healthBar.transform.localPosition = new Vector3(-1 * ((1 - newXScale) / 2), healthBar_Pos.y, healthBar_Pos.z);

        // Check for death
        if (currentHealth == 0)
        {
            Instantiate(collectible, transform.position, Quaternion.identity);
            Die();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Die()
    {
        // Here you can add death effects, animations, sounds, etc.
        Destroy(gameObject);
    }
}
