using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CockpitUIController : MonoBehaviour
{
    [Header("RuderCanvas")]
    [SerializeField] private RectTransform _ruderDisplayImage;

    [Header("ShipValuesCanvas")]
    [SerializeField] private TMP_Text _velocityOverGroundText;
    [SerializeField] private TMP_Text _courseOverGroundText;
    
    [Header("ControllPanelCanvas")] 
    [SerializeField] private RectTransform _panel;
    [SerializeField] private RectTransform _rect;

    [SerializeField] private RuderKnob _ruderKnob;
    [SerializeField] private CourseChanger _courseChanger;
    [SerializeField] private ThrustSlider _thrustSlider;
    
    private ObjectContainer _displayedData;
    
    // user input datea
    private float _lastTimeUserTouched = 0;
    private float _wantedTrust;
    private float _wantedRuder;
    private float _wantedCourse;

    // bool to determine if course is hardset or ruder is used
    private bool _setCourse;
    
    private void Update()
    {
        if (!_displayedData)
            return;

        // show velocity and course in top right display
        _velocityOverGroundText.text = "VOG: " + _displayedData.ActualVelocity;
        _courseOverGroundText.text = "COG: " + _displayedData.ActualCourse;
        // always show actual course in coursechanger
        _courseChanger.SetCourse(_displayedData.ActualCourse);
        
        if (!_displayedData.UserInteract)
        {
            // left top panel with ruder display
            _ruderDisplayImage.localRotation = Quaternion.Euler(new Vector3(0, 0, _displayedData.RuderValue));
            
            _ruderKnob.SetRotation(_displayedData.RuderValue);
            _thrustSlider.SetThrust(_displayedData.ThrustValue);
        } else 
        {
            // does user interact with cockpit in last 3 seconds?
            if (Time.time - _lastTimeUserTouched > 3)
            {
                _displayedData.ThrustValue = _wantedTrust;
                // check if user want to set a course or set the ruder
                if (_setCourse)
                {
                    _displayedData.WantedCourse = _wantedCourse;
                } else
                {
                    _displayedData.RuderValue = _wantedRuder;
                }
                _setCourse = false;
                
                // delete user interacted flag
                _displayedData.UserInteract = false;
                _displayedData.OnUserInteractionStopped?.Invoke();
            }
        }
    }

    public void SetSelectedNauticObject(NauticObject obj)
    {
        _displayedData = obj.Data;
    }
    
    public void SetWantedThrust(int amount)
    {
        // mark user interaction
         if (!_displayedData.UserInteract)
         {
             _displayedData.UserInteract = true;
             _displayedData.OnUserInteractionStarted?.Invoke();
         }
         
        
         _wantedTrust = amount;
         _lastTimeUserTouched = Time.time;
    }

    // user set ruder on ruderknob, set user interacted flag
    public void SetWantedRuder(int amount)
    {
        // mark user interaction
        if (!_displayedData.UserInteract)
        {
            _displayedData.UserInteract = true;
            _displayedData.OnUserInteractionStarted?.Invoke();
        }

        _wantedRuder = amount;
        _lastTimeUserTouched = Time.time;
    }
    
    public void SetWantedCourse(int amount)
    {
        // mark user interaction
        if (!_displayedData.UserInteract)
        {
            _displayedData.UserInteract = true;
            _displayedData.OnUserInteractionStarted?.Invoke();
        }

        _setCourse = true;
        _wantedCourse = amount;
        _lastTimeUserTouched = Time.time;
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
    
    public void ToggleRuder(bool active)
    {
        _ruderKnob.gameObject.SetActive(active);
        _courseChanger.gameObject.SetActive(!active);
    }
}
