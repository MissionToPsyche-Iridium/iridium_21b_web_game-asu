using UnityEngine;
using TMPro;

public class RocketPowerup : MonoBehaviour
{
    public GameObject powerUpText; // Reference to UI text

    private void Start()
    {
        if (powerUpText != null)
        {
            powerUpText.SetActive(false); // Hide message initially
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ShowPowerUpMessage();
            Destroy(gameObject); // Remove the rocket power-up
        }
    }

    private void ShowPowerUpMessage()
    {
        if (powerUpText != null)
        {
            powerUpText.SetActive(true);
            Invoke("HidePowerUpMessage", 2f); 
        }
    }

    private void HidePowerUpMessage()
    {
        powerUpText.SetActive(false);
    }
}
