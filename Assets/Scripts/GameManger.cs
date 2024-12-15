using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Board gameBoard;
    private bool isPlayerTurn = true;  //tracks players turn
    private bool GameStarted = false;

    public NetworkVariable<bool> isPlayerTurnNetwork = new NetworkVariable<bool>(true);  //syncs the turns

    [SerializeField]
    GameObject red, Yellow;

    bool isPlayer, hasGameFinished;

    [SerializeField]
    Text turnMessage;

    const string RED_MESSAGE = "Red's Turn";
    const string YELLOW_MESSAGE = "Yellow's Turn";

    Color RED_COLOR = new Color(231, 29, 54, 255) / 255;
    Color YELLOW_COLOR = new Color(0, 222, 1, 255) / 255;

    Board myBoard;


    private void Awake()
    {
        isPlayer = true;
        hasGameFinished = false;
        turnMessage.text = RED_MESSAGE;
        turnMessage.color = RED_COLOR;
        myBoard = new Board();
    }

    //start game by hosting or joining
    public void hostGame(bool isHost)
    {
        //if hosting
        if (isHost)
        {
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            NetworkManager.Singleton.StartClient(); //join as client
        }
        GameStarted = true;
    }

    public void onPlayerMove(int Column)
    {
        if (!GameStarted) return;

        //only allows player with turn to make a move
        if (isPlayerTurnNetwork.Value)
        {
            updateBoardOnServer(Column, isPlayerTurnNetwork.Value);
            isPlayerTurnNetwork.Value = false; //switches turns
        }
    }

    [ServerRpc]
    private void updateBoardOnServer(int Column, bool isPlayer)
    {
        gameBoard.UpdateBoard(Column, isPlayer); //update board on server
        updateBoardOnClients(); //sync with client
    }

    [ClientRpc]
    private void updateBoardOnClients()
    {
        //update the board on clients
        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                if (gameBoard.playerBoard[row][col] != PlayerType.NONE)
                {
                    GameObject columnObj = GameObject.FindGameObjectsWithTag("Collumn");
                    Column column = columnObj.GetComponent<Collumn>();
                    if (column != null)
                    {
                        Vector3 spawnPos = column.spawnLocation + new Vector3(0, row * 0.7f, 0); // Adjust spawn height
                        GameObject counter = Instantiate(gameBoard.playerBoard[row][col] == PlayerType.RED ? red : Yellow);
                        counter.transform.position = spawnPos;
                    }
                }
            }
        }
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
        if(Input.GetMouseButtonDown(0))
        {
            //If GameFinsished then return
            if (hasGameFinished) return;

            //Raycast2D
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (!hit.collider) return;

            if(hit.collider.CompareTag("Press"))
            {
                //Check out of Bounds
                if (hit.collider.gameObject.GetComponent<Column>().targetlocation.y > 1.5f) return;

                //Spawn the GameObject
                Vector3 spawnPos = hit.collider.gameObject.GetComponent<Column>().spawnLocation;
                Vector3 targetPos = hit.collider.gameObject.GetComponent<Column>().targetlocation;
                GameObject circle = Instantiate(isPlayer ? red : Yellow);
                circle.transform.position = spawnPos;
                circle.GetComponent<Mover>().targetPostion = targetPos;

                //Increase the targetLocationHeight
                hit.collider.gameObject.GetComponent<Column>().targetlocation = new Vector3(targetPos.x, targetPos.y + 0.7f, targetPos.z);

                //UpdateBoard
                myBoard.UpdateBoard(hit.collider.gameObject.GetComponent<Column>().col - 1, isPlayer);
                if(myBoard.Result(isPlayer))
                {
                    turnMessage.text = (isPlayer ? "Red" : "Yellow") + " Wins!";
                    hasGameFinished = true;
                    return;
                }

                //TurnMessage
                turnMessage.text = !isPlayer ? RED_MESSAGE : YELLOW_MESSAGE;
                turnMessage.color = !isPlayer ? RED_COLOR : YELLOW_COLOR;

                //Change PlayerTurn
                isPlayer = !isPlayer;
            }

        }
    }

}