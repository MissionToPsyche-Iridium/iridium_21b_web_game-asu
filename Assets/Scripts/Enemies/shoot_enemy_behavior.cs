using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shoot_enemy_behavior : MonoBehaviour
{
    public GameObject Player;
    public GameObject Projectile;
    private float speed = 2.0f;
    private float lastSeenX;
    private float lastSeenY;
    private float lastEnemyX;
    private float lastEnemyY;
    private float time_between_shot = 0.8f;
    private bool shooting = false;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("PlayerTag");
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(transform.position, Player.transform.position);
        float step = speed * Time.deltaTime;
        //If distance from player = 6
        if (dist > 6)
        {
            if (hasLineOfSight())
            {
                transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, step);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(lastSeenX, lastSeenY, 0), step);
            }
        } 
        else if (!shooting)
        {
            //shoot
            shooting = true;
            Instantiate(Projectile, transform.position, Quaternion.identity);
            StartCoroutine(WaitAfterShot());
        }
    }

    IEnumerator WaitAfterShot()
    {
        yield return new WaitForSeconds(time_between_shot);
        shooting = false;
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
