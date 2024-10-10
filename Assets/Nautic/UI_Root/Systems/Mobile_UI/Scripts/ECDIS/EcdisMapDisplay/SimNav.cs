using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimNav : MonoBehaviour
{
    [SerializeField] private Number _hourFirstDigit;
    [SerializeField] private Number _hourSecondDigit;
    
    [SerializeField] private Number _minutesFirstDigit;
    [SerializeField] private Number _minutesSecondDigit;
    
    [SerializeField] private Number _secondsFirstDigit;
    [SerializeField] private Number _secondsSecondDigit;
    
    
    [SerializeField] private Number _multiplierFirstDigit;
    [SerializeField] private Number _multiplierSecondDigit;
    
    private float _multiplayer = 1;

    private void Update()
    {
        float time = Time.timeSinceLevelLoad;
        
        int seconds = (int) time % 60;
        string secondsString = seconds.ToString();
        if (seconds < 10)
        {
            _secondsFirstDigit.ShowNumber(0);
            _secondsSecondDigit.ShowNumber(int.Parse(secondsString[0].ToString()));
        }
        else
        {
            _secondsFirstDigit.ShowNumber(int.Parse(secondsString[0].ToString()));
            _secondsSecondDigit.ShowNumber(int.Parse(secondsString[1].ToString()));
        }
        
        int minutes = (int) (time / 60) % 60;
        string minutesString = minutes.ToString();
        if (minutes < 10)
        {
            _minutesFirstDigit.ShowNumber(0);
            _minutesSecondDigit.ShowNumber(int.Parse(minutesString[0].ToString()));
        }
        else
        {
            _minutesFirstDigit.ShowNumber(int.Parse(minutesString[0].ToString()));
            _minutesSecondDigit.ShowNumber(int.Parse(minutesString[1].ToString()));
        }
        
        int hours = (int) time / 60 / 60;
        string hoursString = hours.ToString();
        if (hours < 10)
        {
            _hourFirstDigit.ShowNumber(0);
            _hourSecondDigit.ShowNumber(int.Parse(hoursString[0].ToString()));
        }
        else
        {
            _hourFirstDigit.ShowNumber(int.Parse(hoursString[0].ToString()));
            _hourSecondDigit.ShowNumber(int.Parse(hoursString[1].ToString()));
        }

        string numberString = _multiplayer.ToString();
        _multiplayer = Time.timeScale;
        if (_multiplayer < 10)
        {
            _multiplierFirstDigit.ShowNumber(0);
            _multiplierSecondDigit.ShowNumber(int.Parse(numberString[0].ToString()));
        }
        else
        {
            _multiplierFirstDigit.ShowNumber(int.Parse(numberString[0].ToString()));
            _multiplierSecondDigit.ShowNumber(int.Parse(numberString[1].ToString()));
        }
    }

    public void AlterMultiplier(int value)
    {
        if (_multiplayer >= 1)
        {
            _multiplayer += value;
        } 
        else if (_multiplayer < 1)
        {
            if (_multiplayer == 0)
                _multiplayer = 0.5f;
            else if (value > 0)
            {
                _multiplayer /= 0.5f;
            }
            else
            {
                
                _multiplayer = Mathf.Clamp(_multiplayer * 0.5f , .125f, 1) ;
            }
        }

        _multiplayer = Mathf.Clamp(_multiplayer, 0, 20);
        
        Time.timeScale = _multiplayer;
    }

    public void SetMultiplier(int value)
    {
        if (_multiplayer < 0)
        {
            _multiplayer = 1 / value;
        }
        else
        {
            _multiplayer = value;
        }
        
        Time.timeScale = _multiplayer;
    }
}
