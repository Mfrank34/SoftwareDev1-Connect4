using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType { NONE, RED, YELLOW }

public struct GridPos { public int row, col; }

public class Board
{
    PlayerType[][] playerBoard;
    GridPos currentPos;

    public void Board()
    {
        // Initialize the player board with empty values
        playerBoard = new PlayerType[6][];
        for (int i = 0; i < playerBoard.Length; i++)
        {
            playerBoard[i] = new PlayerType[7];  // 7 columns in each row
            for (int j = 0; j < playerBoard[i].Length; j++)
            {
                playerBoard[i][j] = PlayerType.NONE;  // Set all positions to NONE (empty)
            }
        }
        // currentPos = new GridPos { row = -1, col = -1 };  // Initialize to invalid position
    }


    // Function to update the board with the current move
    public void UpdateBoard(int col, bool isPlayer)
    {
        int updatePos = -1;  // Start with no available position

        // Check for an empty spot in the selected column
        for (int i = 5; i >= 0; i--)
        {
            if (playerBoard[i][col] == PlayerType.NONE)  // If the spot is empty
            {
                updatePos = i;  // Save the row number
                break;  // Stop looking
            }
        }

        // If no empty spot is found (column is full)
        if (updatePos == -1)
        {
            Debug.LogError($"Column {col} is full.");
            return false;  // Cannot make the move
        }

        // Place the player's piece in the found row
        playerBoard[updatePos][col] = isPlayer ? PlayerType.RED : PlayerType.YELLOW;

        // Update the position of the last move
        currentPos = new GridPos { row = updatePos, col = col };

        return true;  // Move was successful
    }

    // Helper function to check a given direction for a winning sequence
    bool CheckDirection(GridPos diff, PlayerType current)
    {
        GridPos start = GetEndPoint(new GridPos { row = -diff.row, col = -diff.col });
        List<GridPos> toSearchList = GetPlayerList(start, diff);
        return SearchResult(toSearchList, current);
    }

    // Function to check if the game is over (win or tie)
    public bool Result(bool isPlayer)
    {
        if (currentPos.row < 0 || currentPos.row >= 6 || 
            currentPos.col < 0 || currentPos.col >= 7)
        {
            Debug.LogError("Current position is invalid. Cannot check result.");
            return false;
        }

        PlayerType current = isPlayer ? PlayerType.RED : PlayerType.YELLOW;

        // Check horizontal direction
        if (CheckDirection(new GridPos { row = 0, col = 1 }, current))
        {
            return true;  // Player wins in horizontal direction
        }

        // Check vertical direction
        if (CheckDirection(new GridPos { row = 1, col = 0 }, current))
        {
            return true;  // Player wins in vertical direction
        }

        // Check diagonal direction
        if (CheckDirection(new GridPos { row = 1, col = 1 }, current))
        {
            return true;  // Player wins in diagonal direction
        }

        // Check reverse diagonal direction
        if (CheckDirection(new GridPos { row = 1, col = -1 }, current))
        {
            return true;  // Player wins in reverse diagonal direction
        }

        // If no win is found, return false
        return false;
    }

    // Function to check if the board is full (indicating a tie)
    public bool IsBoardFull()
    {
        for (int col = 0; col < 7; col++)
        {
            if (playerBoard[0][col] == PlayerType.NONE)  // Top row has empty spots
                return false;
        }
        return true;  // Board is full
    }

   
    // Helper function to get the farthest valid position in a given direction
    GridPos GetEndPoint(GridPos diff)
    {
        GridPos result = new GridPos { row = currentPos.row, col = currentPos.col };
        while (result.row + diff.row < 6 &&
               result.col + diff.col < 7 &&
               result.row + diff.row >= 0 &&
               result.col + diff.col >= 0 &&
               playerBoard[result.row + diff.row][result.col + diff.col] ==
               playerBoard[currentPos.row][currentPos.col])  // Stop if the player type doesn't match
        {
            result.row += diff.row;
            result.col += diff.col;
        }
        return result;
    }

    // Helper function to get a list of all positions along the line in the given direction
    List<GridPos> GetPlayerList(GridPos start, GridPos diff)
    {
        List<GridPos> resList = new List<GridPos> { start };
        GridPos result = new GridPos { row = start.row, col = start.col };
        while (result.row + diff.row < 6 &&
               result.col + diff.col < 7 &&
               result.row + diff.row >= 0 &&
               result.col + diff.col >= 0)
        {
            result.row += diff.row;
            result.col += diff.col;
            resList.Add(result);
        }

        return resList;
    }

    // Helper function to check if there are 4 consecutive pieces in the list
    bool SearchResult(List<GridPos> searchList, PlayerType current)
    {
        int counter = 0;

        for (int i = 0; i < searchList.Count; i++)
        {
            PlayerType compare = playerBoard[searchList[i].row][searchList[i].col];
            if (compare == current)
            {
                counter++;
                if (counter == 4)
                    break;
            }
            else
            {
                counter = 0;  // Reset if the sequence is broken
            }
        }

        return counter >= 4;  // Return true if there are 4 consecutive pieces
    }
}
