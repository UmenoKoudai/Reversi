using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reversi : MonoBehaviour
{
    [SerializeField] GameObject _cellPrefab;
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
                cell.name = $"{r} {c}";
                _cells[r, c] = cell;
            }
        }
    }

    void Update()
    {
        
    }
}
