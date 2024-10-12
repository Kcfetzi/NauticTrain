using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CockpitUIController : MonoBehaviour
{
    [Header("RuderCanvas")]
    [SerializeField] private RectTransform _ruderImage;

    [Header("ShipValuesCanvas")]
    [SerializeField] private TMP_Text _velocityOverGroundText;
    [SerializeField] private TMP_Text _courseOverGroundText;
    
    [Header("ControllPanelCanvas")] 
    [SerializeField] private RectTransform _panel;
    [SerializeField] private RectTransform _rect;

    [SerializeField] private RuderKnob _ruderKnob;
    [SerializeField] private Slider _thrustSlider;
    
    [SerializeField] private TMP_Text _ruderText;
    [SerializeField] private TMP_Text _thrustText;
    
    [SerializeField] private TMP_Text _wantedThrustText;
    [SerializeField] private TMP_Text _wantedCourseText;
    
    [SerializeField] private GameObject _thrustChanger;
    
    private ObjectContainer _displayedData;
    
    // user input datea
    private float _lastTimeUserTouched = 0;
    private float _wantedTrust;
    private float _wantedCourse;

    // bool to determine if course is hardset or ruder is used
    private bool _setCourse;
    
    private void Awake()
    {
        _thrustSlider.onValueChanged.AddListener(AddToWantedThrust);
    }

    private void Update()
    {
        if (!_displayedData)
            return;

        _velocityOverGroundText.text = "VOG: " + _displayedData.ActualVelocity;
        _courseOverGroundText.text = "COG: " + _displayedData.ActualCourse;
        
        if (!_displayedData.UserInteract)
        {
            _ruderImage.localRotation = Quaternion.Euler(new Vector3(0, 0, _displayedData.RuderValue));
            
            _ruderKnob.SetRotation(_displayedData.RuderValue);
            _thrustSlider.SetValueWithoutNotify(_displayedData.ThrustValue);
            
            _ruderText.text = _displayedData.RuderValue + " \u00B0";
            _wantedThrustText.text = _displayedData.ThrustValue.ToString();
            _thrustText.text = _displayedData.ThrustValue.ToString();
            _wantedTrust = _displayedData.ThrustValue;
        } else 
        {
            if (Time.time - _lastTimeUserTouched > 3)
            {
                _displayedData.ThrustValue = Int32.Parse(_wantedThrustText.text.Substring(0, _wantedThrustText.text.Length -3));
                if (_setCourse)
                {
                    _displayedData.WantedCourse = _wantedCourse;
                } else
                {
                    _displayedData.RuderValue = _ruderKnob.Rotation;
                }
                _setCourse = false;
                _displayedData.UserInteract = false;
                _displayedData.OnUserInteractionStopped?.Invoke();
            }
        }
    }

    public void SetSelectedNauticObject(NauticObject obj)
    {
        if (!obj)
        {
            _thrustSlider.enabled = false;
            _thrustChanger.SetActive(false);
            return;
        }

        _thrustSlider.enabled = true;
        _thrustChanger.SetActive(true);
        
        _displayedData = obj.Data;
        
        _thrustSlider.value = _displayedData.ThrustValue;
        _thrustText.text = _displayedData.ThrustValue + " KN";
        
        _ruderKnob.SetRotation(_displayedData.RuderValue);
        _ruderImage.localRotation = Quaternion.Euler(new Vector3(0, 0, _displayedData.RuderValue));
        _ruderText.text = _displayedData.RuderValue + " \u00B0";
    }
    
    private void AddToWantedThrust(float amount)
    {
         if (!_displayedData.UserInteract)
         {
             _displayedData.UserInteract = true;
             _displayedData.OnUserInteractionStarted?.Invoke();
         }
         
         _lastTimeUserTouched = Time.time;
         _wantedTrust = Mathf.Clamp(amount,-5, 18);
         _wantedThrustText.text = _wantedTrust + " KN";
    }

    public void SetWantedRuder(float amount)
    {
        if (!_displayedData.UserInteract)
        {
            _displayedData.UserInteract = true;
            _displayedData.OnUserInteractionStarted?.Invoke();
        }

        _ruderText.text = amount.ToString("F0") +  " \u00B0";
        _lastTimeUserTouched = Time.time;
    }
    
    public void AddToWantedDegree(int amount)
    {
        if (!_displayedData.UserInteract)
        {
            _displayedData.UserInteract = true;
            _displayedData.OnUserInteractionStarted?.Invoke();
        }
        _setCourse = true;
        _lastTimeUserTouched = Time.time;

        _wantedCourse += amount;
        if (_wantedCourse < 0)
            _wantedCourse += 360;
        
        if (Math.Abs(_wantedCourse) >= 360)
            _wantedCourse %= 360;
        
        if (_wantedCourse < 100)
            if (_wantedCourse < 10)
                _wantedCourseText.text = "00" + _wantedCourse;
            else
            {
                _wantedCourseText.text = "0" + _wantedCourse;
            }
        else 
            _wantedCourseText.text = _wantedCourse.ToString();
        
    }

    public void ToggleControllPanel()
    {
        if (_panel.anchoredPosition.y > 100)
        {
            _panel.anchoredPosition = new Vector2(0, -60);
            _rect.localRotation = Quaternion.Euler(Vector3.zero);
        }
        else
        {
            _panel.anchoredPosition = new Vector2(0, 175);
            _rect.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
    }
}
