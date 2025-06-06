using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

// Created by Robert DeLucia Jr. during Sprint 1
// Updated to have boost mechanic during Sprint 2
// Updated to have jump forward mechanic during Sprint 3
// Updated to rotate sprite based on movement direction during Sprint 5
// Updated to include push-back power-up integration
public class PlayerMovement : MonoBehaviour
{

    private Health health;
    private int partsCollected = 0;
    public int curHealth;
    public int maxHealth;

    private AutoShooter autoShooterScript;

    private Projectile projectileScript;

    private Rigidbody2D rigidBody;

    public CoinManager cm;
    public float moveSpeed = 8f;
    private float originalMoveSpeed;
    public float speedBoostMultiplier = 1.5f;  // Speed boost grants extra velocity
    public float boostDuration = 5f;            // Boost lasts for 2 seconds
    public float boostCooldown = 2f;            // Boost cooldown is 2 seconds
    public float boostAvailabilityDuration = 30f; // Duration the boost power-up is available
    public float dashAvailabilityDuration = 30f;
    public float fireRateBoostDuration = 30f;
    public float dashDistance = 4f;
    public float doublePressThreshold = 0.3f;
    public float dashCooldown = 2f;
    public int damageFactor = 1;
    public Text metalText;

    // --- Push-Back Power-Up Variables ---
    public float pushBackRadius = 5f;            // Radius within which enemies are pushed
    public float pushBackForce = 500f;           // Force applied to enemies
    public float pushBackCooldown = 3f;          // Cooldown between push-back activations
    public float pushBackAvailabilityDuration = 30f; // Duration the push-back ability is available

    private bool isBoosting = false;
    private bool isOnCooldown = false;
    private bool isDashOnCooldown = false;        // Flag to track if dashing is on cooldown
    private bool canUseBoost = false;
    private bool canUseDash = false;

    // --- Push-Back State Flags ---
    private bool canUsePushBack = false;
    private bool isPushBackOnCooldown = false;
    public GameObject abilityEffectPrefab;        // object to spawn for pushback animation

    private float lastPressTimeX = -1f;           // Track last press time for X-axis
    private float lastPressTimeY = -1f;           // Track last press time for Y-axis

    private Vector3 lastNonZeroMovement = Vector3.zero; // Tracks the last non-zero movement for rotation
    public Animator animator;

    public CinemachineVirtualCamera vcam;  // Reference to the Cinemachine virtual camera
    public float zoomOutSize = 7f;         // Zoomed-out size
    public float zoomInSize = 5f;          // Normal size
    public float zoomDuration = 0.5f;

    public float coinAttractionRadius = 3f;  // Radius within which coins are attracted
    public float coinAttractionSpeed = 3f;

    private bool firstIridiumCollected = false;
    private bool firstGoldCollected = false;
    private bool firstNickelCollected = false;
    private bool firstIronCollected = false;
    private bool firstColbaltCollected = false;
    private int numIridium = 0;
    private int numGold = 0;
    private int numNickel = 0;
    private int numCobalt = 0;
    private int numIron = 0;
    private int indexInList = 0;
    private int indexInList2 = 0;

    private bool firstAllStatUpCollected = false;
    private bool firstDamageCollected = false;
    private bool firstDashCollected = false;
    private bool firstFullHealthCollected = false;
    private bool firstFireRateCollected = false;
    private bool firstHealthUpCollected = false;
    private bool firstPushbackCollected = false;
    private bool firstRocketCollected = false;

    private Dictionary<string, Action> popupDict = new Dictionary<string, Action>();
    private List<GameObject> metalsLog = new List<GameObject>();
    private List<GameObject> powerupsLog = new List<GameObject>();

    [SerializeField] private Sprite iridiumSprite;
    [SerializeField] private Sprite goldSprite;
    [SerializeField] private Sprite ironSprite;
    [SerializeField] private Sprite nickelSprite;
    [SerializeField] private Sprite cobaltSprite;

    [SerializeField] private Sprite allStatUpSprite;
    [SerializeField] private Sprite damageSprite;
    [SerializeField] private Sprite dashSprite;
    [SerializeField] private Sprite fullHealthSprite;
    [SerializeField] private Sprite fireRateSprite;
    [SerializeField] private Sprite healthUpSprite;
    [SerializeField] private Sprite PushbackSprite;
    [SerializeField] private Sprite RocketSprite;

    [SerializeField] private GameObject UpgradeMenu;
    [SerializeField] private List<String> powerupLog = new List<String>();
    [SerializeField] private List<String> metalLog = new List<String>();
    public GameObject pushBackPrefab;
    public Text metalLogCount;
    public Text powerupLogCount;
    private IridiumPopupManager popupManager;
    public AudioClip itemSound;
    public AudioClip powerUpSound;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        health = GetComponent<Health>();
        autoShooterScript = GetComponent<AutoShooter>();
        projectileScript = FindObjectOfType<Projectile>();
        rigidBody = GetComponent<Rigidbody2D>();
        popupManager = FindObjectOfType<IridiumPopupManager>(true);
        originalMoveSpeed = moveSpeed;
        fillDict();

        if (popupManager == null)
        {
            Debug.LogError("PopupManager not found in the scene!");
        }
        else
        {
            Debug.Log("PopupManager found successfully.");
        }
    }

    void Update()
    {

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(moveX, moveY, 0).normalized;
        if (!animator.GetBool("death"))
        {
            transform.position += moveSpeed * Time.deltaTime * movement;
            AnimDirection(movement);
        }

        // Handle Speed Boost Activation
        if (Input.GetKeyDown(KeyCode.Space) && canUseBoost && !isBoosting && !isOnCooldown)
        {
            StartCoroutine(SpeedBoost());
        }

        // Dash checking
        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) &&
            (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)))
        {
            if (canUseDash && !isDashOnCooldown)
            {
                // Create a diagonal direction vector
                Vector3 dashDirection = new Vector3(
                    Input.GetAxisRaw("Horizontal"),
                    Input.GetAxisRaw("Vertical"),
                    0
                ).normalized;

                Dash(dashDirection);
            }
        }

        // --- Push-Back Activation ---
        if (Input.GetKeyDown(KeyCode.P) && canUsePushBack && !isPushBackOnCooldown)
        {
            Debug.Log("Pushback activated");
            ActivatePushBack();
        }

        AttractCoins();
    }

    void AttractCoins()
    {
        int layerMask = LayerMask.GetMask("Ignore Raycast");
        // Find all coins within the attraction radius
        Collider2D[] coinColliders = Physics2D.OverlapCircleAll(transform.position, coinAttractionRadius, layerMask);

        foreach (Collider2D coinCollider in coinColliders)
        {
            if (coinCollider.CompareTag("Coin") || coinCollider.CompareTag("Iron") ||
                coinCollider.CompareTag("Iridium") || coinCollider.CompareTag("Gold") ||
                coinCollider.CompareTag("Cobalt") || coinCollider.CompareTag("Nickel"))
            {
                // Gradually move the coin towards the player
                coinCollider.transform.position = Vector3.MoveTowards(
                    coinCollider.transform.position,
                    transform.position,
                    coinAttractionSpeed * Time.deltaTime
                );
            }
        }
    }

    // Function to dash the player forward
    void Dash(Vector3 direction)
    {
        AnimDirection(direction);
        animator.SetTrigger("isDashing");

        //transform.position += direction * dashDistance;
        float forceMagnitude = 20f;

        StartCoroutine(forceBurst(forceMagnitude, direction, 0.25f));
        StartCoroutine(DashCooldown());
    }

    // Dash cooldown coroutine
    IEnumerator DashCooldown()
    {
        isDashOnCooldown = true;                // Set dash on cooldown
        yield return new WaitForSeconds(dashCooldown);  // Wait for the cooldown duration
        isDashOnCooldown = false;               // Dash is no longer on cooldown
    }

    IEnumerator forceBurst(float magnitude, Vector3 direction, float duration)
    {
        rigidBody.mass = 0f;
        rigidBody.AddForce(direction * magnitude, ForceMode2D.Impulse);
        rigidBody.velocity = direction * magnitude;
        health.isInvincible = true;
        yield return new WaitForSeconds(duration);
        health.isInvincible = false;
        rigidBody.velocity = Vector2.zero;
        rigidBody.mass = 1.0f;
    }
    // Speed boost coroutine
    IEnumerator SpeedBoost()
    {
        isBoosting = true;
        animator.SetBool("isSpeedBoosting", true);
        moveSpeed *= speedBoostMultiplier;

        StartCoroutine(ZoomCamera(zoomOutSize, zoomDuration));

        yield return new WaitForSeconds(boostDuration);  // Boost lasts for specified duration

        StartCoroutine(ZoomCamera(zoomInSize, zoomDuration));

        moveSpeed /= speedBoostMultiplier;  // Revert to normal speed
        isBoosting = false;
        animator.SetBool("isSpeedBoosting", false);

        StartCoroutine(SpeedBoostCooldown());  // Start cooldown coroutine for speed boost
    }

    IEnumerator ZoomCamera(float targetSize, float duration)
    {
        float startSize = vcam.m_Lens.OrthographicSize;  // Use Cinemachine lens size
        float time = 0;

        while (time < duration)
        {
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        vcam.m_Lens.OrthographicSize = targetSize;
    }

    // Speed boost cooldown coroutine
    IEnumerator SpeedBoostCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(boostCooldown);  // Cooldown duration
        isOnCooldown = false;
    }

    // Boost availability coroutine
    IEnumerator BoostAvailability()
    {
        canUseBoost = true;
        Debug.Log("Speed Boost activated! You have 30 seconds to use it.");

        yield return new WaitForSeconds(boostAvailabilityDuration);

        canUseBoost = false;
        Debug.Log("Speed Boost availability ended.");
    }

    // Dash availability coroutine
    IEnumerator DashAvailability()
    {
        canUseDash = true;
        Debug.Log("Dash activated! You have 30 seconds to use it.");

        yield return new WaitForSeconds(dashAvailabilityDuration);

        canUseDash = false;
        Debug.Log("Dash Availability Ended.");
    }

    // --- Push-Back Availability Coroutine ---
    IEnumerator PushBackAvailability()
    {
        canUsePushBack = true;
        Debug.Log("Push-Back activated! You have 30 seconds to use it.");

        yield return new WaitForSeconds(pushBackAvailabilityDuration);

        canUsePushBack = false;
        Debug.Log("Push-Back availability ended.");
    }

    // --- Push-Back Cooldown Coroutine ---
    IEnumerator PushBackCooldown()
    {
        isPushBackOnCooldown = true;
        yield return new WaitForSeconds(pushBackCooldown);  // Cooldown duration
        isPushBackOnCooldown = false;
    }

    IEnumerator FireRateAvailability()
    {
        autoShooterScript.fireRate = 5f;
        yield return new WaitForSeconds(fireRateBoostDuration);
        autoShooterScript.fireRate = 2.5f;
    }


    // Function to activate push-back
    void ActivatePushBack()
    {
        //Debug.Log("ActivatePushBack called");
        Instantiate(pushBackPrefab, transform.position, Quaternion.identity);
        // Find all colliders within the pushBackRadius
        int layerMask = LayerMask.GetMask("Ignore Raycast");
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, pushBackRadius, layerMask);
        //Debug.Log($"Found {hitColliders.Length} colliders within radius");

        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("basic_enemy") ||
                collider.CompareTag("dash_enemy") ||
                collider.CompareTag("shoot_enemy"))
            {
                Rigidbody2D enemyRb = collider.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    Vector2 direction = (collider.transform.position - transform.position).normalized;
                    enemyRb.AddForce(direction * pushBackForce);
                }
                else
                {
                    Debug.LogWarning($"Enemy {collider.name} does not have a Rigidbody2D component.");
                }
            }
        }

        StartCoroutine(PushBackCooldown());
    }

    void ShowIridiumPopup()
    {
        popupManager.ShowPopup("Iridium, a rare platinum-group metal with atomic number 77, has fascinating connections to the Psyche asteroid (16 Psyche). This dense, highly corrosion-resistant element is one of the rarest in Earth's crust, but scientists believe it may be abundant in metallic asteroids like Psyche. The asteroid, located in the main asteroid belt between Mars and Jupiter, is thought to be the exposed metallic core of a protoplanet that lost its outer layers through violent collisions during the early solar system. Surprise Note:  Discovery of an iridium-rich clay layer in rocks in Gubbio, Italy, produced at the end of the Cretaceous period, indicated a giant asteroid strike wiped out the dinosaurs 65 million years ago! NASA's upcoming Psyche mission aims to study this unique asteroid, which could contain significant amounts of iron and nickel along with precious metals like iridium. ", iridiumSprite);
    }

    void ShowGoldPopup()
    {
        popupManager.ShowPopup("Gold, atomic number 79, shares an intriguing potential connection with the Psyche asteroid (16 Psyche). As one of humanity's most valued precious metals, gold on Earth is relatively rare, having been concentrated in the planet's core during its formation. The Psyche asteroid, believed to be the exposed core of a protoplanet, presents a unique opportunity to study how gold and other precious metals are distributed in planetary cores.\n\r\nScientists estimate that if Psyche is similar in composition to Earth's core, it could contain substantial amounts of gold along with its primary components of iron and nickel. This connection between gold and Psyche highlights the fascinating intersection of planetary science, metallurgy, and the early history of our solar system.", goldSprite);
    }

    void ShowNickelPopup()
    {
        popupManager.ShowPopup("Nickel, atomic number 28, plays a crucial role in our understanding of the Psyche asteroid (16 Psyche). This silvery-white metal is one of the most abundant elements in metallic meteorites and is expected to be a major component of Psyche's composition. Scientists believe that like Earth's core, Psyche contains significant amounts of nickel combined with iron, forming an iron-nickel alloy similar to what we find in meteorites.\n\r\nThe NASA Psyche mission's study of this asteroid could reveal how nickel and other metals were distributed during planetary formation, potentially providing insights into the composition of Earth's own core.", nickelSprite);
    }

    void ShowIronPopup()
    {
        popupManager.ShowPopup("Iron, atomic number 26, is believed to be the primary component of the Psyche asteroid (16 Psyche), making it particularly significant for scientific study. As the most abundant element by mass in the Earth's core, iron's presence in Psyche could provide crucial insights into planetary core formation. Scientists estimate that Psyche might contain enough iron to fill several million cubic kilometers, potentially making it the exposed iron core of an early planetesimal.\n\r\nThe NASA Psyche mission's investigation of this iron-rich body could help us understand how planetary cores form and evolve, offering a unique window into processes we cannot directly observe on Earth.", ironSprite);
    }

    void ShowCobaltPopup()
    {
        popupManager.ShowPopup("Cobalt, atomic number 27, represents an important element in the study of the Psyche asteroid (16 Psyche). This magnetic metal often occurs naturally alongside iron and nickel in meteorites, and scientists believe it could be present in significant quantities within Psyche's interior. As a transition metal commonly found in planetary cores, cobalt's presence and distribution within Psyche could provide valuable clues about the formation of planetary bodies.\n\r\nThe NASA Psyche mission's investigation of this metallic asteroid could reveal new insights about how cobalt and other metals were distributed during the early solar system's formation, potentially improving our understanding of planetary core composition.", cobaltSprite);
    }

    void ShowAllStatUpPopup()
    {
        popupManager.ShowPopup("All Stats Up! Your abilities have been boosted across the board. Just like a supernova injecting energy into the cosmos, you're now a force to be reckoned with.", allStatUpSprite);
    }

    void ShowDamagePopup()
    {
        popupManager.ShowPopup("Increased Damage! Tap into the destructive power of cosmic rays--your attacks now hit harder than ever.", damageSprite);
    }

    void ShowDashPopup()
    {
        popupManager.ShowPopup("Evade Unlocked! Use it by pressing either Shift button. Useful for getting out of sticky situations. 2 second cooldown in-between uses.", dashSprite);
    }

    void ShowFullHealthPopup()
    {
        popupManager.ShowPopup("Full Health Restored! Like a star reborn in a nebula, you're back at full strength. Use it wisely.", fullHealthSprite);
    }

    void ShowFireRatePopup()
    {
        popupManager.ShowPopup("Fire Rate Boosted! Your weapon hums with rapid-fire energy, channeling the pulse of a quasar with every shot.", fireRateSprite);
    }

    void ShowHealthUpPopup()
    {
        popupManager.ShowPopup("Health Up! Your endurance is now bolstered, echoing the resilience of dense asteroid cores under pressure.", healthUpSprite);
    }

    void ShowPushbackPopup()
    {
        popupManager.ShowPopup("Pushback Acquired! For the next 30 seconds, by pressing P, enemies around you will be pushed back. Use it if you are in a pickle! 3 second cooldown in-between uses.", PushbackSprite);
    }

    void ShowRocketPopup()
    {
        popupManager.ShowPopup("Rocket Equipped! For the next 30 seconds, if you press the spacebar while moving in a direction it will boost your speed. 2 second cooldown in-between uses.", RocketSprite);
    }

    private void fillDict()
    {
        popupDict["Iridium"] = () => ShowIridiumPopup();
        popupDict["Gold"] = () => ShowGoldPopup();
        popupDict["Cobalt"] = () => ShowCobaltPopup();
        popupDict["Iron"] = () => ShowIronPopup();
        popupDict["Nickel"] = () => ShowNickelPopup();
        popupDict["Rocket"] = () => ShowRocketPopup();
        popupDict["Dash"] = () => ShowDashPopup();
        popupDict["PushBack"] = () => ShowPushbackPopup();
        popupDict["Damage"] = () => ShowDamagePopup();
        popupDict["FireRate"] = () => ShowFireRatePopup();
        popupDict["Health"] = () => ShowFullHealthPopup();
        popupDict["HealthUp"] = () => ShowHealthUpPopup();
        popupDict["AllStatsUp"] = () => ShowAllStatUpPopup();
    }
    // Handle collisions between trigger objects and player object
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("basic_enemy") || other.gameObject.CompareTag("enemy_projectile"))
        {
            health.DamagePlayer(10 + damageFactor);
        }
        else if (other.gameObject.CompareTag("Iridium"))
        {
            metalCollected pickup = other.GetComponent<metalCollected>();
            if (pickup == null || pickup.isCollected)
                return;

            pickup.isCollected = true;

            CircleCollider2D collider = GetComponent<CircleCollider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            if (!firstIridiumCollected)
            {
                popupDict["Iridium"]?.Invoke();
                firstIridiumCollected = true;
                metalLog.Add("Iridium");
                resetMetalIndex();
            }
            audioSource.PlayOneShot(itemSound);
            numIridium++;
            updateMetals();
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Gold"))
        {
            metalCollected pickup = other.GetComponent<metalCollected>();
            if (pickup == null || pickup.isCollected)
                return;

            pickup.isCollected = true;

            CircleCollider2D collider = GetComponent<CircleCollider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            if (!firstGoldCollected)
            {
                popupDict["Gold"]?.Invoke();
                firstGoldCollected = true;
                metalLog.Add("Gold");
                resetMetalIndex();
            }
            audioSource.PlayOneShot(itemSound);
            numGold++;
            updateMetals();
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Nickel"))
        {
            metalCollected pickup = other.GetComponent<metalCollected>();
            if (pickup == null || pickup.isCollected)
                return;

            pickup.isCollected = true;

            CircleCollider2D collider = GetComponent<CircleCollider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            if (!firstNickelCollected)
            {
                popupDict["Nickel"]?.Invoke();
                firstNickelCollected = true;
                metalLog.Add("Nickel");
                resetMetalIndex(); 
            }
            audioSource.PlayOneShot(itemSound);
            numNickel++;
            updateMetals();
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Iron"))
        {
            metalCollected pickup = other.GetComponent<metalCollected>();
            if (pickup == null || pickup.isCollected)
                return;

            pickup.isCollected = true;

            CircleCollider2D collider = GetComponent<CircleCollider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            if (!firstIronCollected)
            {
                popupDict["Iron"]?.Invoke();
                firstIronCollected = true;
                metalLog.Add("Iron");
                resetMetalIndex();
            }
            audioSource.PlayOneShot(itemSound);
            numIron++;
            updateMetals();
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Cobalt"))
        {
            metalCollected pickup = other.GetComponent<metalCollected>();
            if (pickup == null || pickup.isCollected)
                return;

            pickup.isCollected = true;

            CircleCollider2D collider = GetComponent<CircleCollider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            if (!firstColbaltCollected)
            {
                popupDict["Cobalt"]?.Invoke();
                firstColbaltCollected = true;
                metalLog.Add("Cobalt");
                resetMetalIndex();
                
            }
            audioSource.PlayOneShot(itemSound);
            numCobalt++;
            updateMetals();
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Rocket"))
        {
            audioSource.PlayOneShot(powerUpSound);
            if (!firstRocketCollected)
            {
                popupDict["Rocket"]?.Invoke(); //shows the rocket popup
                firstRocketCollected = true;
                metalLog.Add("Rocket");
                resetMetalIndex();
            }
            StartCoroutine(BoostAvailability());
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Dash"))
        {
            audioSource.PlayOneShot(powerUpSound);
            if (!firstDashCollected)
            {
                popupDict["Dash"]?.Invoke(); //shows the dash popup
                firstDashCollected = true;
                metalLog.Add("Dash");
                resetMetalIndex();
            }
            StartCoroutine(DashAvailability());
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("PushBack"))
        {
            audioSource.PlayOneShot(powerUpSound);
            if (!firstPushbackCollected)
            {
                popupDict["PushBack"]?.Invoke(); //shows the pushback popup
                firstPushbackCollected = true;
                metalLog.Add("PushBack");
                resetMetalIndex();
            }
            StartCoroutine(PushBackAvailability());
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("FireRate"))
        {
            audioSource.PlayOneShot(powerUpSound);

            if (!firstFireRateCollected)
            {
                popupDict["FireRate"]?.Invoke(); //shows the firerate popup
                firstFireRateCollected = true;
                powerupLog.Add("FireRate");
                resetMetalIndex();
            }
            autoShooterScript.fireRate += 2f;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("damage"))
        {
            audioSource.PlayOneShot(powerUpSound);
            if (!firstDamageCollected)
            {
                popupDict["Damage"]?.Invoke(); //shows the damage popup
                firstDamageCollected = true;
                metalLog.Add("Damage");
                resetMetalIndex();
            }
            Projectile.defaultDamageAmount += 25f;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("health"))
        {
            audioSource.PlayOneShot(powerUpSound);
            if (!firstFullHealthCollected)
            {
                popupDict["Health"]?.Invoke(); //shows the fullhealth popup
                firstFullHealthCollected = true;
                metalLog.Add("Health");
                resetMetalIndex();
            }
            health.healPlayerToFull();
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("healthUp"))
        {
            audioSource.PlayOneShot(powerUpSound);
            if (!firstHealthUpCollected)
            {
                popupDict["HealthUp"]?.Invoke(); //shows the healthup popup
                firstHealthUpCollected = true;
                metalLog.Add("HealthUp");
                resetMetalIndex();
            }
            health.maxHealth += 50;
            health.curHealth += 50;

            health.invincibilityTime += .25f;
            health.healthBar.SetHealth(health.curHealth);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("AllStatsUp"))
        {
            audioSource.PlayOneShot(powerUpSound);
            if (!firstAllStatUpCollected)
            {
                popupDict["AllStatsUp"]?.Invoke(); //shows the fullhealth popup
                firstAllStatUpCollected = true;
                metalLog.Add("AllStatsUp");
                resetMetalIndex();
            }
            //Damage
            Projectile.defaultDamageAmount += 25f;
            //Health
            health.maxHealth += 50;
            health.curHealth += 50;
            health.invincibilityTime += .25f;
            //FireRate
            autoShooterScript.fireRate += 2f;

            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("spacecraft_part"))
        {
            partsCollected++;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Slow"))
        {

            moveSpeed = moveSpeed * (1.0f); //disabling slow zones
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Slow"))
        {
            moveSpeed = originalMoveSpeed;
        }
    }
    /**
     * Need this function for animation
     */
    void AnimDirection(Vector3 move)
    {
        if (move.sqrMagnitude > 0)
        {
            animator.SetFloat("x", move.x);
            animator.SetFloat("y", move.y);
        }
    }

    // Rotate the sprite based on movement direction
    void RotateSprite(Vector3 movement)
    {
        if (movement != Vector3.zero) // Only rotate if there's movement
        {
            lastNonZeroMovement = movement; // Update the last non-zero movement
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg; // Calculate angle
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 180)); // Rotate sprite (offset to face "north")
        }
    }

    // draws pushback radius for visual indication
    private void OnDrawGizmosSelected()
    {

        // Draw Push-Back Radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pushBackRadius);

    }

    private void updateMetals()
    {
        metalText.text = 
            "Iridium: " + numIridium + "\n" +
            "Gold: " + numGold + "\n" +
            "Cobalt: " + numCobalt + "\n" +
            "Iron: " + numIron + "\n" +
            "Nickel: " + numNickel + "\n" +
            "Score: " + ((numIridium) + (numGold*2) + (numCobalt*4) + (numIron*6) + (numNickel*8));
    }

    private void printMetals()
    {
        Debug.Log("Iridium: " + numIridium + "\n" +
            "Gold: " + numGold + "\n" +
            "Cobalt: " + numCobalt + "\n" +
            "Iron: " + numIron + "\n" +
            "Nickel: " + numNickel);
    }

    // damage power up
    public void giveDamage()
    {
        Projectile.defaultDamageAmount += 25f;
        Debug.Log(Projectile.defaultDamageAmount);
        hideMenu();
        if (!firstDamageCollected)
        {
            popupDict["Damage"]?.Invoke(); //shows the damage popup
            metalLog.Add("Damage");
            resetMetalIndex();
            firstDamageCollected = true;
        }
    }
    // fire rate power up
    public void giveFirerate()
    {
        autoShooterScript.fireRate += 2f;
        Debug.Log(autoShooterScript.fireRate);
        hideMenu();
        if (!firstFireRateCollected)
        {
            popupDict["FireRate"]?.Invoke(); //shows the firerate popup
            metalLog.Add("FireRate");
            resetMetalIndex();
            firstFireRateCollected = true;
        }
    }
    // health up power up
    public void giveHealthUp()
    {
        health.maxHealth += 50;
        health.curHealth += 50;
        health.invincibilityTime += .25f;
        health.healthBar.SetHealth(health.curHealth);
        Debug.Log(health.maxHealth);
        hideMenu();
        if (!firstHealthUpCollected)
        {
            popupDict["HealthUp"]?.Invoke(); //shows the HEalthup popup
            metalLog.Add("HealthUp");
            resetMetalIndex();
            firstHealthUpCollected = true;
        }
    }

    private void resetMetalIndex()
    {
        indexInList = metalLog.Count - 1;
        metalLogCount.text = "" + metalLog.Count + " / 13";
    }

    private void hideMenu()
    {
        Time.timeScale = 1f;
        UpgradeMenu.SetActive(false);
    }

    public void showNewest()
    {
        if (metalLog.Count > 0)
        {
            popupDict[metalLog[^1]]();
        }
    }

    public void cycleNext()
    {
        if (indexInList + 1 < metalLog.Count)
        {
            popupDict[metalLog[indexInList + 1]]();
            indexInList++;
        }
    }

    public void cyclePrev()
    {
        if (indexInList - 1 >= 0)
        {
            popupDict[metalLog[indexInList - 1]]();
            indexInList--;
        }
    }
}