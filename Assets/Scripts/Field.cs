using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Field : MonoBehaviour
{
    //UI
    [SerializeField] protected Image previewImage; 

    //data
    protected Vector2Int gridIndex;
    public bool ColumnIsFree => Utils.GetGameController().GetPossibleMovements().Exists(element => element.y == gridIndex.y);
    private Vector2Int FreeIndex => Utils.GetGameController().GetPossibleMovements().Find(element => element.y == gridIndex.y);

    public void SetIndexes(int x, int y)
    {
        gridIndex = new Vector2Int(x, y);
        gameObject.name = "FIELD " + gridIndex;
    }

    public Vector2Int GetIndexes()
    {
        return gridIndex;
    }

    public void MouseEnter()
    {
        if (!Utils.GetGameController().CanPlay) {
            return;
        }

        if (ColumnIsFree) {
            Vector2Int indexes = FreeIndex;
            Utils.GetUIController().GetBoard()[indexes.x, indexes.y].ShowPreview();
            Utils.GetUIController().SetCursorPosition(transform.position.x);
        }
    }

    public void MouseExit()
    {
        if (!Utils.GetGameController().CanPlay) {
            return;
        }

        if (ColumnIsFree) {
            Vector2Int indexes = FreeIndex;
            Utils.GetUIController().GetBoard()[indexes.x, indexes.y].HidePreview();
        }
    }

    public void MouseClick()
    {
        if (!Utils.GetGameController().CanPlay) {
            return;
        }

        if (ColumnIsFree) {
            Vector2Int indexes = FreeIndex;
            Utils.GetUIController().GetBoard()[indexes.x, indexes.y].HidePreview();
            Utils.GetGameController().AddPiece(indexes);
        }
    }

    public void ShowPreview()
    {
        previewImage.gameObject.SetActive(true);
    }

    public void HidePreview()
    {
        previewImage.gameObject.SetActive(false);
    }
}
