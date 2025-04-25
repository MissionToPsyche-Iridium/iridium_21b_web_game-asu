using UnityEngine;

public class PlayerBoundaryHandler : MonoBehaviour
{
    public float mapMinX;
    public float mapMaxX;
    public float mapMinY;
    public float mapMaxY;

    void Update()
    {
        // Check if the player is outside the map boundaries
        if (!IsWithinBoundaries(transform.position))
        {
            Vector3 clampedPosition = GetValidPosition(transform.position);
            transform.position = clampedPosition;
        }
    }

    bool IsWithinBoundaries(Vector3 position)
    {
        return (position.x >= mapMinX && position.x <= mapMaxX &&
                position.y >= mapMinY && position.y <= mapMaxY);
    }

    Vector3 GetValidPosition(Vector3 currentPosition)
    {
        // Clamp the position to the map boundaries
        float clampedX = Mathf.Clamp(currentPosition.x, mapMinX, mapMaxX);
        float clampedY = Mathf.Clamp(currentPosition.y, mapMinY, mapMaxY);

        return new Vector3(clampedX, clampedY, currentPosition.z);
    }
}