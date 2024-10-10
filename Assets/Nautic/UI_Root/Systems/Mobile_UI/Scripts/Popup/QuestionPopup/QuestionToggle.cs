using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// this is a toggle in the questionpopup
public class QuestionToggle : MonoBehaviour
{
    [SerializeField] private Toggle _toggle;
    [SerializeField] private TMP_Text _answerText;
    
    [SerializeField] private GameObject _correctIcon;
    [SerializeField] private GameObject _notCorrectIcon;

    private Answer _answer;
    
    // get your answer
    public void Init(Answer answer)
    {
        // if u get null, u are not available so disable yourself
        if (answer == null)
        {
            gameObject.SetActive(false);
            return;
        }
        
        // show toggle and hide correct and not correct sign 
        gameObject.SetActive(true);
        _correctIcon.SetActive(false);
        _notCorrectIcon.SetActive(false);
        _toggle.isOn = false;
        
        // set answer and show text
        _answer = answer;
        _answerText.text = answer.AnswerText;
        
        // enable your toggle
        SetActive(true);
    }

    // is this toggle set correctly
    public bool Status()
    {
        // always true if u are disabled
        if (!gameObject.activeSelf)
            return true;

        // show if user was right, show the icon to him and return correct
        bool correct = _toggle.isOn == _answer.ISCorrect;
        _correctIcon.SetActive(_toggle.isOn && correct);
        _notCorrectIcon.SetActive(_toggle.isOn && !correct);
        
        return correct;
    }

    // disable or enable the toggle of the answer
    public void SetActive(bool active)
    {
        _toggle.interactable = active;
    }
}
