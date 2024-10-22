using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CourseChanger : MonoBehaviour
{
    [SerializeField] private CockpitUIController _cockpitUIController;

    [SerializeField] private TMP_Text _wantedCourseText;
    [SerializeField] private TMP_Text _courseText;
    
    private int _wantedCourse;


    public void SetCourse(float course)
    {
        if (course < 100)
            if (course < 10)
            {
                _courseText.text = "00" + course + "\u00B0";
            }
            else
            {
                _courseText.text = "0" + course + "\u00B0";
            }
        else 
            _courseText.text = course.ToString() + "\u00B0";
    }
    
    public void SetWantedCourse(int course)
    {
        _wantedCourse += course;
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
        
        _cockpitUIController.SetWantedCourse(_wantedCourse);
    }
}
