using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basic_enemy_behavior : MonoBehaviour
{
    public GameObject Player;
    public bool hasTarget = false;
    private float lastSeenX;
    private float lastSeenY;
    private float lastEnemyX;
    private float lastEnemyY;
    private float speed = 2.0f;
    private GameObject[] enemyObjects;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("PlayerTag");
    }

    // Update is called once per frame
    void Update()
    {

        enemyObjects = GameObject.FindGameObjectsWithTag("basic_enemy");
        Debug.Log(enemyObjects.Length);

        float step = speed * Time.deltaTime;
        //Move towards player as long as there is line of sight
        if (hasLineOfSight() )
        {
            hasTarget = true;
            speed = 2.0f;
            transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, step);
        }
        //Once line of sight breaks move towards where it last was
        else
        {
            hasTarget = false;
            speed = 2.5f;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(lastSeenX, lastSeenY, 0), step);
        }
        //No clue if this works
        if (!isMoving(transform.position.x, transform.position.y))
        {
            GameObject nearest = findNearestEnemy();
            transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, step);
        }
    }

    bool hasLineOfSight()
    {
        bool ret = false;
        RaycastHit2D hit = Physics2D.Linecast(transform.position, Player.transform.position, 1 << LayerMask.NameToLayer("Default"));

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.CompareTag("PlayerTag"))
            {
                Debug.DrawLine(transform.position, Player.transform.position, Color.red);
                lastSeenX = Player.transform.position.x;
                lastSeenY = Player.transform.position.y;
                ret = true;
            }
            else
            {
                Debug.DrawLine(transform.position, Player.transform.position, Color.blue);
            }
            Debug.Log(hit.collider.gameObject);
            
        }

        return ret;
    }

    GameObject findNearestEnemy()
    {
        GameObject nearest = null;
        float dist = 100000f;
        for (int i = 0; i < enemyObjects.Length; i++)
        {
            if (Vector3.Distance(transform.position,  enemyObjects[i].transform.position) < dist &&
                enemyObjects[i].GetComponent<basic_enemy_behavior>().hasTarget)
            {
                hasTarget = true;
                nearest = enemyObjects[i];
            }
        }
        return nearest;
    }

    bool isMoving(float x, float y)
    {
        if (x == lastEnemyX && y == lastEnemyY) 
            return false;
        else
        {
            lastEnemyX = x; 
            lastEnemyY = y;
            return true;
        }
    }
}

