using System;
using System.Collections;
using System.Collections.Generic;
using Groupup;
using TMPro;
using UnityEngine;

public class RadarObjData : MonoBehaviour
{
    [SerializeField] private TMP_Text _course;
    [SerializeField] private TMP_Text _vel;
    [SerializeField] private TMP_Text _lat;
    [SerializeField] private TMP_Text _lon;
    [SerializeField] private TMP_Text _utc;

    private ScenarioInterface _scenarioInterface;

    private void Awake()
    {
        _scenarioInterface = ResourceManager.GetInterface<ScenarioInterface>();
    }

    public void DisplayData(NauticObject obj)
    {
        if (!obj)
            return;
        
        _course.text = obj.Data.m_Direction.ToString("F2");
        _vel.text = (obj.Data.ActualVelocity * _scenarioInterface.UnityToRealworldRatio).ToString();
        _lat.text = obj.Data.Position.Lat.ToString();
        _lon.text = obj.Data.Position.Lon.ToString();
        _utc.text = DateTime.Now.ToShortTimeString();
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
