using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIInputPopup : UIPopup
{
    [SerializeField] private TMP_Text _msg;
    [SerializeField] private TMP_InputField _inputField;

    private UnityAction<string> _submitCallback;

    public void Init(string text, UnityAction<string> submitCallback = null)
    {
        _msg.text = text;
        _submitCallback = submitCallback;
    }

    public void On_Click_Submit()
    {
        _submitCallback?.Invoke(_inputField.text);
        PopupManager.Instance.Hide();
    }
}
