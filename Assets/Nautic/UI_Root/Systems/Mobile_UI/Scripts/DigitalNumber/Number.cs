using System.Collections;
using System.Collections.Generic;
using MPUIKIT;
using UnityEngine;
using UnityEngine.UI;

public class Number : MonoBehaviour
{
    [SerializeField] private Image _top;
    [SerializeField] private Image _topLeft;
    [SerializeField] private Image _topRight;
    [SerializeField] private Image _mid;
    [SerializeField] private Image _botLeft;
    [SerializeField] private Image _botRight;
    [SerializeField] private Image _bot;


    public void ShowNumber(int number)
    {
        _top.enabled = number != 1 && number != 4;
        _topLeft.enabled = number != 1 && number != 2 && number != 3 && number != 7;
        _topRight.enabled = number != 5 && number != 6;
        _mid.enabled = number != 0 && number != 1 && number != 7;
        _botLeft.enabled = number != 1 && number != 3 && number != 4 && number != 5 && number != 7 && number != 9;
        _botRight.enabled = number != 2;
        _bot.enabled = number != 1 && number != 4 && number != 7;
    }
}
