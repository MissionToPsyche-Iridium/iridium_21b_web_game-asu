using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGenerator : MonoBehaviour
{
    // Reference to the Tilemap; assign via Inspector.
    public Tilemap tilemap;

    // The tile you want to place; assign via Inspector.
    public Tile tileToPlace;

    // The player Transform; assign via Inspector.
    public Transform playerTransform;

    // How many cells ahead of the player should the tiles be generated.
    public int tilesAhead = 5;

    // The direction to generate from player.
    // Assuming the player is moving horizontally to the right.
    // If your player uses a different axis, adjust accordingly.
    public Vector3Int placementDirection = new Vector3Int(1, 0, 0);

    // Keep track of the furthest tile position reached.
    private int furthestX = 0;

    void Start()
    {
        if (tilemap == null || tileToPlace == null || playerTransform == null)
        {
            Debug.LogError("Please assign all required references in the Inspector.");
        }
    }

    void Update()
    {
        // Convert the player's world position to cell position.
        Vector3Int playerCellPos = tilemap.WorldToCell(playerTransform.position);

        // Calculate where the generation should extend (for example, if generating tiles to the right of the player).
        int targetX = playerCellPos.x + tilesAhead;

        // Check if we need to generate new tiles along the X-axis.
        // Here, furthestX keeps track of the rightmost column we've produced.
        // For more complex systems (like multiple rows or irregular generation), you'll adapt this logic.
        while (furthestX < targetX)
        {
            // Define the cell position for the new tile.
            Vector3Int newTilePos = new Vector3Int(furthestX + 1, playerCellPos.y, 0);

            // Place the tile on the tilemap.
            tilemap.SetTile(newTilePos, tileToPlace);

            // Update the furthest generated tile.
            furthestX++;
        }
    }
}
