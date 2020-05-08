using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectFourController : MonoBehaviour
{
    //opponent
    [SerializeField] protected Opponent opponent;
    public bool IsHumanOpponent => opponent == null;

    //game
    protected int turnCount;
    protected Vector2Int boardSize = new Vector2Int(6, 7); //rows x cols
    public int NumberOfRows => boardSize.x;
    public int NumberOfColumns => boardSize.y;
    public PieceType[,] board { get; private set; }
    public List<Vector2Int> possibleMovements { get; private set; }
    protected bool IsDraw => possibleMovements.Count == 0;
    public PieceType CurrentType => _isPlayersTurn ? PieceType.Red : PieceType.Blue;

    //flags
    private bool _isPlayersTurn = false;

    private bool _blockInput = false;
    public bool CanPlay => !_blockInput;
    public bool IsFirstTurn => turnCount == 0;
    
    //const
    public const int piecesToWin = 4;
    
    private void Awake()
    {
        //random player starts
        _isPlayersTurn = (Random.Range(0,2) == 1);

        //start and draw board
        board = new PieceType[boardSize.x, boardSize.y];
        SetPossibleMovements();
        
        Utils.GetUIController().DrawBoard(boardSize.x, boardSize.y);
        Invoke("UpdateState", 0.3f); //wait a bit for the UI
    }

    public void AddPiece(Vector2Int indexes)
    {
        turnCount++;
        board[indexes.x, indexes.y] = CurrentType;
        Utils.GetUIController().AddPiece(indexes, CurrentType);
    }

    public void UpdateState()
    {
        //is the board full?
        SetPossibleMovements();
        if (IsDraw) {
            Utils.GetUIController().ShowPopup(0);
            return;
        }

        //did someone win?
        int result = CheckGameOver();
        if (result == 0) {
            _isPlayersTurn = !_isPlayersTurn;

            if (IsHumanOpponent) {
                Utils.GetUIController().ShowCursor(_isPlayersTurn);
            }
            else {
                if (_isPlayersTurn) {
                    AllowInput();
                }
                else {
                    BlockInput();
                    opponent.Play();
                }
            }

            return;
        }

        //end
        Utils.GetUIController().ShowPopup(result);
    }

    public int CheckGameOver()
    {
        return Simulator.CheckWin(board, _isPlayersTurn);
    }

    public void SetPossibleMovements()
    {
        possibleMovements = Simulator.GetPossibleMovements(board);
    }

    public void BlockInput()
    {
        Utils.GetUIController().HideCursor();
        _blockInput = true;
    }

    public void AllowInput()
    {
        Utils.GetUIController().ShowCursor(_isPlayersTurn);
        _blockInput = false;
    }
}