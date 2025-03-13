using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;
    public GameObject healthBar;
    public GameObject collectible;
    private float newXScale;
    private Vector3 healthBar_Scale;
    private Vector3 healthBar_Pos;
    // Start is called before the first frame update
    void Start()
    {
        healthBar_Scale = healthBar.transform.localScale;
        healthBar_Pos = healthBar.transform.localPosition;
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth > 0)
        {
            currentHealth -= amount;
            newXScale = (float)currentHealth / maxHealth;
            healthBar.transform.localScale = new Vector3(newXScale, healthBar_Scale.y, healthBar_Scale.z);
            healthBar.transform.localPosition = new Vector3(-1 * ((1 - newXScale)/2), healthBar_Pos.y, healthBar_Pos.z);
        } 
        else
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
