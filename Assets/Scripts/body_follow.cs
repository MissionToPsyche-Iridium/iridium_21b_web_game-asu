using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class body_follow : MonoBehaviour
{
    public float speed = 7.0f;
    public float speedFactor = 1.0f;
    public float healthScale = 1.0f;
    public float delay = 0f;
    public float followDistance = .01f; // Distance to maintain between segments
    public Transform target;
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
    private Queue<Vector3> positionHistory = new Queue<Vector3>(); // Stores past positions
    private bool ready = false;
    private bool hasNewBehavior = false;
    void Start()
    {
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
                Vector3 direction = (targetPosition - transform.position).normalized;
                float distance = Vector3.Distance(transform.position, targetPosition);
                if (distance > followDistance)
                {
                    // Dynamically adjust speed based on distance
                    float dynamicSpeed = Mathf.Lerp(speed * speedFactor, speed * speedFactor * 2, (distance - followDistance) / followDistance);
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition - direction * followDistance, dynamicSpeed * 2 * Time.deltaTime);
                }
            }
        } else if (target == null && !hasNewBehavior)
        {
            //Giving basic behavior
            basic_enemy_behavior movScript = gameObject.AddComponent<basic_enemy_behavior>();

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
        }
    }

    IEnumerator Wait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); // Ensure proper delay before movement starts
        ready = true;
    }

    void InitializeScript(EnemyHealth healthScript, Transform healthBar)
    {
        healthScript.currentHealth = 1500;
        healthScript.maxHealth = 1500;
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
        healthScript.AllStatIncrease = AllStatIncrease;
    }
}