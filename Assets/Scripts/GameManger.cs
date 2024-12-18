using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// if you reading this kill me!

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject red, yellow;  // Prefabs for Red and Yellow tokens

    [SerializeField]
    Column[] columns;  // Array of columns
    bool isPlayer;  // Tracks the current player (Red or Yellow)
    bool hasGameFinished;  // Flag to check if the game has finished
    Board myBoard;  // Reference to the board
    float cooldownTime = 0.5f;  // Cooldown time between player inputs
    float timeSinceLastMove;

    [SerializeField]
    Text turnMessage;

    const string RED_MESSAGE = "Red's Turn";
    const string YELLOW_MESSAGE = "Yellow's Turn";

    void Awake()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        isPlayer = true;
        hasGameFinished = false;
        myBoard = new Board();  // Initialize the board
        timeSinceLastMove = cooldownTime;
        turnMessage.text = RED_MESSAGE;
    }

    public void GameStart()
    {
        // Reset game state for a new game
        foreach (Column column in columns)
        {
            column.ResetColumn();
        }
        myBoard = new Board();
        hasGameFinished = false;
        isPlayer = true;
        turnMessage.text = RED_MESSAGE;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

        #if UNITY_EDITOR
            // If running in Unity Editor, stop play mode
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    void Update()
    {
        if (timeSinceLastMove >= cooldownTime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (hasGameFinished) return;

                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

                if (hit.collider != null && hit.collider.CompareTag("Press"))
                {
                    Column clickedColumn = hit.collider.gameObject.GetComponent<Column>();

                    if (clickedColumn.isFull)
                    {
                        Debug.Log("Column is full.");
                        return;
                    }

                    if (myBoard.UpdateBoard(clickedColumn.col, isPlayer))
                    {
                        GameObject token = Instantiate(isPlayer ? red : yellow, clickedColumn.spawnLocation, Quaternion.identity);
                        token.GetComponent<Mover>().targetPostion = clickedColumn.targetLocation;

                        clickedColumn.UpdateColumnState();

                        if (myBoard.Result(isPlayer))
                        {
                            turnMessage.text = (isPlayer ? "Red" : "Yellow") + " Wins!";
                            hasGameFinished = true;
                            return;
                        }

                        isPlayer = !isPlayer;
                        turnMessage.text = isPlayer ? RED_MESSAGE : YELLOW_MESSAGE;
                    }
                    timeSinceLastMove = 0f;  // Reset cooldown
                }
            }
        }
        else
        {
            timeSinceLastMove += Time.deltaTime;
        }
    }
}
