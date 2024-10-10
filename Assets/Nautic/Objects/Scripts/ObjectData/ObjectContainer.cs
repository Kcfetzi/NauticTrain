using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
 * Scriptableobject that stores all data about nauticTrain objects.
 */

[CreateAssetMenu(menuName = "ObjectData", fileName = "ObjectData")]
public class ObjectContainer : Container
{
    public Vector3 m_Velocity;
    public float m_Direction;
    public Position Position;

    public float ActualCourse;
    public float WantedCourse;
    public double ActualVelocity;
    
    public float ThrustValue;
    public float RuderValue;

    public List<int> LightsOrSymbols;

    #region Ecdis

    public Color EcdisColor = Color.black;
    public float EcdisSize = 1;

    #endregion


    //observer 
    public UnityAction OnEcdisChanged;
    // user changed thrust or ruder
    public UnityAction OnUserInteractionStarted;
    // threshhold after last user interaction was set
    public UnityAction OnUserInteractionStopped;
    // is the user interacting at the moment
    public bool UserInteract;
    
    public List<string> PolylineIds = new List<string>();

    [Header("Debug")] 
    public string Debug1;
    public string Debug2;
    public string Debug3;
    public string Debug4;
    
    
    [Header("Init Data")]
    public NauticType ShipType;
    public Symbol m_EcdisSymbolPrefab;
    
    
}
