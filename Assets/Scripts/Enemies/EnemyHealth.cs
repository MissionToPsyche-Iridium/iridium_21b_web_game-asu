using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;
    public GameObject healthBar;
    private float newXScale;
    private Vector3 healthBar_Scale;
    // Start is called before the first frame update
    void Start()
    {
        healthBar_Scale = healthBar.transform.localScale;
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth > 0)
        {
            currentHealth -= amount;
            newXScale = (float)currentHealth / maxHealth;
            healthBar.transform.localScale = new Vector3(newXScale, healthBar_Scale.y, healthBar_Scale.z);
        } 
        else
        {
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
