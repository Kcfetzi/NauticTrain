using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UISubmitPopup : UIPopup
{
    [SerializeField] private TMP_Text _msg;

    private UnityAction _submitCallback;

    public void Init(string text, UnityAction submitCallback = null)
    {
        _msg.text = text;
        _submitCallback = submitCallback;
    }

    public void On_Click_Submit()
    {
        _submitCallback?.Invoke();
        PopupManager.Instance.Hide();
    }
}
