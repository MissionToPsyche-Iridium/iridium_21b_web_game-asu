using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject Coin; // Reference to the coin prefab
    public float xMin, xMax, yMin, yMax, zMin, zMax; // Define spawn area
    public int numberOfCoins; // How many coins to spawn

    // Start is called before the first frame update
    void Start()
    {
        SpawnCoins();
    }

    void SpawnCoins()
    {
        for (int i = 0; i < numberOfCoins; i++)
        {
            // Generate random spawn position
            Vector3 randomPosition = new Vector3(
                Random.Range(xMin, xMax),
                Random.Range(yMin, yMax),
                Random.Range(zMin, zMax)
            );

            // Instantiate the coin at the random position
            Instantiate(Coin, randomPosition, Quaternion.identity);
        }
    }
}
