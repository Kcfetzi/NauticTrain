
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this;
        } 
    }

    private void Start()
    {
        foreach (Transform popup in transform)
        {
            _popups.Add(popup.name, popup.GetComponent<UIPopup>());
            popup.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }


    private Dictionary<string, UIPopup> _popups = new Dictionary<string, UIPopup>();

    private UIPopup _activePopup;

    public UIPopup ActivePopup => _activePopup;
    // List of commands for popup queue
    private Queue<PopupCommand> _popupCommands = new Queue<PopupCommand>();

    private void Show()
    {
        _activePopup.Show();
    }
    
    public void Hide()
    {
        if (_popupCommands.Count == 0)
        {
            if (_activePopup)
                _activePopup.Hide(() => _activePopup = null);
        }
        else
        {
            PopupCommand nextCommand = _popupCommands.Dequeue();
            if (_activePopup)
                _activePopup.Hide(() => nextCommand.Execute());
        }
    }
    
    public void ShowSubmitPopup( string text, UnityAction submitCallback = null)
    {
        if (_activePopup)
        {
            SubmitPopupCommand popupCommand = new SubmitPopupCommand(text, submitCallback);
            _popupCommands.Enqueue(popupCommand);
        }
        else
        {
            InternalShowSubmitPopup( text,  submitCallback);
        }
    }

    public void InternalShowSubmitPopup(string text, UnityAction submitCallback)
    {
        UIPopup popup = _popups["SubmitPopup"];
        _activePopup = popup;
        popup.GetComponent<UISubmitPopup>().Init(text, submitCallback);

        Show();
    }

    public void ShowSubmitCancelPopup(string title, string text, string submitButtonText = "Ok", string cancelButtonText = "Abbrechen", UnityAction submitCallback = null, UnityAction cancelCallback = null)
    {
        if (_activePopup)
        {
            SubmitCancelPopupCommand popupCommand = new SubmitCancelPopupCommand(title, text, submitButtonText, cancelButtonText, submitCallback, cancelCallback);
            _popupCommands.Enqueue(popupCommand);
        }
        else
        {
            InternalShowSubmitCancelPopup(title, text, submitButtonText, cancelButtonText, submitCallback, cancelCallback);
        }
    }
    
    public void InternalShowSubmitCancelPopup(string title, string text, string submitButtonText, string cancelButtonText, UnityAction submitCallback, UnityAction cancelCallback)
    {
        UIPopup popup = _popups["SubmitCanclePopup"];
        _activePopup = popup;
        popup.GetComponent<UISubmitCancelPopup>().Init(title, text, submitButtonText, cancelButtonText, submitCallback, cancelCallback);

        Show();
    }
    
    public void ShowInputPopup(string text, UnityAction<string> submitCallback = null)
    {
        if (_activePopup)
        {
            InputPopupCommand popupCommand = new InputPopupCommand(text, submitCallback);
            _popupCommands.Enqueue(popupCommand);
        }
        else
        {
            InternalShowInputPopup( text,  submitCallback);
        }
    }

    public void InternalShowInputPopup(string text,  UnityAction<string> submitCallback)
    {
        UIPopup popup = _popups["InputPopup"];
        _activePopup = popup;
        popup.GetComponent<UIInputPopup>().Init(text, submitCallback);

        Show();
    }

    public void ShowCommanderPopup(string text, bool commanderleft = true, UnityAction submitaction = null)
    {
        if (_activePopup)
        {
             CommanderPopupCommand popupCommand = new CommanderPopupCommand(text, commanderleft, submitaction);
            _popupCommands.Enqueue(popupCommand);
        }
        else
        {
            InternalShowCommanderPopup(text, commanderleft, submitaction);
        }
    }

    public void InternalShowCommanderPopup(string text, bool commanderleft = true, UnityAction submitaction = null)
    {
        UIPopup popup = _popups["CommanderPopup"];
        _activePopup = popup;
        popup.GetComponent<UICommanderPopup>().Init(text, commanderleft, submitaction);

        Show();
    }
    
    public void ShowLightsSignalsPopup()
    {
        if (_activePopup)
        {
            LightsSignalPopupCommand popupCommand = new LightsSignalPopupCommand();
            _popupCommands.Enqueue(popupCommand);
        }
        else
        {
            InternalShowLightsSignalPopup();
        }
    }

    public void InternalShowLightsSignalPopup()
    {
        UIPopup popup = _popups["LightsSignalPopup"];
        _activePopup = popup;
        popup.GetComponent<UILightsSignalPopup>().Init();

        Show();
    }
    
    public void ShowSoundsSignalPopup()
    {
        if (_activePopup)
        {
            SoundsSignalPopupCommand popupCommand = new SoundsSignalPopupCommand();
            _popupCommands.Enqueue(popupCommand);
        }
        else
        {
            InternalShowSoundsSignalPopup();
        }
    }

    public void InternalShowSoundsSignalPopup()
    {
        UIPopup popup = _popups["SoundsSignalPopup"];
        _activePopup = popup;
        popup.GetComponent<UISoundsSignalPopup>().Init();

        Show();
    }
    
    public void ShowQuestionPopup(string title, string text, List<Answer> answers, UnityAction submitAction)
    {
        if (_activePopup)
        {
            QuestionPopupCommand popupCommand = new QuestionPopupCommand(title, text, answers, submitAction);
            _popupCommands.Enqueue(popupCommand);
        }
        else
        {
            InternalShowQuestionPopup(title, text, answers, submitAction);
        }
    }

    public void InternalShowQuestionPopup(string title, string text, List<Answer> answers, UnityAction submitAction)
    {
        UIPopup popup = _popups["QuestionPopup"];
        _activePopup = popup;
        popup.GetComponent<UIQuestionPopup>().Init( title,  text, answers, submitAction);

        Show();
    }
}
