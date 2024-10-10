using System.Collections;
using System.Collections.Generic;
using MPUIKIT;
using UnityEngine;

public class Number : MonoBehaviour
{
    [SerializeField] private MPImage _top;
    [SerializeField] private MPImage _topLeft;
    [SerializeField] private MPImage _topRight;
    [SerializeField] private MPImage _mid;
    [SerializeField] private MPImage _botLeft;
    [SerializeField] private MPImage _botRight;
    [SerializeField] private MPImage _bot;


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

    private int testNumber = 0;
    public void TestShowNumber()
    {
        ShowNumber(testNumber++);
        if (testNumber / 10 > 0)
            testNumber = ((testNumber) % 10);
    }
}
