using System;
using System.Collections;
using Groupup;
using MPUIKIT;
using TMPro;
using UnityEngine;

public class Compass : MonoBehaviour
{
    //[SerializeField] private MPImage _compass;
    [SerializeField] private TMP_Text _degreeText;
    [SerializeField] private TMP_Text _wantedDegreeText;

    [SerializeField] private GameObject _changer;

    private NauticObject _nauticObject;
    private Transform _magneticNord;

    private bool _degreeWasManualyChanged = false;
    private float _lastTimeUserTouched = 0;

    // Triggered if player is spawned. Players rotation sets compassdegree
    public void SetSelectedNauticObject(NauticObject obj)
    {
        _magneticNord = ResourceManager.GetInterface<ScenarioInterface>().MagneticNorth;
        _nauticObject = obj;

        _changer.SetActive(obj);

        Vector3 directionToObject2 = _magneticNord.position - _nauticObject.Data.Position.UnityPositionFloat;
        
        float angle = Vector3.SignedAngle(_magneticNord.forward, directionToObject2, Vector3.up);
       angle = -angle + _nauticObject.Data.m_Direction;
        if (angle < 0)
            angle += 360;
        angle %= 360;
        _wantedDegreeText.text = angle < 100 ? "0" : "" + Mathf.Round(angle);
    }
 
    void Update ()
    {
        if (!_nauticObject)
            return;

        Vector3 directionToObject2 = _magneticNord.position - _nauticObject.Data.Position.UnityPositionFloat;
        
        float angle = Vector3.SignedAngle(_magneticNord.forward, directionToObject2, Vector3.up);
        Vector3 rotation = new Vector3(0, 0, -angle + _nauticObject.Data.m_Direction);
        _degreeText.text = Mathf.Round(rotation.z) + "\u00B0";
        if (!_degreeWasManualyChanged)
        {
        }
        else
        {
            if (Time.time - _lastTimeUserTouched > 3)
            {
                _nauticObject.SetCourse(float.Parse(_wantedDegreeText.text));
                _degreeWasManualyChanged = false;
            }
        }
        //_compass.transform.rotation = Quaternion.Euler(rotation);
    }

    public void AddToWantedDegree(int amount)
    {
        _degreeWasManualyChanged = true;
        _lastTimeUserTouched = Time.time;

        float degree = Int32.Parse(_wantedDegreeText.text) + amount;
        if (degree < 0)
            degree += 360;
        
        if (Math.Abs(degree) >= 360)
            degree %= 360;
        
        if (degree < 100)
            if (degree < 10)
                _wantedDegreeText.text = "00" + degree;
            else
            {
                _wantedDegreeText.text = "0" + degree;
            }
        else 
            _wantedDegreeText.text = degree.ToString();
    }
}
