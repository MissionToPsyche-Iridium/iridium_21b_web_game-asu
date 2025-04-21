using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemy_spawner : MonoBehaviour
{
    //Entities
    public GameObject Player;
    public GameObject EnemyType1;
    public GameObject EnemyType2;
    public GameObject EnemyType3;
    public GameObject EnemyType4;

    //Spawnable Powerups
    public GameObject multispec;
    public GameObject gammaSpec;
    public GameObject magnetometer;
    public GameObject neutronSpec;
    public GameObject xBandRadio;
    public GameObject damagePU;
    public GameObject fireRatePU;
    public GameObject healthPU;
    public GameObject UpgradeMenu;
    public GameObject PartsPopup;

    //Sprites
    public Sprite multispecSprite;
    public Sprite gammaSpecSprite;
    public Sprite magnetometerSprite;
    public Sprite neutronSpecSprite;
    public Sprite xBandRadioSprite;

    //Labels
    private string multispecString = "Multi Spectral Imager";
    private string gammaString = "Gamma Ray Spectrometer";
    private string magnetometerString = "Magnetometer";
    private string neutronString = "Neutron Spectrometer";
    private string xBandRadioString = "X Band Radio";

    //Node Maps
    public GameObject centerMap;
    public GameObject quadrant1;
    public GameObject quadrant2;
    public GameObject quadrant3;
    public GameObject quadrant4;
    public Node[] nodeList = new Node[6]; //will dynamically set to the nearest nodemap / quadrant
    private GameObject nearestMap;

    //Spawn Logic
    private List<int> validSpawns = new List<int> { 0, 1, 2, 3, 4, 5 };
    private int currentInvalid = -1;
    private GameObject[] enemies;
    private float randTimer = 0;
    private float randSpawnrate = .5f;
    private float randX;
    private float randY;

    //Distance
    private float centerQuadDist;
    private float quad1Dist;
    private float quad2Dist;
    private float quad3Dist;
    private float quad4Dist;
    private GameObject[] maps = new GameObject[5];

    //Wave info
    private int numEnemies = 0;
    private int bossEnemies = 0;
    private int waveNumber = 1; // Start from wave 1
    private int maxWaveEnemies = 10;
    private int maxBossEnemies = 0;
    private bool waiting = false;
    private bool scaledEnemies = false;
    private Timer timer;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("PlayerTag");
        centerMap = GameObject.FindGameObjectWithTag("centerMap");
        timer = FindObjectOfType<Timer>();
        quadrant1 = GameObject.FindGameObjectWithTag("quad1");
        quadrant2 = GameObject.FindGameObjectWithTag("quad2");
        quadrant3 = GameObject.FindGameObjectWithTag("quad3");
        quadrant4 = GameObject.FindGameObjectWithTag("quad4");
        maps[0] = centerMap;
        maps[1] = quadrant1;
        maps[2] = quadrant2;
        maps[3] = quadrant3;
        maps[4] = quadrant4;
        establishNodes(centerMap); //initially setting the spawn points to the central nodemap

        if (UpgradeMenu != null)
        {
            UpgradeMenu.SetActive(false);
        }

        if (PartsPopup != null)
        {
            PartsPopup.SetActive(false);
        }

        Debug.Log("Entering Wave: " + waveNumber);
    }

    void Update()
    {

        //Check for end of wave
        if (waiting)
        {
            return;
        }
        else
        {
            randomSpawner();
        }

        //Check for new enemy scaling
        if (timer.getWaveTime() == 0 && !scaledEnemies)
        {
            Player.GetComponent<PlayerMovement>().damageFactor += 2;
            EnemyHealth.healthScale += .4f;
            basic_enemy_behavior.speedFactor += .1f;
            dash_enemy_script.speedFactor += .1f;
            shoot_enemy_behavior.speedFactor += .1f;
            head_behavior.speedFactor += .1f;
            body_follow.speedFactor += .1f;
            scaledEnemies = true;
        }
        /*
         -Find nearest map
         -Set valid spawns to the points on the nearest map
         -Exclude nearest node
        */
        getNearestNodeMap();
        establishNodes(nearestMap);
        findNearestSeenNode();
    }

    void randomSpawner()
    {
        // Update the enemies array
        enemies = GameObject.FindGameObjectsWithTag("basic_enemy");

        // Regular wave
        if (randTimer < randSpawnrate)
        {
            randTimer += Time.deltaTime;
        }
        else if (numEnemies < maxWaveEnemies)
        {
            spawnEnemy();
            if (waveNumber % 5 == 0 && bossEnemies < maxBossEnemies)
            {
                spawnBoss();
            }
            randTimer = 0;
        }


        // Move on to the next wave
        //Debug.Log(numEnemies + " " + maxWaveEnemies + " " + enemies.Length + " " + bossEnemies);
        if (numEnemies == maxWaveEnemies && enemies.Length - bossEnemies == 0)
        {
            numEnemies = 0;
            bossEnemies = 0;
            maxWaveEnemies += 5;
            waveNumber++;
            if (waveNumber % 5 == 0)
            {
                maxBossEnemies += 1;
                EnemyType4.GetComponentInChildren<head_behavior>().numSegments += 1;
            }

            checkpoint();
            Debug.Log("Entering Wave: " + waveNumber);
        }
    }


    void spawnEnemy()
    {
        getRandCoord();
        float randomEnemy = UnityEngine.Random.Range(0, calcEnemyRange() + 1);
        switch (randomEnemy)
        {
            case 0:
                Instantiate(EnemyType1, new Vector3(randX, randY, 1), transform.rotation);
                break;
            case 1:
                Instantiate(EnemyType2, new Vector3(randX, randY, 1), transform.rotation);
                break;
            case 2:
                Instantiate(EnemyType3, new Vector3(randX, randY, 1), transform.rotation);
                break;
            default:
                break;
        }
        numEnemies++;
    }

    void spawnBoss()
    {
        getRandCoord();
        Instantiate(EnemyType4, new Vector3(randX, randY, 1), transform.rotation);
        bossEnemies++;
    }

    void getRandCoord()
    {
        //Obtains one of the valid nodes at random to spawn the enemy
        int index = (int)UnityEngine.Random.Range(0, 5);
        randX = nodeList[validSpawns[index]].x;
        randY = nodeList[validSpawns[index]].y;
    }

    void establishNodes(GameObject map)
    {
        int i = 0;
        nearestMap = map;
        foreach (Transform node in nearestMap.transform)
        {
            nodeList[i] = new Node(node, 0, 0);
            i++;
        }
    }

    bool hasLineOfSight(Transform target, string tag)
    {
        bool ret = false;

        // Perform the Linecast and get all hits
        RaycastHit2D[] hits = Physics2D.LinecastAll(Player.transform.position, target.position);
        foreach (RaycastHit2D hit in hits)
        {
            // Skip the enemy's own collider
            if (hit.collider.gameObject == Player)
            {
                continue;
            }

            // Check if the hit collider is the player
            if (hit.collider.gameObject.CompareTag(tag))
            {
                Debug.DrawLine(Player.transform.position, target.position, Color.green);
                ret = true;
                break;
            }
            else
            {
                // Line of sight is blocked by another object
                Debug.DrawLine(Player.transform.position, target.position, Color.blue);
                break;
            }
        }

        return ret;
    }

    Node findNearestSeenNode()
    {
        Node node;
        Node ret_node = nodeList[1];
        int ret_index = 0;
        float temp_dist;
        float dist = 1000000f;
        for (int i = 0; i < nodeList.Length; i++)
        {
            node = nodeList[i];
            if (hasLineOfSight(node.node_obj, "node"))
            {
                temp_dist = Vector3.Distance(node.node_obj.position, Player.transform.position);
                if (temp_dist < dist)
                {
                    dist = temp_dist;
                    ret_node = node;
                    ret_index = i;
                }
            }
        }

        //Modifying list to only hold indexes of valid spawns
        if (currentInvalid != -1)
        {
            validSpawns.Add(currentInvalid);
        }
        validSpawns.Remove(ret_index);
        currentInvalid = ret_index;
        return ret_node;
    }

    void getNearestNodeMap()
    {
        nearestMap = maps[0];
        float currDist;
        float smallestDist = Vector3.Distance(Player.transform.position, nearestMap.transform.position);
        for (int i = 0; i < maps.Length; i++)
        {
            currDist = Vector3.Distance(Player.transform.position, maps[i].transform.position);
            if (smallestDist > currDist)
            {
                nearestMap = maps[i];
                smallestDist = Vector3.Distance(Player.transform.position, nearestMap.transform.position);
            }
        }
    }

    void checkpoint()
    {
        //replace spawns with the parts of the ship once we get there
        switch (waveNumber)
        {
            case 4:
                setImage(magnetometerSprite);
                setText(magnetometerString);
                showMenu(PartsPopup);
                revealImage(magnetometer);
                break;
            case 7:
                setImage(multispecSprite);
                setText(multispecString);
                showMenu(PartsPopup);
                revealImage(multispec);
                break;
            case 10:
                setImage(neutronSpecSprite);
                setText(neutronString);
                showMenu(PartsPopup);
                revealImage(neutronSpec);
                break;
            case 13:
                setImage(xBandRadioSprite);
                setText(xBandRadioString);
                showMenu(PartsPopup);
                revealImage(xBandRadio);
                break;
            case 16:
                setImage(gammaSpecSprite);
                setText(gammaString);
                showMenu(PartsPopup);
                revealImage(gammaSpec);
                break;
            default:
                showMenu(UpgradeMenu);
                break;
        }

        //Showing upgrade menu for players
        waiting = true;
        StartCoroutine(WaitBetweenWaves());
    }

    private void revealImage(GameObject obj)
    {
        Image img = obj.GetComponent<Image>();
        if (img != null)
        {
            string hexColor = "#FFFFFF"; // Magenta

            // Convert hex string to Unity Color
            Color newColor;
            if (ColorUtility.TryParseHtmlString(hexColor, out newColor))
            {
                img.color = newColor;
            }
        }
    }
    //calculates which enemies to spawn depending on wave number
    int calcEnemyRange()
    {
        int range_max;
        range_max = waveNumber / 3;
        if (range_max > 3)
        {
            range_max = 3;
        }
        return range_max;
    }

    public void ResetScaling()
    {
        scaledEnemies = false;
    }

    public void showMenu(GameObject menu)
    {
        Time.timeScale = 0f;
        menu.SetActive(true);
    }

    public void closeMenu()
    {
        Time.timeScale = 1.0f;
        PartsPopup.SetActive(false);
        showMenu(UpgradeMenu);
    }

    private void setImage(Sprite sprite)
    {
        Transform partImageTransform = PartsPopup.transform.Find("PartImage");
        if (partImageTransform != null)
        {
            Image partImage = partImageTransform.GetComponent<Image>();
            if (partImage != null)
            {
                partImage.sprite = sprite;
            }
        }

    }

    private void setText(string text)
    {
        Transform partTextTransform = PartsPopup.transform.Find("NameTemplate");
        if (partTextTransform != null)
        {
            Text partText = partTextTransform.GetComponent<Text> ();
            if (partText != null)
            {
                partText.text = text;
            }
        }
    }
    IEnumerator WaitBetweenWaves()
    {
        timer.stopTimer = true;
        yield return new WaitForSeconds(3f); // Increased break time to 10 seconds
        timer.stopTimer = false;
        timer.UpdateWaveUI();
        waiting = false;
    }
}