using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class asteroidBehavior : MonoBehaviour
{
    public float startingYPos;
    public float dropSpeed = 5.5f;
    public float destroyHeight;
    public float despawnDelay = .72f;   // need to make it line up with the frames of the animation
    public GameObject Player;
    public Animator animator;

    private bool isDespawn = false;
    

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("PlayerTag");
        destroyHeight = Player.transform.position.y;
    }


    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3.down) * dropSpeed * Time.deltaTime;
        if (transform.position.y <= destroyHeight)
        {
            dropSpeed = 0f;
            //Destroy(gameObject);
            DespawnObject();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerTag")
        {
            //If the GameObject has the same tag as specified, output this message in the console
            Debug.Log("Player Hit!");
        }
    }

    public void DespawnObject() {
        if (!isDespawn) {
            isDespawn = true;
            // Set the "Despawn" trigger in the Animator to start the despawn animation
            animator.SetTrigger("DespawnTrigger");

            // Wait for the animation to finish before destroying the object
            StartCoroutine(WaitForDespawn());
        }
    }
    private IEnumerator WaitForDespawn() {
        // Wait for the animation duration (adjust based on your actual animation length)
        yield return new WaitForSeconds(despawnDelay);

        // Destroy the object after the animation is done
        Destroy(gameObject);
        isDespawn = false;
    }
}
