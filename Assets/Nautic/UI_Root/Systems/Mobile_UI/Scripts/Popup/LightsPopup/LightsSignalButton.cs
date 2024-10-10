using System;
using System.Collections;
using System.Collections.Generic;
using MPUIKIT;
using UnityEngine;

public class LightsButton : MonoBehaviour
{
    [Header("Lights")] 
    [SerializeField] private MPImage _red;
    [SerializeField] private MPImage _green;
    [SerializeField] private MPImage _white;
    [SerializeField] private MPImage _none;
   

    // some lights are 360 grad visible, this needs to be shown as little black dot in the middle
    [SerializeField] private bool _is360Degree;
    [SerializeField] private MPImage _middleDot;
    
    private LightSignal _ownLight = LightSignal.None;

    public LightSignal Light => _ownLight;

    private void Awake()
    {
        if (_middleDot)
            _middleDot.enabled = _is360Degree;
    }

    public void SetLight(LightSignal signal)
    {
        _red.enabled = signal == LightSignal.Red;
        _green.enabled = signal == LightSignal.Green;
        _white.enabled = signal == LightSignal.White;
        _none.enabled = signal == LightSignal.None;
    }
    
    // light button was pressed by user
    public void PressLight()
    {
        _ownLight++;
        if ((int)_ownLight == 4)
            _ownLight = 0;

        SetLight(_ownLight);
    }

    /*
     * light and symbol encoding.
     */
    public enum LightSignal
    {
        None = 0,
        White = 1,
        Red = 2,
        Green = 3,
    }
}
