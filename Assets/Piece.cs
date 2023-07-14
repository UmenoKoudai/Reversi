using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour
{
    PieceColor _nowColor = PieceColor.Black;

    public PieceColor NowColor { get => _nowColor; set => _nowColor = value; }

    void Update()
    {
        if(_nowColor == PieceColor.White)
        {
            transform.localRotation = Quaternion.Euler(90, 0, 0);
        }
        if(_nowColor == PieceColor.Black)
        {
            transform.localRotation = Quaternion.Euler(-90, 0, 0);
        }
    }
    public enum PieceColor
    {
        White,
        Black,
    }
}
