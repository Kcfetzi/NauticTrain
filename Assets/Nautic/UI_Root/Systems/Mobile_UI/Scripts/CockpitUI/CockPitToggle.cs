using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CockPitToggle : MonoBehaviour
{
    [SerializeField] private RectTransform _navigationTransform;
    [SerializeField] private RectTransform _thrustTransform;

    [SerializeField] private RectTransform _toggleGrafics;
    [SerializeField] private RectTransform _toggleTransform;

    public void Awake()
    {
        _toggleTransform.GetComponent<Toggle>().onValueChanged.AddListener(ToggleControllPanel);
    }

    public void ToggleControllPanel(bool on)
    {
        if (_navigationTransform.anchoredPosition.y > 100)
        {
            _thrustTransform.anchoredPosition = new Vector2(-118.5f, -187);
            _navigationTransform.anchoredPosition = new Vector2(180, -145);
            _toggleTransform.anchoredPosition = new Vector2(40, 360);
            _toggleGrafics.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
        else
        {
            _thrustTransform.anchoredPosition = new Vector2(-118.5f, 223.5f);
            _navigationTransform.anchoredPosition = new Vector2(180, 172.5f);
            _toggleTransform.anchoredPosition = new Vector2(40, 315);
            _toggleGrafics.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }
}
