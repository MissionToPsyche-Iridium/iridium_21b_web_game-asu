using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Created by Robert DeLucia Jr. during Sprint 1
public class PlayerMovement : MonoBehaviour
{
    public CoinManager cm;
    public float moveSpeed = 5f;

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveY = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        Vector3 movement = new Vector3(moveX, moveY, 0);
        transform.position += movement;
    }
    void OnTriggerEnter2D(Collider2D other){
      if(other.gameObject.CompareTag("Coin")){
        cm.coinCount++;
        Destroy(other.gameObject);
      }
    }

}
