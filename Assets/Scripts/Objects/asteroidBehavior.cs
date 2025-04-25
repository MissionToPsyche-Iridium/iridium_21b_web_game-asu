using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class asteroidBehavior : MonoBehaviour
{
    public float startingYPos;
    public float dropSpeed = 5.5f;
    public float destroyY;
    public float destroyX;
    public float hitRadius = 0.5f;
    public float despawnDelay = .72f;   // need to make it line up with the frames of the animation
    public GameObject Player;
    public Animator animator;

    private bool isDespawn = false;
    private Health playerHealth;
    public AudioClip deathClip;
    private AudioSource audioSource;



    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Player = GameObject.FindGameObjectWithTag("PlayerTag");
        playerHealth = GameObject.FindGameObjectWithTag("PlayerTag").GetComponent<Health>();
        destroyY = transform.position.y - 15f;//Player.transform.position.y;
        destroyX = Player.transform.position.x;
    }


    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3.down) * dropSpeed * Time.deltaTime;
        if (transform.position.y <= destroyY)
        {
            dropSpeed = 0f;
            //Destroy(gameObject);
            DespawnObject();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerTag" && 
           (this.inRange(collision.gameObject.transform.position.y, destroyY - hitRadius, destroyY + hitRadius)))
        {
            //If the GameObject has the same tag as specified, output this message in the console
            playerHealth.DamagePlayer(20);
            Debug.Log("Player Hit!");
        }
    }

    bool inRange(float location, float min, float max)
    {
        if (min <= location && max >= location)
        {
            return true;
        }
        return false;
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

        audioSource.PlayOneShot(deathClip);

        // Wait for the animation duration (adjust based on your actual animation length)
        yield return new WaitForSeconds(despawnDelay);
        // disable renderers on death
        GetComponent<SpriteRenderer>().enabled = false;

        // disable all child renderers
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) {
            sr.enabled = false;
        }

        // Disable hitboxes on death
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>()) {
            col.enabled = false;
        }
        // Destroy the object after the animation is done
        Destroy(gameObject);
        isDespawn = false;
    }

}
