using UnityEngine;
using System.Collections.Generic;

public class body_behavior : MonoBehaviour
{
    public GameObject head; // Assign the head object
    public int segmentCount = 10;
    public float segmentSpacing = 0.5f;
    public GameObject seg_prefab;

    private List<Transform> segments = new List<Transform>();
    private Queue<Vector3> positions = new Queue<Vector3>();

    void Start()
    {
    }

    void Update()
    {

    }
}
