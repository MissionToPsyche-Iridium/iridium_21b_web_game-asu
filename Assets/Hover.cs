using UnityEngine;

public class Hover : MonoBehaviour
{
    [Header("Hover Settings")]
    [Tooltip("The maximum height to hover above and below the starting position.")]
    public float hoverHeight = 0.5f;

    [Tooltip("The speed of the hovering motion.")]
    public float hoverSpeed = 2f;

    private Vector3 initialPosition;

    void Start()
    {
        // Store the initial position of the GameObject
        initialPosition = transform.position;
    }

    void Update()
    {
        // Calculate the new Y position using a sine wave for smooth motion
        float newY = initialPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;

        // Update the position while keeping X and Z the same
        transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);
    }
}