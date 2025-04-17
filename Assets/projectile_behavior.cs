using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile_behavior : MonoBehaviour
{
    public GameObject Player;
    private Vector3 dest;
    private float speed = 9.0f;
    public Animator animator;
    Vector3 lastPosition;
    // Start is called before the first frame update

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("PlayerTag");
        dest = Player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float step = speed * Time.deltaTime;
        if (transform.position == dest)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, dest, step);
        }

        Vector3 move = (transform.position - lastPosition).normalized;

        // Pass the movement to directionAnim() to update animation
        directionAnim(move);

        // Update last position for the next frame
        lastPosition = transform.position;
    }
    void directionAnim(Vector3 move) {
        if (move.sqrMagnitude > 0) {
            animator.SetFloat("x", move.x);
            animator.SetFloat("y", move.y);
        }
    }
}
