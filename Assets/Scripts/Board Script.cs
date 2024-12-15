using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum PlayerType { NONE,RED,YELLOW}

public struct GridPos { public int row, col; }

public class Board
{
    public NetworkVariable<PlayerType[]> playerBoardNetwork = new NetworkVariable<PlayerType[]>(new PlayerType[42]); // sync board state (6 rows 7 columns)
    PlayerType[][] playerBoard;
    GridPos currentPos;

    public Board()
    {
        playerBoard = new PlayerType[6][];
        for (int i = 0; i < playerBoard.Length; i++)
        {
            playerBoard[i] = new PlayerType[7];
            for(int j =0; j < playerBoard[i].Length; j++)
            {
                playerBoard[i][j] = PlayerType.NONE;
            }
        }
    }

    public override void onNetworkSpawn()
    {
        base.onNetworkSpawn();
        if (isServer)
        {
            SyncBoardState();
        }
    }

    private void SyncBoardState()
{
    // Convert to flattened array
    PlayerType[] flattenedBoard = new PlayerType[42]; // 6 rows * 7 columns = 42
    for (int i = 0; i < 6; i++)
    {
        for (int j = 0; j < 7; j++)
        {
            flattenedBoard[i * 7 + j] = playerBoard[i][j];
        }
    }
    playerBoardNetwork.Value = flattenedBoard;  // Sync the board state across all clients
}


    public void UpdateBoard(int col,bool isPlayer)
    {
        if (!IsServer) return; // only server can update board
        int updatePos = 6;
        for (int i = 5; i >= 0; i-- )
        {
            if(playerBoard[i][col] == PlayerType.NONE)
            {
                updatePos--;
            }
            else
            {
                break;
            }
        }

        playerBoard[updatePos][col] = isPlayer ? PlayerType.RED : PlayerType.YELLOW;
        currentPos = new GridPos { row = updatePos, col = col };

        SyncBoardState();
    }

    private void SyncBoardState()
    {
        PlayerType[] flattenedBoard = new PlayerType[42]; // Flatten the board
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                flattenedBoard[i * 7 + j] = playerBoard[i][j];
            }
        }

        // Update the NetworkVariable with the new state
        playerBoardNetwork.Value = flattenedBoard;

        // notify clients board was updated
        UpdateBoardOnClients();
    }
    

    // win condition

    public bool Result(bool isPlayer)
    {
        PlayerType current = isPlayer ? PlayerType.RED : PlayerType.YELLOW;
        return IsHorizontal(current) || IsVertical(current) || IsDiagonal(current) || IsReverseDiagonal(current);

    }

    public bool Result(bool isPlayer)
    {
        PlayerType current = isPlayer ? PlayerType.RED : PlayerType.YELLOW;
        return IsHorizontal(current) || IsVertical(current) || IsDiagonal(current) || IsReverseDiagonal(current);
    }

    bool IsHorizontal(PlayerType current)
    {
        GridPos start = GetEndPoint(new GridPos { row = 0, col = -1 });
        List<GridPos> toSearchList = GetPlayerList(start, new GridPos { row = 0, col = 1 });
        return SearchResult(toSearchList,current);
    }

    bool IsVertical(PlayerType current)
    {
        GridPos start = GetEndPoint(new GridPos { row = -1, col = 0 });
        List<GridPos> toSearchList = GetPlayerList(start, new GridPos { row = 1, col = 0 });
        return SearchResult(toSearchList, current);
    }

    bool IsDiagonal(PlayerType current)
    {
        GridPos start = GetEndPoint(new GridPos { row = -1, col = -1 });
        List<GridPos> toSearchList = GetPlayerList(start, new GridPos { row = 1, col = 1 });
        return SearchResult(toSearchList, current);
    }

    bool IsReverseDiagonal(PlayerType current)
    {
        GridPos start = GetEndPoint(new GridPos { row = -1, col = 1 });
        List<GridPos> toSearchList = GetPlayerList(start, new GridPos { row = 1, col = -1 });
        return SearchResult(toSearchList, current);
    }

    GridPos GetEndPoint(GridPos diff)
    {
        GridPos result = new GridPos { row = currentPos.row, col = currentPos.col };
        while (result.row + diff.row < 6 &&
                result.col + diff.col < 7 &&
                result.row + diff.row >= 0 &&
                result.col + diff.col >= 0)
        {
            result.row += diff.row;
            result.col += diff.col;
        }
        return result;
    }

    List<GridPos> GetPlayerList(GridPos start, GridPos diff)
    {
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
        int counter = 0;

        for(int i = 0; i < searchList.Count; i++)
        {
            PlayerType compare = playerBoard[searchList[i].row][searchList[i].col];
            if( compare == current)
            {
                counter++;
                if (counter == 4)
                    break;
            }
            else
            {
                counter = 0;
            }
        }

        return counter >= 4;
    }
}