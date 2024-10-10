
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class UIPopup : MonoBehaviour
{
    [SerializeField] private RectTransform _container;
    
    private void Awake()
    {
        _container.localScale = Vector3.zero;
    }

    public void Show()
    {
        _container.DOScale(Vector3.one, .2f).SetEase(Ease.OutBack);
    }

    public void Hide(UnityAction onHidden)
    {
        _container.DOScale(Vector3.zero, .2f).OnComplete(() => onHidden());
    }
}
