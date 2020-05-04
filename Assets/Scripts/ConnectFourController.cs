using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectFourController : MonoBehaviour
{
	//opponent
	[SerializeField] protected Opponent opponent;

	//game
	protected Vector2Int boardSize = new Vector2Int(6, 7); //rows x cols
    protected PieceType[,] board;
	protected List<Vector2Int> possibleMovements;

	//flags
    private bool _isPlayersTurn = false;
    private bool _blockInput = false;
	public bool CanPlay => _isPlayersTurn && !_blockInput;

	//const
	public const int piecesToWin = 4;

	private void Awake()
	{
		//start and draw board
		board = new PieceType[boardSize.x, boardSize.y];
		Utils.GetUIController().DrawBoard(boardSize.x, boardSize.y);
		UpdateState();
	}

	public void AddPiece(Vector2Int indexes)
	{
		board[indexes.x, indexes.y] = _isPlayersTurn ? PieceType.Red : PieceType.Blue;
		Utils.GetUIController().SpawnPiece(indexes, _isPlayersTurn ? PieceType.Red : PieceType.Blue);
	}

	public void UpdateState()
	{
		//is the board full?
		SetPossibleMovements();
		if(possibleMovements.Count == 0) {
			Utils.GetUIController().SpawnPoup("Draw!");
			return;
		}

		//did someone win?
		int result = CheckGameOver();
		if (result == 0) {
			_isPlayersTurn = !_isPlayersTurn;

			if (_isPlayersTurn) {
				AllowInput();
			}
			else{
				BlockInput();
				opponent.Play();
			}
			
			return;
		}

		//end
		Utils.GetUIController().SpawnPoup(_isPlayersTurn ? "You win!" : "You lose!");
	}

	public int CheckGameOver()
	{
		for(int i = 0; i < boardSize.x; i++) {
			for (int j = 0; j < boardSize.y; j++) {
				bool win = CheckWin(i, j);
				if (win) {
					return _isPlayersTurn ? 1 : -1;
				}
			}
		}

		return 0;
	}

	public bool CheckWin(int x, int y)
	{
		//skip empty slots
		PieceType type = board[x, y];
		if (type == PieceType.Empty) {
			return false;
		}

		//prevent out of bounds
		int range = piecesToWin - 1;
		bool insideRangeX = x + range < boardSize.x;
		bool insideRangeY1 = y + range < boardSize.y;
		bool insideRangeY2 = y - range >= 0;

		//win
		bool winHorizontal = insideRangeX;
		bool winVertical = insideRangeY1;
		bool winDiagonal1 = insideRangeX && insideRangeY1;
		bool winDiagonal2 = insideRangeX && insideRangeY2;

		//check consecutive slots for conditions
		for (int w = 1; w < piecesToWin; w++) {
			winHorizontal = winHorizontal && (type == board[x + w, y]);
			winVertical = winVertical && (type == board[x, y + w]);
			winDiagonal1 = winDiagonal1 && (type == board[x + w, y + w]);
			winDiagonal2 = winDiagonal2 && (type == board[x + w, y - w]);
		}

		//return true if any condition was met
		return winHorizontal || winVertical || winDiagonal1 || winDiagonal2;
	}

	#region GET SET
	public PieceType[,] GetBoard()
	{
		return board;
	}

	public void SetPossibleMovements()
	{
		possibleMovements = new List<Vector2Int>();

		//finds the first empty slot in each column
		for (int i = 0; i < boardSize.y; i++) {
			for (int j = 0; j < boardSize.x; j++) {
				if (board[j, i] == PieceType.Empty) {
					possibleMovements.Add(new Vector2Int(j, i));
					break;
				}
			}
		}
	}

	public List<Vector2Int> GetPossibleMovements()
	{
		return possibleMovements;
	}

	public void BlockInput()
	{
		Utils.GetUIController().HideCursor();
		_blockInput = true;
	}

	public void AllowInput()
	{
		Utils.GetUIController().ShowCursor();
		_blockInput = false;
	}
    #endregion
}