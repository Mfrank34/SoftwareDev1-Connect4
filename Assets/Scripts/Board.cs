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
        // create the current game board
        playerBoard = new PlayerType[6][]; // y = 6
        // makes it 6x7 accross
        for (int i = 0; i < playerBoard.Length; i++)
        {
            // loops for 7 = x 
            playerBoard[i] = new PlayerType[7];
            for(int j =0; j < playerBoard[i].Length; j++)
            {
                // sets them to none creating blank space for tokens
                playerBoard[i][j] = PlayerType.NONE;
            }
        }
    }

    public void UpdateBoard(int col,bool isPlayer)
    {
        int updatePos = 6; // For x amount of collumes 
        for (int i = 5; i >= 0; i-- ) // for y amount of rows 
        {
            // looks for empty rows and removes them depending on col and player verables
            if(playerBoard[i][col] == PlayerType.NONE)
            {
                // removes placeholder
                updatePos--;
            }
            else
            {
                // i know break, little lazy IK
                break;
            }
        }
        // updates with the current player and 
        playerBoard[updatePos][col] = isPlayer ? PlayerType.RED : PlayerType.YELLOW;
        // addes it to grid manager 
        currentPos = new GridPos { row = updatePos, col = col };
    }

    public bool Result(bool isPlayer)
    {
        // find results the game
        PlayerType current = isPlayer ? PlayerType.RED : PlayerType.YELLOW; // checks what player is in play!
        // below very fancy function chaining!
        return IsHorizontal(current) || IsVertical(current) || IsDiagonal(current) || IsReverseDiagonal(current);
    }

    bool IsHorizontal(PlayerType current)
    {
        // looks over the grid, and looks for a winner in horizontal
        GridPos start = GetEndPoint(new GridPos { row = 0, col = -1 });
        List<GridPos> toSearchList = GetPlayerList(start, new GridPos { row = 0, col = 1 });
        return SearchResult(toSearchList,current);
    }

    bool IsVertical(PlayerType current)
    {
        // looks over the grid, and looks for a winner in Vertical
        GridPos start = GetEndPoint(new GridPos { row = -1, col = 0 });
        List<GridPos> toSearchList = GetPlayerList(start, new GridPos { row = 1, col = 0 });
        return SearchResult(toSearchList, current);
    }

    bool IsDiagonal(PlayerType current)
    {
        // looks over the grid, and looks for a winner in Diagonal
        GridPos start = GetEndPoint(new GridPos { row = -1, col = -1 });
        List<GridPos> toSearchList = GetPlayerList(start, new GridPos { row = 1, col = 1 });
        return SearchResult(toSearchList, current);
    }

    bool IsReverseDiagonal(PlayerType current)
    {
        // looks over the grid, and looks for a winner in Reverse Diagonal
        GridPos start = GetEndPoint(new GridPos { row = -1, col = 1 });
        List<GridPos> toSearchList = GetPlayerList(start, new GridPos { row = 1, col = -1 });
        return SearchResult(toSearchList, current);
    }

    GridPos GetEndPoint(GridPos diff)
    {
        // cool grid pathing for the coded game board, not gui elements.
        GridPos result = new GridPos { row = currentPos.row, col = currentPos.col };
        while (result.row + diff.row < 6 &&
                result.col + diff.col < 7 &&
                result.row + diff.row >=0 &&
                result.col + diff.col >=0)
        {
            result.row += diff.row;
            result.col += diff.col;
        }
        return result;
    }

    List<GridPos> GetPlayerList(GridPos start, GridPos diff)
    {
        // listing grid location for later use & helping create the winning function.
        List<GridPos> resList;
        resList = new List<GridPos> { start };
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

    bool SearchResult(List<GridPos> searchList, PlayerType current)
    {
        // starts a counter
        int counter = 0;

        // searches the list of locations
        for(int i = 0; i < searchList.Count; i++)
        {
            // looks though and compare locations with current postion 
            PlayerType compare = playerBoard[searchList[i].row][searchList[i].col];
            if( compare == current)
            {
                // counts 
                counter++;
                if (counter == 4)
                    break;
            }
            else
            {
                counter = 0;
            }
        }
        // return if 4 in a row is found.
        return counter >= 4;
    }
}
