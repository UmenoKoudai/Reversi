using UnityEngine;
using static IColors;

public class Piece : MonoBehaviour
{
    Colors _state = Colors.Black;

    public Colors PieceColor { get => _state; set => _state = value; }

    void Update()
    {
        if(_state == Colors.White)
        {
            transform.localRotation = Quaternion.Euler(90, 0, 0);
        }
        if(_state == Colors.Black)
        {
            transform.localRotation = Quaternion.Euler(-90, 0, 0);
        }
    }
}
