using System;
using System.Collections;
using System.Collections.Generic;
using Groupup;
using MPUIKIT;
using UnityEngine;

public class RadarRings : MonoBehaviour
{
    [SerializeField] private MPImage _ring;
    
    public void SetRings(float radius, float pixelPerSm)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        
        for (float i = 0.5f; i <= radius; i += 0.5f)
        {
            MPImage img = Instantiate(_ring, transform);
            Circle circle = new Circle();
            circle.Radius = pixelPerSm * i;
            img.Circle = circle;
        }
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
