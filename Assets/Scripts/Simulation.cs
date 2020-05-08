using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Simulation
{
    //data
    protected PieceType[,] simulatedBoard;
    public bool isPlayersTurn;

    //shortcuts
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
        return Simulator.GetPossibleMovements(simulatedBoard);
    }

    public int SimulateDrop(int column)
    {
        return Simulator.SimulateDrop(simulatedBoard, column, isPlayersTurn);
    }

    public int GetRandomMove()
    {
        return Simulator.GetRandomMove(simulatedBoard);
    }

    public int CheckWin()
    {
        return Simulator.CheckWin(simulatedBoard, isPlayersTurn);
    }

    public void SwitchPlayer()
    {
        isPlayersTurn = !isPlayersTurn;
    }
}

public class Simulator : MonoBehaviour
{
    public static List<Vector2Int> GetPossibleMovements(PieceType[,] board)
    {
        List<Vector2Int> possibleMovements = new List<Vector2Int>();

        for (int i = 0; i < board.GetLength(1); i++) {
            for (int j = 0; j < board.GetLength(0); j++) {
                if (board[j, i] == PieceType.Empty) {
                    possibleMovements.Add(new Vector2Int(j, i));
                    break;
                }
            }
        }

        return possibleMovements;
    }

    public static int SimulateDrop(PieceType[,] board, int column, bool isPlayersTurn)
    {
        var movements = GetPossibleMovements(board);
        Vector2Int indexes = movements.Find(element => element.y == column);
        board[indexes.x, indexes.y] = isPlayersTurn ? PieceType.Red : PieceType.Blue;
        return column;
    }

    public static int GetRandomMove(PieceType[,] board)
    {
        var movements = GetPossibleMovements(board);
        System.Random r = new System.Random();
        return movements[r.Next(0, movements.Count)].y;
    }

    public static int CheckWin(PieceType[,] board, bool isPlayersTurn)
    {
        for (int i = 0; i < board.GetLength(0); i++) {
            for (int j = 0; j < board.GetLength(1); j++) {
                bool win = CheckWin(board, i, j);
                if (win) {
                    return isPlayersTurn ? 1 : -1;
                }
            }
        }

        return 0;
    }

    public static bool CheckWin(PieceType[,] board, int x, int y)
    {
        //skip empty slots
        PieceType type = board[x, y];
        if (type == PieceType.Empty) {
            return false;
        }

        //prevent out of bounds
        int range = ConnectFourController.piecesToWin - 1;
        bool insideRangeX = x + range < board.GetLength(0);
        bool insideRangeY1 = y + range < board.GetLength(1);
        bool insideRangeY2 = y - range >= 0;

        //win
        bool winHorizontal = insideRangeX;
        bool winVertical = insideRangeY1;
        bool winDiagonal1 = insideRangeX && insideRangeY1;
        bool winDiagonal2 = insideRangeX && insideRangeY2;

        //check conditions
        for (int w = 1; w < ConnectFourController.piecesToWin; w++) {
            winHorizontal = winHorizontal && (type == board[x + w, y]);
            winVertical = winVertical && (type == board[x, y + w]);
            winDiagonal1 = winDiagonal1 && (type == board[x + w, y + w]);
            winDiagonal2 = winDiagonal2 && (type == board[x + w, y - w]);
        }

        //return true if any condition was met
        return winHorizontal || winVertical || winDiagonal1 || winDiagonal2;
    }
}