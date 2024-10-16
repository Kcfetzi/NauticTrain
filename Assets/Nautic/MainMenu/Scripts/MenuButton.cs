using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private Image _background;

    private UnityAction<int> _clickCallback;
    
    public void Init(UnityAction<int> clickCallback)
    {
        _clickCallback = clickCallback;
    }

    public void OnClick()
    {
        _clickCallback?.Invoke(transform.GetSiblingIndex());
    }
    
    public void SetActive(bool active)
    {
        _background.color = active ? Color.yellow : Color.white;
    }
    
    
}
