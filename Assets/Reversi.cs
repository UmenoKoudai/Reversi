using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static IColors;

public class Reversi : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject _cellPrefab;
    [SerializeField] GameObject _piecePrefab;
    [SerializeField] int _row;
    [SerializeField]int _column;
    Cell[,] _cells;
    bool _myTurn = true;

    void Start()
    {
        GetComponent<GridLayoutGroup>().constraintCount = _column;
        _cells = new Cell[_row, _column];
        for(int r = 0; r < _row; r++)
        {
            for(int c = 0; c < _column; c++)
            {
                GameObject cell =  Instantiate(_cellPrefab, transform);
                cell.name = $"{r} {c}";
                _cells[r, c] = cell.GetComponent<Cell>();
                if (r == 3 && c == 3 || r == 4 && c == 4)
                {
                    GameObject piece = Instantiate(_piecePrefab, cell.transform);
                    //piece.transform.position = new Vector3(-13, 16, -2);
                    piece.GetComponent<Piece>().PieceColor = Colors.White;
                    _cells[r, c].CellColor = Colors.White;
                }
                if(r == 3 && c == 4 || r == 4 && c == 3)
                {
                    GameObject piece = Instantiate(_piecePrefab, cell.transform);
                    //piece.transform.position = new Vector3(-13, 16, -2);
                    piece.GetComponent<Piece>().PieceColor = Colors.Black;
                    _cells[r, c].CellColor = Colors.Black;
                }            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Cell currentCell = eventData.pointerCurrentRaycast.gameObject.GetComponent<Cell>();
        if(CheckCell(currentCell, out int r, out int c))
        {
            if (_myTurn)
            {
                PieceCreate(r, c, Colors.Black);
            }
            else
            {
                PieceCreate(r, c, Colors.White);
            }
        }
    }

    void PieceCreate(int row, int column, Colors nowColor)
    {

        _myTurn = !_myTurn;
        GameObject piece = Instantiate(_piecePrefab, _cells[row, column].transform);
        piece.GetComponent<Piece>().PieceColor = nowColor;
        _cells[row, column].CellColor = nowColor;
    }

    bool CheckCell(Cell currentCell, out int row, out int column)
    {
        for(int r = 0; r < _row; r++)
        {
            for (int c = 0; c < _column; c++)
            {
                if (_cells[r, c] == currentCell && _cells[r, c].CellColor == Colors.None)
                {
                    row = r;
                    column = c;
                    return true;
                }
            }
        }
        row = 0; column = 0;
        return false;
    }
}
