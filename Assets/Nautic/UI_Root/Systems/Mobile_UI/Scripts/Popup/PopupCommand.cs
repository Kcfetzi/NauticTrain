using System;
using System.Collections.Generic;
using UnityEngine.Events;


public abstract class PopupCommand
{
    public abstract void Execute();
}


public class SubmitPopupCommand : PopupCommand
{
    private string _text;
    private UnityAction _submitCallback;

    public SubmitPopupCommand(string text, UnityAction submitCallback)
    {
        _text = text;
        _submitCallback = submitCallback;
    }
    
    public override void Execute()
    {
        PopupManager.Instance.InternalShowSubmitPopup(_text, _submitCallback);
    }
}

public class SubmitCancelPopupCommand : PopupCommand
{
    private string _title;
    private string _text;
    private string _submitButtonText;
    private string _cancleButtonText;
    private UnityAction _submitCallback;
    private UnityAction _cancelCallback;

    public SubmitCancelPopupCommand(string title, string text, string submitButtonText, string cancleButtonText, UnityAction submitCallback, UnityAction cancelCallback)
    {
        _title = title;
        _text = text;
        _submitButtonText = submitButtonText;
        _cancleButtonText = cancleButtonText;
        _submitCallback = submitCallback;
        _cancelCallback = cancelCallback;
    }
    
    public override void Execute()
    {
        PopupManager.Instance.InternalShowSubmitCancelPopup(_title, _text, _submitButtonText, _cancleButtonText, _submitCallback, _cancelCallback);
    }
}

public class InputPopupCommand : PopupCommand
{
    private string _text;
    private UnityAction<string> _submitCallback;

    public InputPopupCommand(string text, UnityAction<string> submitCallback)
    {
        _text = text;
        _submitCallback = submitCallback;
    }
    
    public override void Execute()
    {
        PopupManager.Instance.InternalShowInputPopup(_text, _submitCallback);
    }
}

public class CommunicationPopupCommand : PopupCommand
{
    private string _text;
    private bool _leftSide;
    private UnityAction _submitCallback;
    private float _timeToHide;
    private bool _officer;

    public CommunicationPopupCommand(string text, bool officier, bool leftSide, float timeToHide, UnityAction submitCallback)
    {
        _text = text;
        _leftSide = leftSide;
        _submitCallback = submitCallback;
        _timeToHide = timeToHide;
        _officer = officier;
    }

    public override void Execute()
    {
        PopupManager.Instance.InternalShowCommunicationPopup(_text, _officer, _leftSide, _timeToHide, _submitCallback);
    }
}

public class LightsSignalPopupCommand : PopupCommand
{

    public LightsSignalPopupCommand()
    {
        
    }

    public override void Execute()
    {
        PopupManager.Instance.InternalShowLightsSignalPopup();
    }
}

public class SoundsSignalPopupCommand : PopupCommand
{

    public SoundsSignalPopupCommand()
    {
        
    }

    public override void Execute()
    {
        PopupManager.Instance.InternalShowLightsSignalPopup();
    }
}

public class QuestionPopupCommand : PopupCommand
{
    private string _title;
    private string _text;
    private List<Answer> _answers;
    private UnityAction _submitAction;
    
    
    public QuestionPopupCommand(string title, string text, List<Answer> answers, UnityAction submitAction)
    {
        _title = title;
        _text = text;
        _answers = answers;
        _submitAction = submitAction;
    }

    public override void Execute()
    {
        PopupManager.Instance.InternalShowQuestionPopup(_title, _text, _answers, _submitAction);
    }
}

public class SettingsPopupCommand : PopupCommand
{

    
    
    public SettingsPopupCommand()
    {

    }

    public override void Execute()
    {
        PopupManager.Instance.InternalShowSettingsPopup();
    }
}