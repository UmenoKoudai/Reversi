using UnityEngine;
using static IColors;

public class Cell : MonoBehaviour
{
    Colors _state = Colors.None;

    public Colors CellColor { get => _state; set => _state = value; }
}
