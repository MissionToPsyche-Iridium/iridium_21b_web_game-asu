using UnityEngine;
using System.Collections.Generic;

public class WormBody : MonoBehaviour
{
    public Transform head; // Assign the head object
    public int segmentCount = 10;
    public float segmentSpacing = 0.5f;

    private List<Transform> segments = new List<Transform>();
    private Queue<Vector3> positions = new Queue<Vector3>();

    void Start()
    {
        // Instantiate body segments
        for (int i = 0; i < segmentCount; i++)
        {
            GameObject segment = Instantiate(gameObject, transform.position, Quaternion.identity);
            segments.Add(segment.transform);
        }
    }

    void Update()
    {
        positions.Enqueue(head.position);

        // Keep the queue at the correct size
        if (positions.Count > segmentCount * 5)
        {
            positions.Dequeue();
        }

        // Move each segment to a past position
        int index = 0;
        foreach (Transform segment in segments)
        {
            if (index < positions.Count)
            {
                segment.position = Vector3.Lerp(segment.position, positions.ToArray()[index], Time.deltaTime * 10);
            }
            index += 5; // Controls spacing between segments
        }
    }
}
