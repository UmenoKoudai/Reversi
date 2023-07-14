using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reversi : MonoBehaviour
{
    [SerializeField] GameObject _cellPrefab;
    [SerializeField] GameObject _piecePrefab;
    [SerializeField] int _row;
    [SerializeField]int _column;
    GameObject[,] _cells;
    void Start()
    {
        GetComponent<GridLayoutGroup>().constraintCount = _column;
        _cells = new GameObject[_row, _column];
        for(int r = 0; r < _row; r++)
        {
            for(int c = 0; c < _column; c++)
            {
                GameObject cell =  Instantiate(_cellPrefab, transform);
                if (r == 3 && c == 3 || r == 4 && c == 4)
                {
                    GameObject piece = Instantiate(_piecePrefab, cell.transform);
                    piece.transform.position = new Vector3(-13, 16, -2);
                    piece.GetComponent<Piece>().NowColor = Piece.PieceColor.White;
                }
                if(r == 3 && c == 4 || r == 4 && c == 3)
                {
                    GameObject piece = Instantiate(_piecePrefab, cell.transform);
                    piece.transform.position = new Vector3(-13, 16, -2);
                    piece.GetComponent<Piece>().NowColor = Piece.PieceColor.Black;
                }
                cell.name = $"{r} {c}";
                _cells[r, c] = cell;
            }
        }
    }

    void Update()
    {
        
    }
}
