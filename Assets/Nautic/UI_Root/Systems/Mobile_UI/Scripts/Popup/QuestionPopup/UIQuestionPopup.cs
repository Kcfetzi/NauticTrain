using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Answer
{
    public string AnswerText;
    public bool ISCorrect;

    public Answer(string answerText, bool isCorrect)
    {
        AnswerText = answerText;
        ISCorrect = isCorrect;
    }
}

public class UIQuestionPopup : UIPopup
{
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private QuestionToggle _answerOne;
    [SerializeField] private QuestionToggle _answerTwo;
    [SerializeField] private QuestionToggle _answerThree;
    [SerializeField] private QuestionToggle _answerFour;

    [SerializeField] private Button _nextButton;
    
    private UnityAction _submitAction;
    private List<Answer> _answers;
    private List<int> _checkboxSet;
    
    
    public void Init(string title, string text, List<Answer> answers, UnityAction submitAction)
    {
        // show the question
        _title.text = title;
        _text.text = text;

        _answers = answers;
        _submitAction = submitAction;
        
        // hide nextbutton till the question is answered
        _nextButton.gameObject.SetActive(false);
        
        // init your checkboxes with answers
        _answerOne.Init(answers.Count > 0 ? answers[0] : null);
        _answerTwo.Init(answers.Count > 1 ? answers[1] : null);
        _answerThree.Init(answers.Count > 2 ? answers[2] : null);
        _answerFour.Init(answers.Count > 3 ? answers[3] : null);
    }

    // user clicked check on question popup
    public void OnClick_Check()
    {
        bool firstCorrect = _answerOne.Status();
        bool secondCorrect = _answerTwo.Status();
        bool thirdCorrect = _answerThree.Status();
        bool fourthCorrect = _answerFour.Status();
        
        if (firstCorrect && secondCorrect && thirdCorrect && fourthCorrect)
        {
            _answerOne.SetActive(false);
            _answerTwo.SetActive(false);
            _answerThree.SetActive(false);
            _answerFour.SetActive(false);
            
            _nextButton.gameObject.SetActive(true);
        }
    }
    
    
    public void OnClick_Submit()
    {
        _submitAction?.Invoke();
        
        PopupManager.Instance.Hide();
    }
}
