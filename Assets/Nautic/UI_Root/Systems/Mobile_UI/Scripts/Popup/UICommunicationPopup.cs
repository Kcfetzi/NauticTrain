using System;
using MPUIKIT;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UICommunicationPopup : UIPopup
{
    [SerializeField] private Sprite _funk;
    [SerializeField] private Sprite _commander;
    
    [SerializeField] private Image _communicator;
    [SerializeField] private RectTransform _imageTransform;

    [SerializeField] private RectTransform _panel;
    
    [SerializeField] private TMP_Text _msg;

    private float _timeToHide;
    private float _timeSinceShowUp;
    
    private UnityAction _submitCallback;
    
    
    public void Init(string text, bool officier, bool leftSide, float secondsToHide = -1, UnityAction submitCallback = null)
    {
        _communicator.sprite = officier ? _commander : _funk;
        
        if (leftSide)
        {
            // Setze die Anker und Pivot für linksbündig
            _imageTransform.anchorMin = new Vector2(0, 0.5f);
            _imageTransform.anchorMax = new Vector2(0, 0.5f);
            _imageTransform.pivot = new Vector2(0, 0f);
            _imageTransform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));

            // Setze den Abstand vom linken Rand
            _imageTransform.anchoredPosition = new Vector2(84f, -123);
            
            _panel.anchorMin = new Vector2(0, 0.5f);
            _panel.anchorMax = new Vector2(0, .5f);
            _panel.pivot = new Vector2(0, 0f);
            
            _panel.anchoredPosition = new Vector2(100, -106f);
        }
        else
        {
            // Setze die Anker und Pivot für rechtsbündig
            _imageTransform.anchorMin = new Vector2(1, 0.5f);
            _imageTransform.anchorMax = new Vector2(1, 0.5f);
            _imageTransform.pivot = new Vector2(0, 0f);
            _imageTransform.rotation = Quaternion.identity;

            // Setze den Abstand vom rechten Rand
            _imageTransform.anchoredPosition = new Vector2(-82, -123);
            
            _panel.anchorMin = new Vector2(1, .5f);
            _panel.anchorMax = new Vector2(1, .5f);
            _panel.pivot = new Vector2(0, 0f);
            
            _panel.anchoredPosition = new Vector2(-542, -106f);
        }

        _timeToHide = secondsToHide;
        _timeSinceShowUp = 0;
        
        _msg.text = text;
        _submitCallback = submitCallback;
    }

    private void Update()
    {
        if (_timeToHide > -1)
        {
            _timeSinceShowUp += Time.deltaTime;
            if (_timeToHide < _timeSinceShowUp)
                On_Click_Submit();
        }
    }

    public void On_Click_Submit()
    {
        _timeToHide = -1;
        _submitCallback?.Invoke();
        PopupManager.Instance.Hide();
    }
}
