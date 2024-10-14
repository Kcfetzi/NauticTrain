using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINavigationController : MonoBehaviour
{
    [SerializeField] private Mobile_UIController _uiController;
    
    [SerializeField] private Toggle _radarToggle;
    [SerializeField] private Toggle _ecdisToggle;
    [SerializeField] private Toggle _diopterToggle;

    private void Awake()
    {
        _radarToggle.onValueChanged.AddListener(OnClick_Radar);
        _ecdisToggle.onValueChanged.AddListener(OnClick_Ecdis);
        _diopterToggle.onValueChanged.AddListener(OnClick_Diopter);
    }

    // inital when user select a view from cockpit
    public void SetToRadar()
    {
        _radarToggle.SetIsOnWithoutNotify(true);
    }
    // inital when user select a view from cockpit
    public void SetToEcdis()
    {
        _ecdisToggle.SetIsOnWithoutNotify(true);
    }
    // inital when user select a view from cockpit
    public void SetToDiopter()
    {
        _diopterToggle.SetIsOnWithoutNotify(true);
    }
    public void OnClick_Radar(bool isOn)
    {
        _ecdisToggle.SetIsOnWithoutNotify(false);
        _diopterToggle.SetIsOnWithoutNotify(false);
        _uiController.ActivateRadar(!isOn);
    }
    public void OnClick_Ecdis(bool isOn)
    {
        _radarToggle.SetIsOnWithoutNotify(false);
        _diopterToggle.SetIsOnWithoutNotify(false);
        _uiController.ActivateEcdis(!isOn);
    }

    public void OnClick_Diopter(bool isOn)
    {
        _ecdisToggle.SetIsOnWithoutNotify(false);
        _radarToggle.SetIsOnWithoutNotify(false);
        _uiController.ActivateDiopter(!isOn);
    }

}
