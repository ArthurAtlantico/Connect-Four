using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Field : MonoBehaviour
{
    //UI
    [SerializeField] protected Color defaultColor;
    [SerializeField] protected Color previewColor;
    private Image _myImage;

    //data
    protected Vector2Int gridIndex;
    public bool ColumnIsFree => Utils.GetGameController().possibleMovements.Exists(element => element.y == gridIndex.y);
 
    private void Awake()
    {
        _myImage = GetComponent<Image>();
    }

    public void SetIndexes(int x, int y)
    {
        gridIndex = new Vector2Int(x, y);
        gameObject.name = "FIELD " + gridIndex;
    }

    public void MouseEnter()
    {
        Utils.GetUIController().lastField = gridIndex;

        if (!Utils.GetGameController().CanPlay) {
            return;
        }

        ShowPreview();
    }

    public void MouseExit()
    {
        if (!Utils.GetGameController().CanPlay) {
            return;
        }

        HidePreview();
    }

    public void MouseClick()
    {
        if (!Utils.GetGameController().CanPlay) {
            return;
        }

        if (ColumnIsFree) {
            Vector2Int indexes = Utils.GetGameController().possibleMovements.Find(element => element.y == gridIndex.y);
            Utils.GetGameController().AddPiece(indexes);
        }
    }

    public void ShowPreview()
    {
        if (!ColumnIsFree) {
            return;
        }

        Utils.GetUIController().SetCursorPosition(transform.position.x);

       foreach(Field f in Utils.GetUIController().boardField) {
            if(f.gridIndex.y == this.gridIndex.y) {
                f.SetColor(previewColor);
            }
        }
    }

    public void HidePreview()
    {
        foreach (Field f in Utils.GetUIController().boardField) {
            if (f.gridIndex.y == this.gridIndex.y) {
                f.SetColor(defaultColor);
            }
        }
    }

    protected void SetColor(Color c)
    {
        _myImage.color = c;
    }
}
