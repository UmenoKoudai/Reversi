using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static IColors;

public class Reversi : MonoBehaviour, IPointerClickHandler
{
    /// <summary>セルのオブジェクト</summary>
    [SerializeField] GameObject _cellPrefab;
    /// <summary>駒のオブジェクト</summary>
    [SerializeField] GameObject _piecePrefab;
    /// <summary>盤面のサイズ(縦)</summary>
    [SerializeField] int _row;
    /// <summary>盤面のサイズ(横)</summary>
    [SerializeField] int _column;
    /// <summary>ゲームマネージャー</summary>
    [SerializeField]GameManager _gameManager;
    /// <summary>盤面の情報を格納する配列</summary>
    Cell[,] _cells;
    /// <summary>フィールドの駒情報を格納する配列</summary>
    GameObject[,] _pieces;
    bool _myTurn = true;

    public int Row { get { return _row; } }
    public int Column { get { return _column;} }

    void Start()
    {
        //初期盤面を生成
        GetComponent<GridLayoutGroup>().constraintCount = _column;
        _cells = new Cell[_row, _column];
        _pieces = new GameObject[_row, _column];
        for (int r = 0; r < _row; r++)
        {
            for(int c = 0; c < _column; c++)
            {
                GameObject cell =  Instantiate(_cellPrefab, transform);
                cell.name = $"{r} {c}";
                _cells[r, c] = cell.GetComponent<Cell>();
                if (r == 3 && c == 3 || r == 4 && c == 4)
                {
                    _gameManager.WhiteCount++;
                    GameObject piece = Instantiate(_piecePrefab, cell.transform);
                    piece.transform.localRotation = Quaternion.Euler(90, 0, 0);
                    _cells[r, c].CellColor = Colors.White;
                    _pieces[r, c] = piece;
                }
                if(r == 3 && c == 4 || r == 4 && c == 3)
                {
                    _gameManager.BlackCount++;
                    GameObject piece = Instantiate(_piecePrefab, cell.transform);
                    piece.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                    _cells[r, c].CellColor = Colors.Black;
                    _pieces[r, c] = piece;
                }            
            }
        }
        CostCheck();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Cell currentCell = eventData.pointerCurrentRaycast.gameObject.GetComponent<Cell>();
        if(CheckCell(currentCell, out int r, out int c) && _myTurn)
        {
            List<KeyValuePair<int, int>> piece = new List<KeyValuePair<int, int>>();
            if (_cells[r, c].BlackCost > 0)
            {
                PieceCreate(r, c, Colors.Black);
                ReversCell(ReversCheck(ref piece, Colors.Black, r, c, 0, 1), piece);//右
                ReversCell(ReversCheck(ref piece, Colors.Black, r, c, 0, -1), piece);//左
                ReversCell(ReversCheck(ref piece, Colors.Black, r, c, 1, 0), piece);//下
                ReversCell(ReversCheck(ref piece, Colors.Black, r, c, -1, 0), piece);//上
                ReversCell(ReversCheck(ref piece, Colors.Black, r, c, 1, 1), piece);//右下
                ReversCell(ReversCheck(ref piece, Colors.Black, r, c, -1, -1), piece);//左上
                ReversCell(ReversCheck(ref piece, Colors.Black, r, c, -1, 1), piece);//右上
                ReversCell(ReversCheck(ref piece, Colors.Black, r, c, 1, -1), piece);//左下
                CostCheck();
                StartCoroutine(NextTurn());
            }
        }
    }

    System.Collections.IEnumerator NextTurn()
    {
        yield return new WaitForSeconds(2f);
        EnemyTurn();
        CostCheck();
    }
    void EnemyTurn()
    {
        List<KeyValuePair<int, int>> piece = new List<KeyValuePair<int, int>>();
        int max = int.MinValue;
        int r = 0;
        int c = 0;
        for(int i = 0; i < _row; i++)
        {
            for(int n = 0; n < _column; n++)
            {
                if (_cells[i,n].CellColor == Colors.None&& _cells[i, n].WhiteCost > max)
                {
                    max = _cells[i, n].WhiteCost;
                    r = i;
                    c = n;
                }
            }
        }
        PieceCreate(r, c, Colors.White);
        ReversCell(ReversCheck(ref piece, Colors.White, r, c, 0, 1), piece);//右
        ReversCell(ReversCheck(ref piece, Colors.White, r, c, 0, -1), piece);//左
        ReversCell(ReversCheck(ref piece, Colors.White, r, c, 1, 0), piece);//下
        ReversCell(ReversCheck(ref piece, Colors.White, r, c, -1, 0), piece);//上
        ReversCell(ReversCheck(ref piece, Colors.White, r, c, 1, 1), piece);//右下
        ReversCell(ReversCheck(ref piece, Colors.White, r, c, -1, -1), piece);//左上
        ReversCell(ReversCheck(ref piece, Colors.White, r, c, -1, 1), piece);//右上
        ReversCell(ReversCheck(ref piece, Colors.White, r, c, 1, -1), piece);//左下
    }
    void CostCheck()
    {
        for(int r = 0; r < _row; r++)
        {
            for(int c = 0; c < _column; c++)
            {
                if (_cells[r, c].CellColor == Colors.None)
                {
                    _cells[r, c].WhiteCost = 0;
                    _cells[r, c].BlackCost = 0;
                    CostCount(r, c, 0, 1, Colors.Black);
                    CostCount(r, c, 0, -1, Colors.Black);
                    CostCount(r, c, 1, 0, Colors.Black);
                    CostCount(r, c, -1, 0, Colors.Black);
                    CostCount(r, c, 1, 1, Colors.Black);
                    CostCount(r, c, -1, -1, Colors.Black);
                    CostCount(r, c, -1, 1, Colors.Black);
                    CostCount(r, c, 1, -1, Colors.Black);
                    CostCount(r, c, 0, 1, Colors.White);
                    CostCount(r, c, 0, -1, Colors.White);
                    CostCount(r, c, 1, 0, Colors.White);
                    CostCount(r, c, -1, 0, Colors.White);
                    CostCount(r, c, 1, 1, Colors.White);
                    CostCount(r, c, -1, -1, Colors.White);
                    CostCount(r, c, -1, 1, Colors.White);
                    CostCount(r, c, 1, -1, Colors.White);
                }
            }
        }
    }

    void CostCount(int row, int column, int rowPlus, int columnPlus, Colors searchColor)
    {
        int currentRow = row;
        int currentColumn = column;
        int currentWhiteCount = 0;
        int currentBlackCount = 0;
        row += rowPlus;
        column += columnPlus;
        while (row >= 0 && column >= 0 && row < _row && column < _column)
        {
            if (_cells[row, column].CellColor == Colors.None) break;
            if (searchColor == Colors.White)
            {
                if (_cells[row, column].CellColor == Colors.White)
                {
                    currentBlackCount++;
                    row += rowPlus;
                    column += columnPlus;
                }
                else if (_cells[row, column].CellColor == Colors.Black)
                {
                    _cells[currentRow, currentColumn].BlackCost += currentBlackCount;
                    break;
                }
            }
            else if (searchColor == Colors.Black)
            {
                if (_cells[row, column].CellColor == Colors.Black)
                {
                    currentWhiteCount++;
                    row += rowPlus;
                    column += columnPlus;
                }
                else if (_cells[row, column].CellColor == Colors.White)
                {
                    _cells[currentRow, currentColumn].WhiteCost += currentWhiteCount;
                    break;
                }
            }
        }
    }

    /// <summary>クリックした所に駒を置く</summary>
    /// <param name="row">駒を置く位置(縦)</param>
    /// <param name="column">駒を置く位置(横)</param>
    /// <param name="nowColor">何色の駒を置くのか</param>
    void PieceCreate(int row, int column, Colors nowColor)
    {
        _myTurn = !_myTurn;//置いたらターンを変える
        GameObject piece = Instantiate(_piecePrefab, _cells[row, column].transform);
        if (nowColor == Colors.White)
        {
            _gameManager.WhiteCount++;
            piece.transform.localRotation = Quaternion.Euler(90, 0, 0);
        }
        else if(nowColor == Colors.Black)
        {
            _gameManager.BlackCount++;
            piece.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        }
        _cells[row, column].CellColor = nowColor;
        _pieces[row, column] = piece;
    }

    /// <summary>上下左右斜めを見てリバース出来るかチェックする</summary>
    /// <param name="pieces">リバース出来る駒の位置</param>
    /// <param name="nowColor">今置いた駒の色</param>
    /// <param name="row">置いた位置(縦)</param>
    /// <param name="column">置いた位置(横)</param>
    /// <param name="rowPlus">上下に移動</param>
    /// <param name="columnPlus">左右に移動</param>
    /// <returns></returns>
    Colors ReversCheck(ref List<KeyValuePair<int, int>> pieces, Colors nowColor, int row, int column, int rowPlus, int columnPlus)
    {
        Colors changeColor = Colors.None;
        List<KeyValuePair<int, int>> currentReversPiece = new List<KeyValuePair<int, int>>();
        row += rowPlus;
        column += columnPlus;
        while (row >= 0 && column >= 0 && row < _row && column < _column)
        {
            if (_cells[row, column].CellColor == Colors.None) 
            {
                changeColor = Colors.None;
                break;
            }
            if (nowColor == Colors.White)
            {
                if (_cells[row, column].CellColor == Colors.Black)
                {
                    currentReversPiece.Add(new KeyValuePair<int, int>(row, column));
                    row += rowPlus;
                    column += columnPlus;
                }
                else if (_cells[row, column].CellColor == Colors.White)
                {
                    pieces = currentReversPiece;
                    changeColor = Colors.White;
                    break;
                }
            }
            else if (nowColor == Colors.Black)
            {
                if (_cells[row, column].CellColor == Colors.White)
                {
                    currentReversPiece.Add(new KeyValuePair<int, int>(row, column));
                    row += rowPlus;
                    column += columnPlus;
                }
                else if (_cells[row, column].CellColor == Colors.Black)
                {
                    pieces = currentReversPiece;
                    changeColor = Colors.Black;
                    break;
                }
            }
        }
        return changeColor;
    }

    /// <summary>挟まれた駒をリバースする</summary>
    /// <param name="reversColor">何色にリバースするか</param>
    /// <param name="pieces">リバースする駒の位置</param>
    void ReversCell(Colors reversColor, List<KeyValuePair<int, int>> pieces)
    {
        if (reversColor != Colors.None)
        {
            foreach (var p in pieces)
            {
                _cells[p.Key, p.Value].CellColor = reversColor;
                if (reversColor == Colors.White)
                {
                    _gameManager.BlackCount--;
                    _gameManager.WhiteCount++;
                    _pieces[p.Key, p.Value].transform.DORotate(new Vector3(1, 0, 0) * 90f, 1f);
                }
                else if(reversColor == Colors.Black)
                {
                    _gameManager.WhiteCount--;
                    _gameManager.BlackCount++;
                    _pieces[p.Key, p.Value].transform.DORotate(new Vector3(1, 0, 0) * -90f, 1f);
                }
            }
        }
    }

    /// <summary>どこのセルをクリックしたかをチェックする</summary>
    /// <param name="currentCell">クリックしたセル</param>
    /// <param name="row">縦の位置情報を返す</param>
    /// <param name="column">横の位置情報を返す</param>
    /// <returns></returns>
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
