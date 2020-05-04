using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Simulation
{
    protected PieceType[,] simulatedBoard;
    public bool isPlayersTurn;
    private int PiecesToWin => ConnectFourController.piecesToWin;
    public bool ContainsEmptyCell => GetPossibleMovements().Count != 0;
    public bool SomeoneWon => CheckWin() != 0;

    public Simulation(bool isPlayersTurn, PieceType[,] board)
    {
        this.isPlayersTurn = isPlayersTurn;

        simulatedBoard = new PieceType[board.GetLength(0), board.GetLength(1)];
        for (int x = 0; x < board.GetLength(0); x++) {
            for (int y = 0; y < board.GetLength(1); y++) {
                simulatedBoard[x, y] = board[x, y];
            }
        }
    }

    public Simulation Clone()
    {
        return new Simulation(isPlayersTurn, simulatedBoard);
    }

    public List<Vector2Int> GetPossibleMovements()
    {
        List<Vector2Int> possibleMovements = new List<Vector2Int>();

        for (int i = 0; i < simulatedBoard.GetLength(1); i++) {
            for (int j = 0; j < simulatedBoard.GetLength(0); j++) {
                if (simulatedBoard[j, i] == PieceType.Empty) {
                    possibleMovements.Add(new Vector2Int(j, i));
                    break;
                }
            }
        }

        return possibleMovements;
    }

    public int SimulateDrop(int column)
    {
        var movements = GetPossibleMovements();
        if(!movements.Exists(element => element.y == column)){
            return -1;
        }

        Vector2Int indexes = movements.Find(element => element.y == column);
        simulatedBoard[indexes.x, indexes.y] = isPlayersTurn ? PieceType.Red : PieceType.Blue;
        return column;
    }

    public int GetRandomMove()
    {
        if (!ContainsEmptyCell) {
            return -1;
        }

        List<Vector2Int> movements = GetPossibleMovements();

        System.Random r = new System.Random();
        return movements[r.Next(0, movements.Count)].y;
    }

    public int CheckWin()
    {
        for (int i = 0; i < simulatedBoard.GetLength(0); i++) {
            for (int j = 0; j < simulatedBoard.GetLength(1); j++) {
                bool win = CheckWin(i, j);
                if (win) {
                    return isPlayersTurn ? 1 : -1;
                }
            }
        }

        return 0;
    }

    public bool CheckWin(int x, int y)
    {
        //skip empty slots
        PieceType type = simulatedBoard[x, y];
        if (type == PieceType.Empty) {
            return false;
        }

        //prevent out of bounds
        int range = PiecesToWin - 1;
        bool insideRangeX = x + range < simulatedBoard.GetLength(0);
        bool insideRangeY1 = y + range < simulatedBoard.GetLength(1);
        bool insideRangeY2 = y - range >= 0;

        //win
        bool winHorizontal = insideRangeX;
        bool winVertical = insideRangeY1;
        bool winDiagonal1 = insideRangeX && insideRangeY1;
        bool winDiagonal2 = insideRangeX && insideRangeY2;

        //check conditions
        for (int w = 1; w < PiecesToWin; w++) {
            winHorizontal = winHorizontal && (type == simulatedBoard[x + w, y]);
            winVertical = winVertical && (type == simulatedBoard[x, y + w]);
            winDiagonal1 = winDiagonal1 && (type == simulatedBoard[x + w, y + w]);
            winDiagonal2 = winDiagonal2 && (type == simulatedBoard[x + w, y - w]);
        }

        //return true if any condition was met
        return winHorizontal || winVertical || winDiagonal1 || winDiagonal2;
    }

    public void SwitchPlayer()
    {
        isPlayersTurn = !isPlayersTurn;
    }
}