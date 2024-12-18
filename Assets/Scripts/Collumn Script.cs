using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Column : MonoBehaviour
{
    public int col;  // Column index (0-6 for a 7-column grid)
    public Vector3 spawnLocation;  // Spawn at top of the column
    public Vector3 targetLocation;  // Target at the bottom of the column
    public bool isFull;  // Flag to check if the column is full

    private void Start()
    {
        // Set the spawn and target locations
        spawnLocation = new Vector3(transform.position.x, 5f, 0f);  // Top of the column
        targetLocation = new Vector3(transform.position.x, -3f, 0f);  // Start at the bottom
        isFull = false;
    }

    // Update the column's state after a token is placed
    public void UpdateColumnState()
    {
        targetLocation = new Vector3(targetLocation.x, targetLocation.y - 0.7f, targetLocation.z);  // Move the target position down
        if (targetLocation.y < -3f)  // Check if column is full
        {
            isFull = true;  // Mark the column as full
        }
    }

    // Reset column state (for a new game)
    public void ResetColumn()
    {
        targetLocation = new Vector3(transform.position.x, -3f, 0f);  // Reset target position to bottom
        isFull = false;  // Reset fullness
    }
}
