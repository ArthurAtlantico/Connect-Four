using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : MonoBehaviour
{
    public virtual void Play()
    {
        int choice = Random.Range(0, Utils.GetGameController().GetPossibleMovements().Count);
        Vector2Int indexes = Utils.GetGameController().GetPossibleMovements()[choice];
        Utils.GetGameController().AddPiece(indexes);
    }
}
