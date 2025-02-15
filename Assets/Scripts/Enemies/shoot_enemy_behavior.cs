using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shoot_enemy_behavior : MonoBehaviour
{
    public GameObject Player;
    public GameObject Projectile;
    private float speed = 2.0f;
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
            transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, step);
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
}
