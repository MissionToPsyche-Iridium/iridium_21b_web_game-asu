using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int curHealth = 100;
    public int maxHealth = 200;
    public HealthBar healthBar;
    public Animator animator;
    public GameObject gameOverScreen; // Reference to Game Over UI
    public GameObject metalsCollected;
    public GameObject partsCollected;

    // Damage audio vars
    public AudioClip damageSFX;
    private AudioSource damageSFXfile;

    // Invincibility variables
    public float invincibilityTime = 0.5f; // Time player is invincible after taking damage
    private bool isInvincible = false;

    // Sprite renderer for blinking effect
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        curHealth = maxHealth;
        damageSFXfile = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Ensure Game Over screen is hidden at start
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }
    }

    public void DamagePlayer(int damage)
    {
        // Only take damage if not invincible
        if (!isInvincible)
        {
            curHealth -= damage;
            healthBar.SetHealth(curHealth, maxHealth);
            damageSFXfile.PlayOneShot(damageSFX, .75f); // Play damage audio 

            // Start invincibility
            StartCoroutine(InvincibilityFrames());

            if (curHealth <= 0)
            {
                animator.SetBool("death", true); // Trigger death animation
                StartCoroutine(ShowGameOverScreen(2f)); // Wait before showing Game Over
            }
        }
    }

    public void healPlayerToFull() 
    {
        curHealth = maxHealth;
        healthBar.SetHealth(curHealth, maxHealth);
    }

    public void setMaxHealth(int maxHP)
    {
        maxHealth = maxHP;
    }

    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        float invincibilityTimer = 0f;
        Color originalColor = spriteRenderer.color;

        while (invincibilityTimer < invincibilityTime)
        {
            // Lerp between original color and red
            for (float t = 0; t < 1; t += Time.deltaTime * 10f)
            {
                spriteRenderer.color = Color.Lerp(originalColor, Color.red, Mathf.PingPong(t, 0.5f));
                yield return null;
            }

            invincibilityTimer += 0.5f;
        }

        // Reset to original color
        spriteRenderer.color = originalColor;
        isInvincible = false;
    }

    private IEnumerator ShowGameOverScreen(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); // Wait for animation

        if (gameOverScreen != null)
        {
            //metalsCollected.setActive(false);
            //partsCollected.setActive(false);
            gameOverScreen.SetActive(true); // Show Game Over screen
            Time.timeScale = 0f; // Pause the game
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume time before restarting
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload the scene
    }
    public void OnDeathAnimationEnd() {
        // disable renderers on death
        GetComponent<SpriteRenderer>().enabled = false;

        // disable all child renderers
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) {
            sr.enabled = false;
        }

        // Disable hitboxes on death
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>()) {
            col.enabled = false;
        }
    }
}