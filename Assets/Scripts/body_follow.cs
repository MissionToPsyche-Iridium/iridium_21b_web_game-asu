using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class body_follow : MonoBehaviour, IEnemyDeathHandler
{
    public float speed = 7.0f;
    public static float speedFactor = 1.0f;
    public float healthScale = 1.0f;
    public float delay = 0f;
    public float followDistance = .01f; // Distance to maintain between segments
    public Transform target;
    public RuntimeAnimatorController headController;
    public Sprite up, down, left, right, upLeft, upRight, downLeft, downRight; //For smoother transition when target dies
    public Sprite currentSprite;
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
    public GameObject dmgUpgrade;
    public GameObject firerateUpgrade;
    public GameObject healthUpgrade;
    public Animator animator;
    private bool waitingBeforeRedirect = false;
    private SpriteRenderer spriteRend;
    private basic_enemy_behavior movScript;
    private Vector3 direction;
    private Queue<Vector3> positionHistory = new Queue<Vector3>(); // Stores past positions
    private string lastFacingDirection = "";
    private bool ready = false;
    private bool hasNewBehavior = false;

    public AudioClip deathSound;
    public AudioClip bossHitSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRend = GetComponent<SpriteRenderer>();
        StartCoroutine(Wait(delay));
    }

    void Update()
    {
        if (target != null && ready && !hasNewBehavior)
        {
            // Add the target's position to the history
            positionHistory.Enqueue(target.position);

            // Ensure the queue doesn't grow indefinitely
            if (positionHistory.Count > Mathf.CeilToInt(delay / Time.fixedDeltaTime))
            {
                Vector3 targetPosition = positionHistory.Dequeue();
                direction = (targetPosition - transform.position).normalized;
                determineAnimationState(direction);
                float distance = Vector3.Distance(transform.position, targetPosition);
                if (distance > followDistance)
                {
                    // Dynamically adjust speed based on distance
                    float dynamicSpeed = Mathf.Lerp(speed * speedFactor, speed * speedFactor * 2, (distance - followDistance) / followDistance);
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition - direction * followDistance, dynamicSpeed * 2 * Time.deltaTime);
                }
            }
        }

        else if (target == null && !hasNewBehavior)
        {
            ApplyNewBehavior();
            ChangeSprite();
            waitingBeforeRedirect = true;
        }

        if (hasNewBehavior)
        {
            if (waitingBeforeRedirect)
            {
                StartCoroutine(ResumeNextFrame());
            }
            else
            {
                direction = direction = (movScript.target - transform.position).normalized;
                determineAnimationState(direction);
            }

        }
    }

    IEnumerator Wait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); // Ensure proper delay before movement starts
        ready = true;
    }

    void determineAnimationState(Vector3 direction)
    {

        string newDirection = "";

        if (direction.x >= 0.923f)
        { // Right
            newDirection = "facingRight";
            currentSprite = right;
        }
        else if (direction.x <= -0.923f)
        { // Left
            newDirection = "facingLeft";
            currentSprite = left;
        }
        else if (direction.y >= 0.923f)  // Up
        {
            newDirection = "facingUp";
            currentSprite = up;
        }
        else if (direction.y <= -0.923f)  // Down
        {
            newDirection = "facingDown";
            currentSprite = down;
        }
        else if (direction.x > 0.382f && direction.y > 0.382f)  // Up-Right
        {
            newDirection = "facingUpRight";
            currentSprite = upRight;
        }
        else if (direction.x < -0.382f && direction.y > 0.382f)  // Up-Left
        {
            newDirection = "facingUpLeft";
            currentSprite = upLeft;
        }
        else if (direction.x < -0.382f && direction.y < -0.382f)  // Down-Left
        {
            newDirection = "facingDownLeft";
            currentSprite = downLeft;
        }
        else if (direction.x > 0.382f && direction.y < -0.382f)  // Down-Right
        {
            newDirection = "facingDownRight";
            currentSprite = downRight;
        }

        // Prevent re-triggering the same animation
        if (newDirection != lastFacingDirection)
        {
            animator.ResetTrigger(lastFacingDirection); // Clear previous trigger
            animator.SetTrigger(newDirection);
            lastFacingDirection = newDirection; // Update last known direction
        }
    }

    void InitializeScript(EnemyHealth healthScript, Transform healthBar)
    {
        healthScript.currentHealth = 1500;
        healthScript.maxHealth = 1500;
        healthScript.deathDelay = 0f;
        healthScript.healthBar = healthBar.gameObject;
        healthScript.goldDrop = goldDrop;
        healthScript.cobaltDrop = cobaltDrop;
        healthScript.iridiumDrop = iridiumDrop;
        healthScript.ironDrop = ironDrop;
        healthScript.nickelDrop = nickelDrop;
        healthScript.pushBackPU = pushBackPU;
        healthScript.dashPU = dashPU;
        healthScript.dodgePU = dodgePU;
        healthScript.healthPack = healthPack;
        healthScript.damageUpgrade = dmgUpgrade;
        healthScript.firerateUpgrade = firerateUpgrade;
        healthScript.healthUpgrade = healthUpgrade;
        healthScript.AllStatIncrease = AllStatIncrease;
    }

    void ChangeSprite()
    {
        animator.runtimeAnimatorController = headController;
        if (currentSprite != null)
        {
            spriteRend.sprite = currentSprite;
        }
        else
        {
            spriteRend.sprite = up;
            Debug.Log("HEEEREEEEEE");
        }
        animator.runtimeAnimatorController = headController;

    }

    void ApplyNewBehavior()
    {

        //Giving basic behavior
        movScript = gameObject.AddComponent<basic_enemy_behavior>();

        //Allowing player damage
        EnemyHealth healthScript = gameObject.AddComponent<EnemyHealth>();
        Transform healthBar = transform.GetChild(0);
        InitializeScript(healthScript, healthBar);

        //Adding colliders for physics and damage
        CircleCollider2D triggerCollider = gameObject.AddComponent<CircleCollider2D>();
        CircleCollider2D physicsCollider = gameObject.AddComponent<CircleCollider2D>();
        triggerCollider.isTrigger = true;

        //Assigning as official enemy
        gameObject.tag = "basic_enemy";
        hasNewBehavior = true;
        movScript.speed = 4.0f;
        movScript.isBoss = true;
    }

    IEnumerator ResumeNextFrame()
    {
        yield return null; // Wait one frame
        animator.SetTrigger(lastFacingDirection);
        waitingBeforeRedirect = false;
    }

    public void OnDeath() {
        this.enabled = false;
        if (audioSource != null && deathSound != null) {
            audioSource.PlayOneShot(deathSound);
        }
        animator.SetBool("death", true);
    }
    public void OnHit() {
        if (audioSource != null && deathSound != null) {
            audioSource.PlayOneShot(bossHitSound);
        }
    }
}