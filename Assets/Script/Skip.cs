using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Skip : MonoBehaviour
{
    [SerializeField] GameObject _skip;
    [SerializeField] Transform _startPosition;
    [SerializeField] Transform _endPosition;
    [SerializeField] Reversi _reversi;

     public IEnumerator Play()
    {
        bool nowTurn = _reversi.Turn;
        _skip.GetComponent<Text>().DOFade(1, 0.5f);
        _skip.transform.DOMoveX(_endPosition.position.x, 1f).OnComplete(() => _skip.GetComponent<Text>().DOFade(0, 1f));
        yield return new WaitForSeconds(1f);
        _skip.transform.position = _startPosition.position;
        _reversi.Turn = !nowTurn;
    }
}
