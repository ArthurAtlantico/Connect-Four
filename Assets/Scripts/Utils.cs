using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    private static ConnectFourController _controller;
    public static ConnectFourController GetGameController()
    {
        if(_controller == null) {
            _controller = FindObjectOfType<ConnectFourController>();
        }

        return _controller;
    }

    private static ConnectFourUI _ui;
    public static ConnectFourUI GetUIController()
    {
        if (_ui == null) {
            _ui = FindObjectOfType<ConnectFourUI>();
        }

        return _ui;
    }
}

[System.Serializable]
public enum PieceType
{
    Empty = 0,
    Blue = 1,
    Red = 2
}