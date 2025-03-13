using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class body_follow : MonoBehaviour
{
    public float speed = 4.0f;
    public float delay = 0f;
    public float followDistance = .01f; // Distance to maintain between segments
    public Transform target;
    public GameObject collectible;
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
                    float dynamicSpeed = Mathf.Lerp(speed, speed * 2, (distance - followDistance) / followDistance);
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition - direction * followDistance, dynamicSpeed * Time.deltaTime);
                }
            }
        } else if (target == null && !hasNewBehavior)
        {
            //Giving basic behavior
            basic_enemy_behavior movScript = gameObject.AddComponent<basic_enemy_behavior>();

            //Allowing player damage
            EnemyHealth healthScript = gameObject.AddComponent<EnemyHealth>();
            Transform healthBar = transform.GetChild(0);
            healthScript.currentHealth = 1500;
            healthScript.maxHealth = 1500;
            healthScript.healthBar = healthBar.gameObject;
            healthScript.collectible = collectible;

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
}