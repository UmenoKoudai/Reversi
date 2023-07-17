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
    Piece[,] _pieces;
    bool _myTurn = true;

    void Start()
    {
        GetComponent<GridLayoutGroup>().constraintCount = _column;
        _cells = new Cell[_row, _column];
        _pieces = new Piece[_row, _column];
        for (int r = 0; r < _row; r++)
        {
            for(int c = 0; c < _column; c++)
            {
                GameObject cell =  Instantiate(_cellPrefab, transform);
                cell.name = $"{r} {c}";
                _cells[r, c] = cell.GetComponent<Cell>();
                if (r == 3 && c == 3 || r == 4 && c == 4)
                {
                    GameObject piece = Instantiate(_piecePrefab, cell.transform);
                    piece.GetComponent<Piece>().PieceColor = Colors.White;
                    _cells[r, c].CellColor = Colors.White;
                    _pieces[r, c] = piece.GetComponent<Piece>();
                }
                if(r == 3 && c == 4 || r == 4 && c == 3)
                {
                    GameObject piece = Instantiate(_piecePrefab, cell.transform);
                    piece.GetComponent<Piece>().PieceColor = Colors.Black;
                    _cells[r, c].CellColor = Colors.Black;
                    _pieces[r, c] = piece.GetComponent<Piece>();
                }            
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Cell currentCell = eventData.pointerCurrentRaycast.gameObject.GetComponent<Cell>();
        if(CheckCell(currentCell, out int r, out int c))
        {
            List<KeyValuePair<int, int>> piece = new List<KeyValuePair<int, int>>();
            if (_myTurn)
            {
                PieceCreate(r, c, Colors.Black);
                ReversCell(ReversCheckRight(r, c + 1, ref piece, Colors.Black), piece);
                ReversCell(ReversCheckLeft(r, c - 1, ref piece, Colors.Black), piece);
                ReversCell(ReversCheckUp(r - 1, c, ref piece, Colors.Black), piece);
                ReversCell(ReversCheckDown(r + 1, c, ref piece, Colors.Black), piece);
            }
            else
            {
                PieceCreate(r, c, Colors.White);
                ReversCell(ReversCheckRight(r, c + 1, ref piece, Colors.White), piece);
                ReversCell(ReversCheckLeft(r, c - 1, ref piece, Colors.White), piece);
                ReversCell(ReversCheckUp(r - 1, c, ref piece, Colors.White), piece);
                ReversCell(ReversCheckDown(r + 1, c, ref piece, Colors.White), piece);
            }
        }
    }

    void PieceCreate(int row, int column, Colors nowColor)
    {

        _myTurn = !_myTurn;
        GameObject piece = Instantiate(_piecePrefab, _cells[row, column].transform);
        piece.GetComponent<Piece>().PieceColor = nowColor;
        _cells[row, column].CellColor = nowColor;
        _pieces[row, column] = piece.GetComponent<Piece>();
    }

    Colors ReversCheckRight(int row, int column, ref List<KeyValuePair<int, int>> pieces, Colors nowColor)
    {
        Colors changeColor = Colors.None;
        while(row >= 0 && column >= 0 && row < _row && column < _column)
        {
            if (nowColor == Colors.White)
            {
                if (_cells[row, column].CellColor == Colors.Black)
                {
                    pieces.Add(new KeyValuePair<int, int>(row, column));
                    column++;
                }
                else if(_cells[row, column].CellColor == Colors.White)
                {
                    changeColor = Colors.White;
                    break;
                }
            }
            else if(nowColor == Colors.Black)
            {
                if (_cells[row, column].CellColor == Colors.White)
                {
                    pieces.Add(new KeyValuePair<int, int>(row, column));
                    column++;
                }
                else if (_cells[row, column].CellColor == Colors.Black)
                {
                    changeColor = Colors.Black;
                    break;
                }
            }
        }
        return changeColor;
    }

    Colors ReversCheckLeft(int row, int column, ref List<KeyValuePair<int, int>> pieces, Colors nowColor)
    {
        Colors changeColor = Colors.None;
        while (row >= 0 && column >= 0 && row < _row && column < _column)
        {
            if (nowColor == Colors.White)
            {
                if (_cells[row, column].CellColor == Colors.Black)
                {
                    pieces.Add(new KeyValuePair<int, int>(row, column));
                    column--;
                }
                else if (_cells[row, column].CellColor == Colors.White)
                {
                    changeColor = Colors.White;
                    break;
                }
            }
            else if (nowColor == Colors.Black)
            {
                if (_cells[row, column].CellColor == Colors.White)
                {
                    pieces.Add(new KeyValuePair<int, int>(row, column));
                    column--;
                }
                else if (_cells[row, column].CellColor == Colors.Black)
                {
                    changeColor = Colors.Black;
                    break;
                }
            }
        }
        return changeColor;
    }

    Colors ReversCheckUp(int row, int column, ref List<KeyValuePair<int, int>> pieces, Colors nowColor)
    {
        Colors changeColor = Colors.None;
        while (row >= 0 && column >= 0 && row < _row && column < _column)
        {
            if (nowColor == Colors.White)
            {
                if (_cells[row, column].CellColor == Colors.Black)
                {
                    pieces.Add(new KeyValuePair<int, int>(row, column));
                    row--;
                }
                else if (_cells[row, column].CellColor == Colors.White)
                {
                    changeColor = Colors.White;
                    break;
                }
            }
            else if (nowColor == Colors.Black)
            {
                if (_cells[row, column].CellColor == Colors.White)
                {
                    pieces.Add(new KeyValuePair<int, int>(row, column));
                    row--;
                }
                else if (_cells[row, column].CellColor == Colors.Black)
                {
                    changeColor = Colors.Black;
                    break;
                }
            }
        }
        return changeColor;
    }

    Colors ReversCheckDown(int row, int column, ref List<KeyValuePair<int, int>> pieces, Colors nowColor)
    {
        Colors changeColor = Colors.None;
        while (row >= 0 && column >= 0 && row < _row && column < _column)
        {
            if (nowColor == Colors.White)
            {
                if (_cells[row, column].CellColor == Colors.Black)
                {
                    pieces.Add(new KeyValuePair<int, int>(row, column));
                    row++;
                }
                else if (_cells[row, column].CellColor == Colors.White)
                {
                    changeColor = Colors.White;
                    break;
                }
            }
            else if (nowColor == Colors.Black)
            {
                if (_cells[row, column].CellColor == Colors.White)
                {
                    pieces.Add(new KeyValuePair<int, int>(row, column));
                    row++;
                }
                else if (_cells[row, column].CellColor == Colors.Black)
                {
                    changeColor = Colors.Black;
                    break;
                }
            }
        }
        return changeColor;
    }

    void ReversCell(Colors reversColor, List<KeyValuePair<int, int>> pieces)
    {
        if (reversColor != Colors.None)
        {
            foreach (var p in pieces)
            {
                _cells[p.Key, p.Value].CellColor = reversColor;
                _pieces[p.Key, p.Value].PieceColor = reversColor;
            }
        }
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
