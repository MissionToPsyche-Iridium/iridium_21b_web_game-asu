using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWall : MonoBehaviour
{
    public bool isOpen;
    public GameObject wallLeft;
    public GameObject wallRight;
    public float moveTime = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D (Collider2D other) 
    {
        if (other.gameObject.CompareTag("PlayerTag") && !isOpen)
        {
            StartCoroutine(MoveObjectLeft(this.wallLeft));
            StartCoroutine(MoveObjectRight(this.wallRight));
            this.isOpen = true;
        }
    }

    IEnumerator MoveObjectLeft(GameObject obj)
    {

        Vector3 startPos = obj.transform.position; // Store initial position
        Vector3 targetPos = new Vector3(startPos.x - 1, startPos.y, 0);
        float elapsedTime = 0f; // Keep track of time passed

        while (elapsedTime < moveTime)
        {
            elapsedTime += Time.deltaTime; // Update elapsed time
            float progress = elapsedTime / moveTime; // Calculate progress
            obj.transform.position = Vector3.Lerp(startPos, targetPos, progress); // Lerp between start and target
            yield return null;
        }
        // Once the movement is complete, set the final position if needed

        obj.transform.position = targetPos;
    }

    IEnumerator MoveObjectRight(GameObject obj)
    {

        Vector3 startPos = obj.transform.position; // Store initial position
        Vector3 targetPos = new Vector3(startPos.x + 1, startPos.y, 0);
        float elapsedTime = 0f; // Keep track of time passed

        while (elapsedTime < moveTime)
        {
            elapsedTime += Time.deltaTime; // Update elapsed time
            float progress = elapsedTime / moveTime; // Calculate progress
            obj.transform.position = Vector3.Lerp(startPos, targetPos, progress); // Lerp between start and target
            yield return null;
        }
        // Once the movement is complete, set the final position if needed

        obj.transform.position = targetPos;
    }

}
