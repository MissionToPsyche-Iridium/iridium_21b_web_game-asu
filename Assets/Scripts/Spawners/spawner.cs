using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    public GameObject Player;
    public GameObject asteroid;
    public GameObject areaOfEffect;
    public GameObject areaOfEffectRand;
    public GameObject nodeMapCenter;
    public GameObject quadrant1;
    public GameObject quadrant2;
    public GameObject quadrant3;
    public GameObject quadrant4;
    public GameObject currentArea = null;
    public float height = 20f;
    private SpriteRenderer sprite;
    private float timer = 0;
    private float randTimer = 0;
    private float spawnrate = 1;
    private float randSpawnrate = .2f;
    private float timeBetweenChange = 30f;
    private float randX;
    private float randY;
    private bool isWaiting = false;
    // Start is called before the first frame update
    void Start()
    { 
        Player = GameObject.FindGameObjectWithTag("PlayerTag");
        nodeMapCenter = GameObject.FindGameObjectWithTag("node_map");
        quadrant1 = GameObject.FindGameObjectWithTag("quad1");
        quadrant2 = GameObject.FindGameObjectWithTag("quad2");
        quadrant3 = GameObject.FindGameObjectWithTag("quad3");
        quadrant4 = GameObject.FindGameObjectWithTag("quad4");
        sprite = GetComponent<SpriteRenderer>();
        sprite.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

        //Countdown until next area
        if (!isWaiting)
        {
            isWaiting = true;
            changeLocation();
            if (currentArea != null)
            {
                transform.position = currentArea.transform.position;
            }
            StartCoroutine(waitForNewLocation());
        }

        //If there is an area, spawn meteors
        if (currentArea != null)
        {
            randomSpawner();
        }
        
    }

    void randomSpawner()
    {
        if (randTimer < randSpawnrate)
        {
            randTimer += Time.deltaTime;
        }
        else
        {
            randX = (float)Random.Range(-16, 16) + transform.position.x;
            randY = (float)Random.Range(0, 32) + transform.position.y;

            if (asteroid != null)
            {
                Instantiate(asteroid, new Vector3(randX, randY, 1), transform.rotation);
            }
            randTimer = 0;
        }

    }

    void changeLocation()
    {
        float location = UnityEngine.Random.Range(1, 11);
        if (location == 1)
        {
            setLocation(nodeMapCenter);
        }
        else if (location == 2)
        {
            setLocation(quadrant1);
        }
        else if (location == 3)
        {
            setLocation(quadrant2);
        }
        else if (location == 4)
        {
            setLocation(quadrant3);
        }   
        else if (location == 5)
        {
            setLocation(quadrant4);
        }
        else
        {
            currentArea = null;
            sprite.enabled = false;
        }
    }

    void setLocation(GameObject location)
    {
        currentArea = location;
        sprite.enabled = true;
    }

    IEnumerator waitForNewLocation()
    {
        float elapsed = 0f;
        while (elapsed < timeBetweenChange)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        isWaiting = false;
    }
}
