using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Text _blackCountText;
    [SerializeField] Text _whiteCountText;
    [SerializeField] GameObject _winnerObject;
    Reversi _reversi;
    int _blackCount;
    int _whiteCount;

    public int BlackCount { get => _blackCount; set => _blackCount = value; }
    public int WhiteCount { get => _whiteCount; set => _whiteCount = value; }

    private void Start()
    {
        _reversi = FindObjectOfType<Reversi>();
    }

    void Update()
    {
        if(_blackCountText && _whiteCountText)
        {
            _blackCountText.text = _blackCount.ToString("D2");
            _whiteCountText.text = _whiteCount.ToString("D2");
        }
        if(_blackCount + _whiteCount == _reversi.Row * _reversi.Column)
        {
            GameEnd();
        }
    }

    public void GameEnd()
    {
        _winnerObject.SetActive(true);
        Text winner = _winnerObject.transform.GetChild(0).GetComponent<Text>();
        if (_blackCount > _whiteCount)
        {
            winner.color = Color.black;
            winner.text = "Black";
        }
        else if (_whiteCount > _blackCount)
        {
            winner.color = Color.white;
            winner.text = "White";
        }
        else
        {
            winner.color = Color.blue;
            _winnerObject.transform.GetChild(1).gameObject.SetActive(false);
            winner.text = "Draw";
        }
    }
}
