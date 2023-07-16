using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    CellState _state = CellState.None;

    public CellState State { get => _state; set => _state = value; }

    public enum CellState
    {
        None,
        White,
        Black,
    }
}
