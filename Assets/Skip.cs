using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static IColors;

public class Skip : MonoBehaviour
{
    [SerializeField] GameObject _skip;
    [SerializeField] Transform _startPosition;
    [SerializeField] Transform _endPosition;

     public IEnumerator Play(bool turn)
    {
        _skip.SetActive(true);
        yield return null;
    }
}
