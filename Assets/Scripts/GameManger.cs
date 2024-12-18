using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// if you reading this kill me!

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject red, Yellow;

    bool isPlayer, hasGameFinished;

    [SerializeField]
    Text turnMessage;

    const string RED_MESSAGE = "Red's Turn";
    const string YELLOW_MESSAGE = "Yellow's Turn";

    Color RED_COLOR = new Color(231, 29, 54, 255) / 255;
    Color YELLOW_COLOR = new Color(1f, 1f, 0f, 1f);

    Board myBoard;

    // Cooldown time between player inputs (in seconds)
    [SerializeField]
    float cooldownTime = 0.5f;
    float timeSinceLastMove;

    // A list to track dynamically spawned tokens
    List<GameObject> spawnedTokens = new List<GameObject>();

    private void Awake()
    {
        InitializeGame();
    }

    public void GameStart()
    {
        // Clear all dynamically spawned game objects
        foreach (GameObject token in spawnedTokens)
        {
            Debug.Log("Destroying token: " + token.name);  // Log token destruction for debugging
            Destroy(token);
        }   
        spawnedTokens.Clear();

        // Reset the game state
        InitializeGame();
    }

// Reset the game Properly IK, im dum ass
    private void InitializeGame()
    {
        isPlayer = true;
        hasGameFinished = false;

        turnMessage.text = RED_MESSAGE;
        turnMessage.color = RED_COLOR;

        myBoard = new Board(); // Reset the board to a new instance
        timeSinceLastMove = cooldownTime; // Allow the first move immediately
    }

    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

// starting to hate...
    private void Update()
    {
        // Only allow input if enough time has passed since the last move
        if (timeSinceLastMove >= cooldownTime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // If the game has finished, do nothing
                if (hasGameFinished) return;

                // Raycast2D to detect mouse position
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

                if (!hit.collider) return;

                if (hit.collider.CompareTag("Press"))
                {
                    // Get the column component
                    Column column = hit.collider.gameObject.GetComponent<Column>();

                    // Check if the column is full (out of bounds)
                    if (column.targetlocation.y > 1.5f) return;
                    // Spawn the GameObject (Red or Yellow)
                    Vector3 spawnPos = column.spawnLocation;
                    Vector3 targetPos = column.targetlocation;
                    GameObject circle = Instantiate(isPlayer ? red : Yellow, spawnPos, Quaternion.identity);

                    // Set the target position for the Mover component
                    Mover mover = circle.GetComponent<Mover>();
                    mover.targetPostion = targetPos;  // Set the target position for the token

                    // Add the token to the list of spawned tokens
                    spawnedTokens.Add(circle);

                    // Update the target location for the column
                    column.targetlocation = new Vector3(targetPos.x, targetPos.y + 0.7f, targetPos.z);

                    // Update the board with the player's move
                    myBoard.UpdateBoard(column.col - 1, isPlayer);

                    // Check if the player has won
                    if (myBoard.Result(isPlayer))
                    {
                        turnMessage.text = (isPlayer ? "Red" : "Yellow") + " Wins!";
                        hasGameFinished = true;
                        return;
                    }

                    // Update the turn message and change the player turn
                    turnMessage.text = !isPlayer ? RED_MESSAGE : YELLOW_MESSAGE;
                    turnMessage.color = !isPlayer ? RED_COLOR : YELLOW_COLOR;

                    // Switch the player turn
                    isPlayer = !isPlayer;

                    // Reset the cooldown timer
                    timeSinceLastMove = 0f; // Start the cooldown after a move
                }
            }
        }
        else
        {
            // Increment the timer with the time passed since the last frame
            timeSinceLastMove += Time.deltaTime;
        }
    }
}
