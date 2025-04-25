using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public GameObject damageUpgrade;
    public GameObject firerateUpgrade;
    public GameObject healthUpgrade;
    public static int numMetals = 1;
    private List<GameObject> currentMetals;
    private List<GameObject> upgrades = new List<GameObject>();
    private float newXScale;
    private Vector3 healthBar_Scale;
    private Vector3 healthBar_Pos;
    private IEnemyDeathHandler deathHandler;


    private void Awake() {
        deathHandler = GetComponent<IEnemyDeathHandler>();
    }
    // Start is called before the first frame update
    void Start()
    {
        maxHealth = maxHealth * healthScale;
        currentHealth = currentHealth * healthScale;
        healthBar_Scale = healthBar.transform.localScale;
        healthBar_Pos = healthBar.transform.localPosition;

        currentMetals = new List<GameObject> { iridiumDrop, goldDrop,  cobaltDrop, ironDrop, nickelDrop };
        upgrades = new List<GameObject> { damageUpgrade, firerateUpgrade, healthUpgrade };

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
            deathHandler?.OnDeath();
            StartCoroutine(Die(.36f));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator Die(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
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
        float drop = UnityEngine.Random.Range(1, 121);
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
        else if (drop == 6) //Damage / Firerate / HealthUp
        {
            //random upgrade drop
            randomDrops(upgrades, 3);
        }
        else
        {
            //random metal drop by default
            randomDrops(currentMetals, numMetals);
        }
    }

    void randomDrops(List<GameObject> options, int upper)
    {
        Debug.Log("numMetals: " + numMetals);
        float drop = UnityEngine.Random.Range(0, upper);
        Debug.Log("drop float: " + drop);
        Instantiate(options[Convert.ToInt32(drop)], transform.position, Quaternion.identity);
    }
}