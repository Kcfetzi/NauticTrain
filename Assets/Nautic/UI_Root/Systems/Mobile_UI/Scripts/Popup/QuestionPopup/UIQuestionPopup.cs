using System.Collections;
using System.Collections.Generic;
using Groupup;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;



public class UIQuestionPopup : UIPopup
{
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private QuestionToggle _answerOne;
    [SerializeField] private QuestionToggle _answerTwo;
    [SerializeField] private QuestionToggle _answerThree;
    [SerializeField] private QuestionToggle _answerFour;
    [SerializeField] private QuestionToggle _answerFive;
    [SerializeField] private QuestionToggle _answerSix;

    // if diopter or stuff needs to be opened
    [SerializeField] private Button _kontextButton;
    [SerializeField] private TMP_Text _kontextLabel;
    [SerializeField] private TMP_Text _kontextAnswer;
    
    [SerializeField] private Button _nextButton;

    private Question _question;
    
    private UnityAction _submitAction;
    private List<int> _checkboxSet;
    
    
    public void Init(Question question, UnityAction submitAction, string kontextAnswer)
    {
        // show the question
        _title.text = question.Title  + "\nVorbedingung: " + question.Precondition;
        _text.text = question.QuestionText;

        _question = question;
        _submitAction = submitAction;
        
        // hide nextbutton till the question is answered
        _nextButton.gameObject.SetActive(false);

        // if this quests is solved by multiple choice
        // init your checkboxes with answers
        _answerOne.Init(_question.Answers.Count > 0 && _question.Context == 0 ? _question.Answers[0] : null);
        _answerTwo.Init(_question.Answers.Count > 1 && _question.Context == 0 ? _question.Answers[1] : null);
        _answerThree.Init(_question.Answers.Count > 2 && _question.Context == 0 ? _question.Answers[2] : null);
        _answerFour.Init(_question.Answers.Count > 3 && _question.Context == 0 ? _question.Answers[3] : null);
        _answerFive.Init(_question.Answers.Count > 4 && _question.Context == 0 ? _question.Answers[4] : null);
        _answerSix.Init(_question.Answers.Count > 5 && _question.Context == 0 ? _question.Answers[5] : null);
        
        // if there is a kontext, user needs to interact with third party
        _kontextButton.gameObject.SetActive(_question.Context != 0);
        string kontextText = "";

        switch (_question.Context)
        {
            case 1:
                kontextText = "Visuelle Signale öffnen";
                break;
            case 2:
                kontextText = "Akustische Signale öffnen";
                break;
            case 3:
                kontextText = "Diopter öffnen";
                break;
            default:
                kontextText = "None";
                break;
        }
        _kontextLabel.text = kontextText;
        _kontextAnswer.text = kontextAnswer;
    }

    // user clicked check on question popup
    public void OnClick_Check()
    {
        if (_question.Context == 0)
        {
            bool firstCorrect = _answerOne.Status();
            bool secondCorrect = _answerTwo.Status();
            bool thirdCorrect = _answerThree.Status();
            bool fourthCorrect = _answerFour.Status();
            bool fithCorrect = _answerFive.Status();
            bool sixthCorrect = _answerSix.Status();

            if (firstCorrect && secondCorrect && thirdCorrect && fourthCorrect && fithCorrect && sixthCorrect)
            {
                _answerOne.SetActive(false);
                _answerTwo.SetActive(false);
                _answerThree.SetActive(false);
                _answerFour.SetActive(false);
                _answerFive.SetActive(false);
                _answerSix.SetActive(false);

                _nextButton.gameObject.SetActive(true);
            }
        }
        else
        {
            //_nextButton.gameObject.SetActive(_kontextAnswer.text == _question.Answers[0].AnswerText);
            _nextButton.gameObject.SetActive(true);
        }
    }

    // 1 = visual signals, 2 = auditiv signals, 3 = diopter
    public void OnClick_Kontext()
    {
        switch (_question.Context)
        {
            case 1:
                PopupManager.Instance.ShowLightsSignalsPopup(_question, _submitAction);
                break;
            case 2:
                PopupManager.Instance.ShowSoundsSignalPopup(_question, _submitAction);
                break;
            case 3:
                ResourceManager.GetInterface<UI_RootInterface>().OnOpenDiopter(_question, _submitAction);
                break;
        }
        PopupManager.Instance.Hide();
    }
    
    
    public void OnClick_Submit()
    {
        _submitAction?.Invoke();
        
        PopupManager.Instance.Hide();
    }
}
