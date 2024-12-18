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
    const string YELLOW_MESSAGE =  "Yellows's Turn";

    Color RED_COLOR = new Color(231, 29, 54, 255) / 255;
    Color YELLOW_COLOR = new Color(255f, 255f, 0f, 255f) / 255f;
    
    Board myBoard;


    private void Awake()
    {
        isPlayer = true;
        hasGameFinished = false;
        turnMessage.text = RED_MESSAGE;
        turnMessage.color = RED_COLOR;
        myBoard = new Board();
    }


    public void GameStart()
    {
        Debug.Log("Game Start Pressed | loading scene 1");
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void GameQuit()
    {
        Debug.Log("Quiting game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }


    private void Update()
    {
        // Check if the mouse button (left click) is pressed
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse button pressed.");  // Debug log for mouse click

            // If the game is finished, return immediately
            if (hasGameFinished)
            {
                Debug.Log("Game has finished. No further action.");
                return;
            }

            // Convert mouse position to world position
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            
            // Perform raycast to detect the object clicked
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (!hit.collider)
            {
                Debug.Log("No collider hit.");  // Debug log if nothing is hit
                return;
            }

            // If the hit object has the "Press" tag, continue
            if (hit.collider.CompareTag("Press"))
            {
                Debug.Log("Hit object with 'Press' tag.");  // Debug log for valid hit

                // Check if the target location's y-coordinate is out of bounds
                if (hit.collider.gameObject.GetComponent<Column>().targetlocation.y > 1.5f)
                {
                    Debug.Log("Target location is out of bounds (y > 1.5).");  // Debug log if out of bounds
                    return;
                }

                // Get spawn and target positions from the column
                Vector3 spawnPos = hit.collider.gameObject.GetComponent<Column>().spawnLocation;
                Vector3 targetPos = hit.collider.gameObject.GetComponent<Column>().targetlocation;

                // Log spawn and target positions
                Debug.Log($"Spawn Position: {spawnPos}, Target Position: {targetPos}");

                // Instantiate the correct game object (red or yellow)
                GameObject circle = Instantiate(isPlayer ? red : Yellow);
                circle.transform.position = spawnPos;
                circle.GetComponent<Mover>().targetPostion = targetPos;

                // Increase the targetLocation height by 0.7f
                hit.collider.gameObject.GetComponent<Column>().targetlocation = new Vector3(targetPos.x, targetPos.y + 0.7f, targetPos.z);
                Debug.Log($"Updated target location: {hit.collider.gameObject.GetComponent<Column>().targetlocation}");

                // Update the board with the current move
                myBoard.UpdateBoard(hit.collider.gameObject.GetComponent<Column>().col - 1, isPlayer);

                // Check the result of the board after the move
                if (myBoard.Result(isPlayer))
                {
                    Debug.Log($"Player {(isPlayer ? "Red" : "Yellow")} wins!");  // Debug log for win condition
                    turnMessage.text = (isPlayer ? "Red" :  "Yellow") + " Wins!";
                    hasGameFinished = true;
                    return;
                }

                // Update the turn message
                turnMessage.text = !isPlayer ? RED_MESSAGE : YELLOW_MESSAGE;
                turnMessage.color = !isPlayer ? RED_COLOR : YELLOW_COLOR;

                // Log the turn change
                Debug.Log($"Turn changed. Next player: {(isPlayer ? "Yellow" : "Red")}");

                // Switch the player turn
                isPlayer = !isPlayer;
            }
            else
            {
                Debug.Log("Clicked on an object without 'Press' tag.");  // Debug log for invalid object
            }
        }
    }
}