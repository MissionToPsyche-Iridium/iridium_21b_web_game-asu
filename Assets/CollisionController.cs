using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionController : MonoBehaviour
{
    // Start is called before the first frame update
    void OnCollisionEnter2D(Collision2D collision)
    {
    if (collision.gameObject.CompareTag("boulder"))
    {
        Debug.Log("Collision detected with Boulder!");
    }
  }


    // Update is called once per frame
    void Update()
    {

    }
}
