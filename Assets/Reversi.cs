using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] GameManager _gameManager;
    /// <summary>スキップ演出をするオブジェクト</summary>
    [SerializeField] Skip _skip;
    /// <summary>盤面の情報を格納する配列</summary>
    Dictionary<string, Cell> _fieldData = new Dictionary<string, Cell>();
    /// <summary>フィールドの駒情報を格納する配列</summary>
    Dictionary<string, GameObject> _pieceData = new Dictionary<string, GameObject>();
    /// <summary>8方向を調べるための数字</summary>
    Position[] direction = {
        new Position(1,0),
        new Position(-1,0),
        new Position(0,1),
        new Position(0,-1),
        new Position(1,1),
        new Position(-1,-1),
        new Position(1,-1),
        new Position(-1,1),
    };
    /// <summary>現在のターン情報を管理するフラグ</summary>
    public bool _myTurn = true;


    public int Row { get { return _row; } }
    public int Column { get { return _column; } }
    public bool Turn
    {
        get => _myTurn;
        set
        {
            _myTurn = value;
        }
    }

    void Start()
    {
        //初期盤面を生成
        GetComponent<GridLayoutGroup>().constraintCount = _column;
        char columnLine = 'A';
        for (int r = 0; r < _row; r++)
        {
            char rowLine = '1';
            for (int c = 0; c < _column; c++)
            {
                
                string id = $"{columnLine}{rowLine}";
                GameObject cell = Instantiate(_cellPrefab, transform);
                cell.name = $"{columnLine}{rowLine}";
                //_cells[r, c] = cell.GetComponent<Cell>();
                _fieldData.Add(id, cell.GetComponent<Cell>());
                GameObject piece = Instantiate(_piecePrefab, cell.transform);
                piece.SetActive(false);
                _pieceData[id] = piece;
                if (r == 3 && c == 3 || r == 4 && c == 4)
                {
                    _gameManager.WhiteCount++;
                    piece.SetActive(true);
                    piece.transform.localRotation = Quaternion.Euler(90, 0, 0);
                    _fieldData[id].CellColor = Colors.White;
                }
                if (r == 3 && c == 4 || r == 4 && c == 3)
                {
                    _gameManager.BlackCount++;
                    piece.SetActive(true);
                    piece.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                    _fieldData[id].CellColor = Colors.Black;
                }
                rowLine++;
            }
            columnLine++;
        }
        CostCheck();
        //RecodeSave();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Cell currentCell = eventData.pointerCurrentRaycast.gameObject.GetComponent<Cell>();
        if (CheckCell(currentCell, out char r, out char c) && _myTurn)
        {
            List<KeyValuePair<int, int>> piece = new List<KeyValuePair<int, int>>();
            if (_fieldData[$"{c}{r}"].BlackCost > 0)
            {
                PieceCreate($"{c}{r}", Colors.Black);
                foreach (var dir in direction)
                {
                    StartCoroutine(Revers(ReversCheck(ref piece, Colors.Black, r, c, dir.x, dir.y), piece));
                }
                StartCoroutine(NextTurn());
                //RecodeSave();
            }
        }
    }
    void PlayerCheck()
    {
        foreach (var cell in _fieldData)
        {
            if (cell.Value.BlackCost > 0)
            {
                return;
            }
        }
        StartCoroutine(_skip.Play());
    }
    /// <summary>時間を送らせてAIのターンを実行する</summary>
    IEnumerator NextTurn()
    {
        yield return new WaitForSeconds(2f);
        EnemyTurn();
    }
    /// <summary>AIのターンを行う</summary>
    void EnemyTurn()
    {
        List<KeyValuePair<int, int>> piece = new List<KeyValuePair<int, int>>();
        int max = int.MinValue;
        string selectId = "";
        char r = '1';
        char c = 'A';
        char columnLine = 'A';
        for (int i = 0; i < _row; i++)
        {
            char rowLine = '1';
            for (int n = 0; n < _column; n++)
            {
                
                string id = $"{columnLine}{rowLine}";
                if (_fieldData[id].CellColor == Colors.None && _fieldData[id].WhiteCost > max)
                {
                    max = _fieldData[id].WhiteCost;
                    selectId = id;
                    r = rowLine; 
                    c = columnLine;
                }
                rowLine++;
            }
            columnLine++;
        }
        if (max == 0)
        {
            StartCoroutine(_skip.Play());
            _myTurn = !_myTurn;
        }
        else
        {
            PieceCreate(selectId, Colors.White);
            foreach (var dir in direction)
            {
                StartCoroutine(Revers(ReversCheck(ref piece, Colors.White, r, c, dir.x, dir.y), piece));
            }
        }
        PlayerCheck();
        //RecodeSave();
    }
    /// <summary>駒を置いた場合ひっくり返す事ができる枚数を計算</summary>
    void CostCheck()
    {
        char columnLine = 'A';
        for (int r = 0; r < _row; r++)
        {
            char rowLine = '1';
            for (int c = 0; c < _column; c++)
            {
                
                string id = $"{columnLine}{rowLine}";
                if (_fieldData[id].CellColor == Colors.None)
                {
                    _fieldData[id].WhiteCost = 0;
                    _fieldData[id].BlackCost = 0;
                    foreach (Colors color in Enum.GetValues(typeof(Colors)))
                    {
                        if (color == Colors.None) continue;
                        foreach (var dir in direction)
                        {
                            CostCount(rowLine, columnLine, dir.x, dir.y, color);
                        }
                    }
                }
            }
        }
    }

    //void RecodeSave()
    //{
    //    Cell[,] recode = new Cell[_row, _column];
    //    for (int r = 0; r < _row; r++)
    //    {
    //        for (int c = 0; c < _column; c++)
    //        {
    //            Cell currentCell = _cells[r, c];
    //            recode[r, c] = new Cell(currentCell.CellColor, currentCell.BlackCost, currentCell.WhiteCost);
    //        }
    //    }
    //    _gameRecode.Add(recode);
    //    _recodeIndex = _gameRecode.Count;
    //}
    /// <summary>実際に枚数を計算するメソッド</summary>
    /// <param name="row">計算するマス(縦)</param>
    /// <param name="column">計算するマス(横)</param>
    /// <param name="rowPlus">上下に移動する</param>
    /// <param name="columnPlus">左右に移動する</param>
    /// <param name="searchColor">どの色の駒を計算するか</param>
    void CostCount(char row, char column, int rowPlus, int columnPlus, Colors searchColor)
    {
        string currentId = $"{column}{row}";
        int currentRow = row;
        int currentColumn = column;
        row = (char)(currentRow + rowPlus);
        column = (char)(currentColumn + columnPlus);
        int currentWhiteCount = 0;
        int currentBlackCount = 0;
        string id = $"{column}{row}";
        while (_fieldData.Keys.Contains(id))
        {
            id = $"{column}{row}";
            if (_fieldData[id].CellColor == Colors.None) break;
            if (searchColor == Colors.White)
            {
                if (_fieldData[id].CellColor == Colors.White)
                {
                    currentBlackCount++;
                    currentRow = row;
                    currentColumn = column;
                    row = (char)(currentRow + rowPlus);
                    column = (char)(currentColumn + columnPlus);
                }
                else if (_fieldData[id].CellColor == Colors.Black)
                {
                    _fieldData[currentId].BlackCost += currentBlackCount;
                    break;
                }
            }
            else if (searchColor == Colors.Black)
            {
                if (_fieldData[id].CellColor == Colors.Black)
                {
                    currentWhiteCount++;
                    currentRow = row;
                    currentColumn = column;
                    row = (char)(currentRow + rowPlus);
                    column = (char)(currentColumn + columnPlus);
                }
                else if (_fieldData[id].CellColor == Colors.White)
                {
                    _fieldData[currentId].WhiteCost += currentWhiteCount;
                    break;
                }
            }
        }
    }
    /// <summary>クリックした所に駒を置く</summary>
    /// <param name="row">駒を置く位置(縦)</param>
    /// <param name="column">駒を置く位置(横)</param>
    /// <param name="nowColor">何色の駒を置くのか</param>
    void PieceCreate(string id, Colors nowColor)
    {
        _myTurn = !_myTurn;//置いたらターンを変える
        GameObject piece = _pieceData[id];
        if (nowColor == Colors.White)
        {
            _gameManager.WhiteCount++;
            piece.SetActive(true);
            piece.transform.localRotation = Quaternion.Euler(90, 0, 0);
        }
        else if (nowColor == Colors.Black)
        {
            _gameManager.BlackCount++;
            piece.SetActive(true);
            piece.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        }
        _fieldData[id].CellColor = nowColor;
        _pieceData[id] = piece;
    }
    /// <summary>上下左右斜めを見てリバース出来るかチェックする</summary>
    /// <param name="pieces">リバース出来る駒の位置</param>
    /// <param name="nowColor">今置いた駒の色</param>
    /// <param name="row">置いた位置(縦)</param>
    /// <param name="column">置いた位置(横)</param>
    /// <param name="rowPlus">上下に移動</param>
    /// <param name="columnPlus">左右に移動</param>
    /// <returns></returns>
    Colors ReversCheck(ref List<KeyValuePair<int, int>> pieces, Colors nowColor, char row, char column, int rowPlus, int columnPlus)
    {
        Colors changeColor = Colors.None;
        List<KeyValuePair<int, int>> currentReversPiece = new List<KeyValuePair<int, int>>();
        int r = row;
        row = (char)(r + rowPlus);
        int c = column;
        column = (char)(c + columnPlus);
        while (row >= 0 && column >= 0 && row < _row && column < _column)
        {
            string id = $"{column}{row}";
            if (_fieldData[id].CellColor == Colors.None)
            {
                changeColor = Colors.None;
                break;
            }
            if (nowColor == Colors.White)
            {
                if (_fieldData[id].CellColor == Colors.Black)
                {
                    currentReversPiece.Add(new KeyValuePair<int, int>(row, column));
                    int r2 = row;
                    row = (char)(r2 + rowPlus);
                    int c2 = column;
                    column = (char)(c2 + columnPlus);
                }
                else if (_fieldData[id].CellColor == Colors.White)
                {
                    pieces = currentReversPiece;
                    changeColor = Colors.White;
                    break;
                }
            }
            else if (nowColor == Colors.Black)
            {
                if (_fieldData[id].CellColor == Colors.White)
                {
                    currentReversPiece.Add(new KeyValuePair<int, int>(row, column));
                    int r2 = row;
                    row = (char)(r2 + rowPlus);
                    int c2 = column;
                    column = (char)(c2 + columnPlus);
                }
                else if (_fieldData[id].CellColor == Colors.Black)
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
    IEnumerator Revers(Colors reversColor, List<KeyValuePair<int, int>> pieces)
    {
        if (reversColor != Colors.None)
        {
            foreach (var p in pieces)
            {
                string id = $"{p.Key}{p.Value}";
                _fieldData[id].CellColor = reversColor;
                if (reversColor == Colors.White)
                {
                    _gameManager.BlackCount--;
                    _gameManager.WhiteCount++;
                    _pieceData[id].transform.DORotate(new Vector3(1, 0, 0) * 90f, 1f);
                }
                else if (reversColor == Colors.Black)
                {
                    _gameManager.WhiteCount--;
                    _gameManager.BlackCount++;
                    _pieceData[id].transform.DORotate(new Vector3(1, 0, 0) * -90f, 1f);
                }
                yield return new WaitForSeconds(0.5f);
            }
            CostCheck();
        }
    }
    /// <summary>どこのセルをクリックしたかをチェックする</summary>
    /// <param name="currentCell">クリックしたセル</param>
    /// <param name="row">縦の位置情報を返す</param>
    /// <param name="column">横の位置情報を返す</param>
    /// <returns></returns>
    bool CheckCell(Cell currentCell, out char row, out char column)
    {
        char columnLine = 'A';
        for (int r = 0; r < _row; r++)
        {
            char rowLine = '1';
            for (int c = 0; c < _column; c++)
            {
                
                string id = $"{columnLine}{rowLine}";
                if (_fieldData[id] == currentCell && _fieldData[id].CellColor == Colors.None)
                {
                    row = rowLine;
                    column = columnLine;
                    return true;
                }
            }
        }
        row = '0'; column = '0';
        return false;
    }
    //public void FieldChange(int n)
    //{
    //    int tmp = _recodeIndex;
    //    _recodeIndex += n;
    //    if (_recodeIndex > 0 && _recodeIndex < _gameRecode.Count)
    //    {
    //        var cell = _gameRecode[_recodeIndex - 1];
    //        _gameManager.WhiteCount = 0;
    //        _gameManager.BlackCount = 0;
    //        for (int r = 0; r < _row; r++)
    //        {
    //            for (int c = 0; c < _column; c++)
    //            {
    //                GameObject piece = _pieces[r, c];
    //                if(cell[r, c].CellColor == Colors.Black)
    //                {
    //                    _gameManager.BlackCount++;
    //                    piece.SetActive(true);
    //                    piece.transform.localRotation = Quaternion.Euler(-90, 0, 0);
    //                }
    //                else if (cell[r, c].CellColor == Colors.White)
    //                {
    //                    _gameManager.WhiteCount++;
    //                    piece.SetActive(true);
    //                    piece.transform.localRotation = Quaternion.Euler(90, 0, 0);
    //                }
    //                else if (cell[r,c].CellColor == Colors.None)
    //                {
    //                    piece.SetActive(false);
    //                }
    //            }
    //        }
    //        _cells = cell;
    //        CostCheck();
    //    }
    //    else
    //    {
    //        _recodeIndex = tmp;
    //    }
    //}
}
