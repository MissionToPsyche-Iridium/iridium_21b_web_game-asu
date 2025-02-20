using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int curHealth = 100;
    public int maxHealth = 100;
    public HealthBar healthBar;
    public Animator animator;
    public GameObject gameOverScreen; // Reference to Game Over UI

    // Damage audio vars
    public AudioClip damageSFX;
    private AudioSource damageSFXfile;

    void Start()
    {
        curHealth = maxHealth;
        damageSFXfile = GetComponent<AudioSource>();

        // Ensure Game Over screen is hidden at start
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DamagePlayer(10);
        }
    }

    public void DamagePlayer(int damage)
    {
        curHealth -= damage;
        healthBar.SetHealth(curHealth);
        damageSFXfile.PlayOneShot(damageSFX, .75f); // Play damage audio 

        if (curHealth <= 0)
        {
            animator.SetBool("death", true); // Trigger death animation
            StartCoroutine(ShowGameOverScreen(2f)); // Wait before showing Game Over
        }
    }

    private IEnumerator ShowGameOverScreen(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); // Wait for animation

        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true); // Show Game Over screen
            Time.timeScale = 0f; // Pause the game
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume time before restarting
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload the scene
    }
}
