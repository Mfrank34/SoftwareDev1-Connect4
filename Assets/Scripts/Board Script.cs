using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType { NONE, RED, YELLOW }

public struct GridPos { public int row, col; }

public class Board
{
    PlayerType[][] playerBoard;
    GridPos currentPos;

    public Board()
    {
        playerBoard = new PlayerType[6][];
        for (int i = 0; i < playerBoard.Length; i++)
        {
            playerBoard[i] = new PlayerType[7];  // 7 columns
            for (int j = 0; j < playerBoard[i].Length; j++)
            {
                playerBoard[i][j] = PlayerType.NONE;  // Empty
            }
        }
        currentPos = new GridPos { row = -1, col = -1 };  // Invalid position to start
    }

    // Update the board with the current move
    public bool UpdateBoard(int col, bool isPlayer)
    {
        int updatePos = -1;

        // Find the first available row in the selected column (from bottom to top)
        for (int i = 5; i >= 0; i--)  // Start from bottom (row 5)
        {
            if (playerBoard[i][col] == PlayerType.NONE)
            {
                updatePos = i;
                break;
            }
        }

        if (updatePos == -1)
        {
            Debug.LogError($"Column {col} is full. Cannot place a token.");
            return false;  // Column is full
        }

        // Place the token in the found row and column
        playerBoard[updatePos][col] = isPlayer ? PlayerType.RED : PlayerType.YELLOW;
        currentPos = new GridPos { row = updatePos, col = col };  // Update position
        return true;
    }

    // Check if there is a winning condition
    public bool Result(bool isPlayer)
    {
        if (currentPos.row < 0 || currentPos.row >= 6 || 
            currentPos.col < 0 || currentPos.col >= 7)
        {
            Debug.LogError("Current position is invalid. Cannot check for a result.");
            return false;
        }

        PlayerType current = isPlayer ? PlayerType.RED : PlayerType.YELLOW;

        // Check for 4 in a row in all directions (horizontal, vertical, diagonal)
        return CheckDirection(new GridPos { row = 0, col = 1 }, current) ||  // Horizontal
               CheckDirection(new GridPos { row = 1, col = 0 }, current) ||  // Vertical
               CheckDirection(new GridPos { row = 1, col = 1 }, current) ||  // Diagonal
               CheckDirection(new GridPos { row = 1, col = -1 }, current);   // Reverse Diagonal
    }

    // Check the direction for a winning sequence (e.g., horizontal, vertical)
    private bool CheckDirection(GridPos direction, PlayerType current)
    {
        GridPos start = GetEndPoint(new GridPos { row = -direction.row, col = -direction.col });
        List<GridPos> toSearchList = GetPlayerList(start, direction);
        return SearchResult(toSearchList, current);
    }

    // Get the farthest valid position in a given direction
    private GridPos GetEndPoint(GridPos direction)
    {
        GridPos result = new GridPos { row = currentPos.row, col = currentPos.col };
        while (result.row + direction.row < 6 &&
               result.col + direction.col < 7 &&
               result.row + direction.row >= 0 &&
               result.col + direction.col >= 0 &&
               playerBoard[result.row + direction.row][result.col + direction.col] == playerBoard[currentPos.row][currentPos.col])
        {
            result.row += direction.row;
            result.col += direction.col;
        }
        return result;
    }

    // Get all positions along the line in the given direction
    private List<GridPos> GetPlayerList(GridPos start, GridPos direction)
    {
        List<GridPos> resList = new List<GridPos> { start };
        GridPos result = new GridPos { row = start.row, col = start.col };
        while (result.row + direction.row < 6 &&
               result.col + direction.col < 7 &&
               result.row + direction.row >= 0 &&
               result.col + direction.col >= 0)
        {
            result.row += direction.row;
            result.col += direction.col;
            resList.Add(result);
        }
        return resList;
    }

    // Check for 4 consecutive tokens in the list
    private bool SearchResult(List<GridPos> searchList, PlayerType current)
    {
        int counter = 0;
        foreach (var pos in searchList)
        {
            if (playerBoard[pos.row][pos.col] == current)
            {
                counter++;
                if (counter == 4)
                    return true;
            }
            else
            {
                counter = 0;
            }
        }
        return false;
    }
}
