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
    [SerializeField] protected Color32 redColor;
    [SerializeField] protected Color32 blueColor;
    [SerializeField] protected Transform piecesParent;

    //popups
    [SerializeField] protected GameObject waitForOpponent;
    [SerializeField] protected GameObject drawPopup;
    [SerializeField] protected GameObject redWinsPopup;
    [SerializeField] protected GameObject blueWinsPopup;

    public Field[,] boardField { get; private set; }

    //ref
    public Vector2Int lastField;

    public void DrawBoard(int x, int y)
    {
        boardField = new Field[x, y];

        for (int i = 0; i < x; i++) {
            for (int j = 0; j < y; j++) {
                boardField[i, j] = Instantiate(fieldPrefab, this.transform);
                boardField[i, j].SetIndexes(i, j);
            }
        }
    }

    public void AddPiece(Vector2Int indexes, PieceType type)
    {
        Piece p = Instantiate(piecePrefab, piecesParent);
        p.SetPieceType(type);

        Vector3 targetPosition = boardField[indexes.x, indexes.y].transform.position;
        Vector3 endPosition = new Vector3(targetPosition.x, cursor.transform.position.y, 0);
        p.StartAnimation(endPosition, targetPosition);
    }

    public void ShowPopup(int result)
    {
        Utils.GetGameController().BlockInput();
        HideCursor();

        switch (result) {
            case -1:
                blueWinsPopup.SetActive(true);
                break;

            case 0:
                drawPopup.SetActive(true);
                break;

            case 1:
                redWinsPopup.SetActive(true);
                break;

        }
    }

    public void OpponentPopup(bool show)
    {
        waitForOpponent.SetActive(show);
    }

    public void ShowCursor(bool isRed)
    {
        cursor.color = isRed ? redColor : blueColor;
        cursor.gameObject.SetActive(true);
        boardField[lastField.x, lastField.y].ShowPreview();
    }

    public void HideCursor()
    {
        cursor.gameObject.SetActive(false);
        boardField[lastField.x, lastField.y].HidePreview();
    }

    public void SetCursorPosition(float x)
    {
        cursor.rectTransform.position = new Vector2(x, cursor.rectTransform.position.y);
    }

}