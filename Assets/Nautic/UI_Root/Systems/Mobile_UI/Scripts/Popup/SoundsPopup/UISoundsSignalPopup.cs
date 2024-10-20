using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UISoundsSignalPopup : UIPopup
{
    private Question _contextQuestion;
    private UnityAction _submitCallback;
    
    public void Init(Question question, UnityAction submitCallback)
    {
        _contextQuestion = question;
        _submitCallback = submitCallback;
    }
    
    
    public void OnClick_Submit()
    {
        if (_contextQuestion == null)
        {
            
        }
        else
        {
            PopupManager.Instance.ShowQuestionPopup(_contextQuestion, _submitCallback, "Antwort vom SoundPopup");
        }
        PopupManager.Instance.Hide();
    }
}
