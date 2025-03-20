// Written by Robert Delucia Jr. 11/11/24 for Sprint 4

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    public Slider healthBar;
    public Health playerHealth;
    private void Start()
    {
        playerHealth = GameObject.FindGameObjectWithTag("PlayerTag").GetComponent<Health>();
        healthBar = GetComponent<Slider>();
        healthBar.maxValue = playerHealth.maxHealth;
        healthBar.value = playerHealth.maxHealth;
    }
    public void SetHealth(int hp)
    {
        healthBar.maxValue = playerHealth.maxHealth;
        healthBar.value = hp;
    }
}