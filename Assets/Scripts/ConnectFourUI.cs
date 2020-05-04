using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class ConnectFourUI : MonoBehaviour
{
    //Prefabs
    [SerializeField] protected Piece piecePrefab;
    [SerializeField] protected Field fieldPrefab;
    
    //Objects
    [SerializeField] protected Image cursor;
    [SerializeField] protected Transform piecesParent;
    [SerializeField] protected TextMeshProUGUI gameOverPopup;
    protected Field[,] boardField;

    //shortcut
    protected int Rows => boardField.GetLength(0);
    protected int Cols => boardField.GetLength(1);

    public void DrawBoard(int x, int y)
    {
        boardField = new Field[x, y];

        for(int i = 0; i < x; i++) {
            for (int j = 0; j < y; j++) {
                boardField[i, j] = Instantiate(fieldPrefab, this.transform);
                boardField[i, j].SetIndexes(i, j);
            }
        }
    }

    public void SpawnPiece(Vector2Int indexes, PieceType type)
    {
        Piece p = Instantiate(piecePrefab, piecesParent);
        p.SetPieceType(type);

        Vector3 targetPosition = boardField[indexes.x, indexes.y].transform.position;
        Vector3 endPosition = new Vector3(targetPosition.x, cursor.transform.position.y, 0);
        p.StartAnimation(endPosition, targetPosition);
    }

    public void SpawnPoup(string text)
    {
        Utils.GetGameController().BlockInput();
        HideCursor();
        gameOverPopup.text = text;
        gameOverPopup.transform.parent.gameObject.SetActive(true);
    }

    #region GET SET
    public Field[,] GetBoard()
    {
        return boardField;
    }

    public void ShowCursor()
    {
        cursor.gameObject.SetActive(true);
    }

    public void HideCursor()
    {
        cursor.gameObject.SetActive(false);
    }

    public void SetCursorPosition(float x)
    {
        cursor.rectTransform.position = new Vector2(x, cursor.rectTransform.position.y);
    }
    #endregion

}