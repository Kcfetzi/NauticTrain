using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class FunkMessage : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    private float startX;
    private UnityAction _finishedAction;

    public void Init(string msg, UnityAction finished)
    {
        _text.text = msg;
        startX = transform.localPosition.x;
        _finishedAction = finished;
        transform.DOLocalMoveX(0, 1).OnComplete(() => StartCoroutine(ArrivedAtMid()));
    }

    public IEnumerator ArrivedAtMid()
    {
        yield return new WaitForSeconds(3);
        
        transform.DOLocalMoveX(-startX, 1).OnComplete(() =>
        {
            _finishedAction?.Invoke();
            Destroy(gameObject);
        });
    }
}
