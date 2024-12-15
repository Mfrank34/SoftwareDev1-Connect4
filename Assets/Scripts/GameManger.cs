using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private void Awake()
    {
        isPlayer = true;
        hasGameFinished = false;
        turnMessage.text = RED_MESSAGE;
        turnMessage.color = RED_COLOR;
        myBoard = new Board();
        timeSinceLastMove = cooldownTime; // Initialize to allow the first move immediately
    }

    public void GameStart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);

    }

    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

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
                    // Check if the column is full (out of bounds)
                    if (hit.collider.gameObject.GetComponent<Column>().targetlocation.y > 1.5f) return;

                    // Spawn the GameObject (Red or Yellow)
                    Vector3 spawnPos = hit.collider.gameObject.GetComponent<Column>().spawnLocation;
                    Vector3 targetPos = hit.collider.gameObject.GetComponent<Column>().targetlocation;
                    GameObject circle = Instantiate(isPlayer ? red : Yellow);
                    circle.transform.position = spawnPos;
                    circle.GetComponent<Mover>().targetPostion = targetPos;

                    // Update the target location for the column
                    hit.collider.gameObject.GetComponent<Column>().targetlocation = new Vector3(targetPos.x, targetPos.y + 0.7f, targetPos.z);

                    // Update the board with the player's move
                    myBoard.UpdateBoard(hit.collider.gameObject.GetComponent<Column>().col - 1, isPlayer);

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
                    timeSinceLastMove = 0f;  // Start the cooldown after a move
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
