using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static IColors;
using static IGameState;

public class Reversi : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Text _timerText;
    [SerializeField] float _timerValue;
    [SerializeField] InputField _inputField;
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
    Recode _recode;
    GameState _state = GameState.Game;
    float timer;
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
        timer = _timerValue;
        _recode = FindObjectOfType<Recode>();
        //初期盤面を生成
        GetComponent<GridLayoutGroup>().constraintCount = _column;
        for (char r = '1'; r <= '8'; r++)
        {
            for (char c = 'A'; c <= 'H'; c++)
            {
                string id = $"{c}{r}";
                GameObject cell = Instantiate(_cellPrefab, transform);
                cell.name = $"{id}";
                _fieldData.Add(id, cell.GetComponent<Cell>());
                GameObject piece = Instantiate(_piecePrefab, cell.transform);
                piece.SetActive(false);
                _pieceData[id] = piece;
                if(id == "D4" || id == "E5")
                {
                    _gameManager.WhiteCount++;
                    piece.SetActive(true);
                    piece.transform.localRotation = Quaternion.Euler(90, 0, 0);
                    _fieldData[id].CellColor = Colors.White;
                }
                if (id == "E4" || id == "D5")
                {
                    _gameManager.BlackCount++;
                    piece.SetActive(true);
                    piece.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                    _fieldData[id].CellColor = Colors.Black;
                }
            }
        }
        CostCheck();
    }

    private void Update()
    {
        if (_state == GameState.Game)
        {
            timer -= Time.deltaTime;
        }
        _timerText.text = timer.ToString("f2");
        if (timer < 0)
        {
            SkipCheck(Colors.None);
        }

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Cell currentCell = eventData.pointerCurrentRaycast.gameObject.GetComponent<Cell>();
        if (CheckCell(currentCell, out char r, out char c) && _myTurn)
        {
            if (SkipCheck(Colors.Black))
            {
                return;
            }
            if (_fieldData[$"{c}{r}"].BlackCost > 0)
            {
                PieceCreate($"{c}{r}", Colors.Black);
                StartCoroutine(NextTurn());
            }
        }
    }
    bool SkipCheck(Colors nowColor)
    {
        if (_state == GameState.Recode) return false;
        foreach (var cell in _fieldData)
        {
            if(nowColor == Colors.Black)
            {
                if (cell.Value.BlackCost > 0)
                {
                    return false;
                }
            }
            else if(nowColor == Colors.White)
            {
                if (cell.Value.WhiteCost > 0)
                {
                    return false;
                }
            }
        }
        _myTurn = !_myTurn;
        timer = _timerValue;
        StartCoroutine(_skip.Play());
        return true;
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
        List<KeyValuePair<char, char>> piece = new List<KeyValuePair<char, char>>();
        int max = int.MinValue;
        string selectId = "";
        for (char c = 'A'; c <= 'H'; c++)
        {
            for (char r = '1'; r <= '8'; r++)
            {
                string id = $"{c}{r}";
                if (_fieldData[id].CellColor == Colors.None && _fieldData[id].WhiteCost > max)
                {
                    max = _fieldData[id].WhiteCost;
                    selectId = id;
                }
            }
        }
        if (SkipCheck(Colors.White))
        {
            return;
        }
        else
        {
            PieceCreate(selectId, Colors.White);
        }
    }
    /// <summary>駒を置いた場合ひっくり返す事ができる枚数を計算</summary>
    void CostCheck()
    {
        for (char c = 'A'; c <= 'H'; c++)
        {
            for (char r = '1'; r <= '8'; r++)
            {
                
                string id = $"{c}{r}";
                if (_fieldData[id].CellColor == Colors.None)
                {
                    _fieldData[id].WhiteCost = 0;
                    _fieldData[id].BlackCost = 0;
                    foreach (Colors color in Enum.GetValues(typeof(Colors)))
                    {
                        if (color == Colors.None) continue;
                        foreach (var dir in direction)
                        {
                            CostCount(r, c, dir.x, dir.y, color);
                        }
                    }
                }
            }
        }
    }
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
                    id = $"{column}{row}";
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
                    id = $"{column}{row}";
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
        List<KeyValuePair<char, char>> pieces = new List<KeyValuePair<char, char>>();
        if (_state == GameState.Game)
        {
            timer = _timerValue;
            _myTurn = !_myTurn;//置いたらターンを変える
        }
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
        foreach (var dir in direction)
        {
            StartCoroutine(Revers(ReversCheck(ref pieces, nowColor, id[0], id[1], dir.x, dir.y), pieces));
        }
        _fieldData[id].CellColor = nowColor;
        _pieceData[id] = piece;
        _recode.AddText(id);
    }
    /// <summary>上下左右斜めを見てリバース出来るかチェックする</summary>
    /// <param name="pieces">リバース出来る駒の位置</param>
    /// <param name="nowColor">今置いた駒の色</param>
    /// <param name="row">置いた位置(縦)</param>
    /// <param name="column">置いた位置(横)</param>
    /// <param name="rowPlus">上下に移動</param>
    /// <param name="columnPlus">左右に移動</param>
    /// <returns></returns>
    Colors ReversCheck(ref List<KeyValuePair<char, char>> pieces, Colors nowColor, char column, char row, int rowPlus, int columnPlus)
    {
        Colors changeColor = Colors.None;
        List<KeyValuePair<char, char>> currentReversPiece = new List<KeyValuePair<char, char>>();
        int r = row;
        row = (char)(r + rowPlus);
        int c = column;
        column = (char)(c + columnPlus);
        string id = $"{column}{row}";
        while (_fieldData.Keys.Contains(id))
        {
            id = $"{column}{row}";
            if (_fieldData[id].CellColor == Colors.None)
            {
                changeColor = Colors.None;
                break;
            }
            if (nowColor == Colors.White)
            {
                if (_fieldData[id].CellColor == Colors.Black)
                {
                    currentReversPiece.Add(new KeyValuePair<char, char>(row, column));
                    r = row;
                    row = (char)(r + rowPlus);
                    c = column;
                    column = (char)(c + columnPlus);
                    id = $"{column}{row}";
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
                    currentReversPiece.Add(new KeyValuePair<char, char>(row, column));
                    r = row;
                    row = (char)(r + rowPlus);
                    c = column;
                    column = (char)(c + columnPlus);
                    id = $"{column}{row}";
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
    IEnumerator Revers(Colors reversColor, List<KeyValuePair<char, char>> pieces)
    {
        if (reversColor != Colors.None)
        {
            foreach (var p in pieces)
            {
                string id = $"{p.Value}{p.Key}";
                _fieldData[id].CellColor = reversColor;
                var piece = _pieceData[id];
                if (reversColor == Colors.White)
                {
                    _gameManager.BlackCount--;
                    _gameManager.WhiteCount++;
                    piece.transform.DORotate(new Vector3(1, 0, 0) * 90f, 1f);
                }
                else if (reversColor == Colors.Black)
                {
                    _gameManager.WhiteCount--;
                    _gameManager.BlackCount++;
                    piece.transform.DORotate(new Vector3(1, 0, 0) * -90f, 1f);
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
        for (char c = 'A'; c <= 'H'; c++)
        {
            for (char r = '1'; r <= '8'; r++)
            {
                
                string id = $"{c}{r}";
                if (_fieldData[id] == currentCell && _fieldData[id].CellColor == Colors.None)
                {
                    row = r;
                    column = c;
                    return true;
                }
            }
        }
        row = '0'; column = '0';
        return false;
    }
    public void FieldChange()
    {
        _state = GameState.Recode;
        for(char c = 'A'; c <= 'H'; c++)
        {
            for(char r = '1'; r <= '8'; r++)
            {
                string id = $"{c}{r}";
                if (_fieldData[id].CellColor == Colors.None) continue;
                else
                {
                    _fieldData[id].CellColor = Colors.None;
                    _pieceData[id].SetActive(false);
                }
            }
        }
        _fieldData["D4"].CellColor = Colors.White;
        _fieldData["E5"].CellColor = Colors.White;
        _fieldData["D5"].CellColor = Colors.Black;
        _fieldData["E4"].CellColor = Colors.Black;
        GameObject piece = _pieceData["D4"];
        piece.SetActive(true);
        piece.transform.localRotation = Quaternion.Euler(90, 0, 0);
        piece = _pieceData["E5"];
        piece.SetActive(true);
        piece.transform.localRotation = Quaternion.Euler(90, 0, 0);
        piece = _pieceData["E4"];
        piece.SetActive(true);
        piece.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        piece = _pieceData["D5"];
        piece.SetActive(true);
        piece.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        List<char> gameRecode = new List<char>();
        foreach(var t in _inputField.text)
        {
            gameRecode.Add(t);
        }
        Colors nowColor = Colors.Black;
        while(gameRecode.Count > 0)
        {
            string id = $"{gameRecode[0]}{gameRecode[1]}";
            gameRecode.RemoveAt(0);
            gameRecode.RemoveAt(0);
            PieceCreate(id, nowColor);
            if(nowColor == Colors.Black)
            {
                nowColor = Colors.White;
                continue;
            }
            if(nowColor == Colors.White)
            {
                nowColor = Colors.Black;
                continue;
            }
        }
    }
}
