using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public class QuestionController : MonoBehaviour
{
    [SerializeField] private TextAsset _questionText;
    
    private List<Question> _questions;
    
    private void Start()
    {
        _questions = JsonConvert.DeserializeObject<List<Question>>(_questionText.text);
    }


    public Question GetRandomQuestion()
    {
        //return _questions[Random.Range(0, _questions.Count - 1)];
        return _questions[4];
    }
}

public class Question
{
    public string Title;
    public string QuestionText;
    public string Precondition;
    public int Context; // 0 = answer, 1 = lightSignals, 2 = SoundSignals, 3 = Diopter
    public List<Answer> Answers;

    public Question(string title, string questionText, string precondition, List<Answer> answers)
    {
        Title = title;
        QuestionText = questionText;
        Precondition = precondition;
        Answers = answers;
    }
}

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
