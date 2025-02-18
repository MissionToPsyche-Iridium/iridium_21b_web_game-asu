using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dash_enemy_script : MonoBehaviour
{
    public GameObject Player;
    public float speed = 2.0f;
    private Vector3 dash_location;
    private float lastSeenX;
    private float lastSeenY;
    private float lastEnemyX;
    private float lastEnemyY;
    public float dashSpeed = 8.0f;
    public float waitTimeAfterDash = 3.0f; // Time to wait after dashing
    private bool isDashing = false;
    private bool isWaiting = false;
    
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("PlayerTag");
    }

    // Update is called once per frame
    void Update()
    {
        if (isWaiting) return; // Stop movement while waiting

        float step = speed * Time.deltaTime;
        float distance = Vector3.Distance(transform.position, Player.transform.position);

        // Check if enemy should dash
        if (!isDashing && distance <= 4.0f)
        {
            if (hasLineOfSight())
            {
                dash_location = Player.transform.position; // Capture the player's position for dashing
            }
            else
            {
                dash_location = new Vector3(lastSeenX, lastSeenY, 0);
            }
            isDashing = true;
        }

        // If dashing, move towards the captured location at dash speed
        if (isDashing)
        {
            transform.position = Vector3.MoveTowards(transform.position, dash_location, dashSpeed * Time.deltaTime);
            

            // Stop dashing when reaching the target
            if (Vector3.Distance(transform.position, dash_location) < 0.1f)
            {
                StartCoroutine(WaitAfterDash());
            }
        }
        else
        {
            // Regular movement towards the player
            transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, step);
        }
    }

    IEnumerator WaitAfterDash()
    {
        isDashing = false; // Stop dashing
        isWaiting = true;  // Prevent movement

        Debug.Log("Waiting for " + waitTimeAfterDash + " seconds...");
        yield return new WaitForSeconds(waitTimeAfterDash);

        Debug.Log("Resuming movement.");
        isWaiting = false; // Resume movement
    }

    bool hasLineOfSight()
    {
        bool ret = false;

        // Perform the Linecast and get all hits
        RaycastHit2D[] hits = Physics2D.LinecastAll(transform.position, Player.transform.position);

        foreach (RaycastHit2D hit in hits)
        {
            // Skip the enemy's own collider
            if (hit.collider.gameObject == this.gameObject)
            {
                continue;
            }

            // Check if the hit collider is the player
            if (hit.collider.gameObject.CompareTag("PlayerTag"))
            {
                Debug.DrawLine(transform.position, Player.transform.position, Color.red);
                lastSeenX = Player.transform.position.x;
                lastSeenY = Player.transform.position.y;
                ret = true;
                break;
            }
            else
            {
                // Line of sight is blocked by another object
                Debug.DrawLine(transform.position, Player.transform.position, Color.blue);
                break;
            }
        }

        return ret;
    }
}
