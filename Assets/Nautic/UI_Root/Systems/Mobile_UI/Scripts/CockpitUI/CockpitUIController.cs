using TMPro;
using UnityEngine;

public class CockpitUIController : MonoBehaviour
{
    [Header("RuderCanvas")]
    [SerializeField] private RectTransform _ruderDisplayImage;

    [Header("ShipValuesCanvas")]
    [SerializeField] private TMP_Text _velocityOverGroundText;
    [SerializeField] private TMP_Text _courseOverGroundText;

    [SerializeField] private RuderKnob _ruderKnob;
    [SerializeField] private CourseChanger _courseChanger;
    [SerializeField] private ThrustSlider _thrustSlider;

    [SerializeField] private GameObject _thrustGO;
    
    private ObjectContainer _displayedData;
    
    // user input datea
    private float _lastTimeUserTouched = 0;
    private float _wantedTrust;
    private float _wantedRuder;
    private float _wantedCourse;

    // bool to determine if course is hardset or ruder is used
    private bool _setCourse;


    private void Start()
    {
        _ruderKnob.Init(SetWantedRuder);
    }

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
            
            //_ruderKnob.SetRotation(_displayedData.RuderValue);
            _thrustSlider.SetThrust(_displayedData.ThrustValue);
        } else 
        {
            // does user interact with cockpit in last 3 seconds?
            if (Time.time - _lastTimeUserTouched > 3)
            {
                if (_displayedData.ThrustValue != _wantedTrust)
                {
                    _displayedData.ThrustValue = _wantedTrust;
                    PopupManager.Instance.ShowCommunicationPopup("Geschwindigkeit auf " + _wantedTrust + " Knoten gesetzt!", true,true, 2);
                }

                // check if user want to set a course or set the ruder
                if (_setCourse && _displayedData.WantedCourse != -_wantedCourse)
                {
                    _displayedData.WantedCourse = _wantedCourse;
                    PopupManager.Instance.ShowCommunicationPopup("Wir haben Kurs auf " + Mathf.Abs(_wantedCourse) + " gesetzt!", true,true, 2);
                } else if (!_setCourse && _displayedData.RuderValue != -_wantedRuder)
                {
                    _displayedData.RuderValue = _wantedRuder;
                    PopupManager.Instance.ShowCommunicationPopup("Wir haben Kurs auf " + Mathf.Abs(_wantedRuder) + (_wantedRuder < 0 ? " Backbord" : " Steuerbord" ) + " gesetzt!", true, true, 2);
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
    
    public void ToggleThrust(bool active)
    {
        _thrustGO.SetActive(active);
        _courseChanger.gameObject.SetActive(!active);
    }
}
