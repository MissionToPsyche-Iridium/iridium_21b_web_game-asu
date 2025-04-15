using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 460f;
    public float currentHealth = 460f;
    public static float healthScale = 1.0f;
    public GameObject healthBar;
    public GameObject goldDrop;
    public GameObject cobaltDrop;
    public GameObject iridiumDrop;
    public GameObject ironDrop;
    public GameObject nickelDrop;
    public GameObject pushBackPU;
    public GameObject dashPU;
    public GameObject dodgePU;
    public GameObject healthPack;
    public GameObject AllStatIncrease;
    private GameObject[] metals;
    private float newXScale;
    private Vector3 healthBar_Scale;
    private Vector3 healthBar_Pos;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = maxHealth * healthScale;
        currentHealth = currentHealth * healthScale;
        healthBar_Scale = healthBar.transform.localScale;
        healthBar_Pos = healthBar.transform.localPosition;
        metals = new GameObject[] { goldDrop, cobaltDrop, iridiumDrop, ironDrop, nickelDrop};
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return; // Prevent multiple death calls
        currentHealth = Mathf.Max(currentHealth - amount, 0f);

        // Update health bar
        newXScale = currentHealth / maxHealth;
        healthBar.transform.localScale = new Vector3(newXScale, healthBar_Scale.y, healthBar_Scale.z);
        healthBar.transform.localPosition = new Vector3(-1 * ((1 - newXScale) / 2), healthBar_Pos.y, healthBar_Pos.z);

        // Check for death
        if (currentHealth == 0)
        {
            randomDrop();
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

    bool inRange_inclusive(float value, float min, float max)
    {
        if (min <= value && value <= max)
        {
            return true;
        }
        return false;
    }

    void randomDrop()
    {
        float drop = UnityEngine.Random.Range(1, 151);
        if (drop == 1) //healthpack
        {
            Instantiate(healthPack, transform.position, Quaternion.identity);
        }
        else if (drop == 2) //pushback powerup
        {
            Instantiate(pushBackPU, transform.position, Quaternion.identity);
        }
        else if (drop == 3) //rocket powerup
        {
            Instantiate(dashPU, transform.position, Quaternion.identity);
        }
        else if (drop == 4) //dodge powerup
        {
            Instantiate(dodgePU, transform.position, Quaternion.identity);
        }
        else if (drop == 5) //All stat up
        {
            Instantiate(AllStatIncrease, transform.position, Quaternion.identity);
        }
        else
        {
            randomMetalDrop();
        }
    }

    void randomMetalDrop()
    {
        float drop = UnityEngine.Random.Range(0, 5);
        Instantiate(metals[Convert.ToInt32(drop)], transform.position, Quaternion.identity);
    }
}
