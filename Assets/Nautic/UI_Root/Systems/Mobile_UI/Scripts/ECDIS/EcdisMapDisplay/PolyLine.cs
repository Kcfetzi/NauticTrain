using System;
using System.Collections.Generic;
using Groupup;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class PolyLine : MonoBehaviour
{
    private UI_RootInterface _uiInfo;
    
    // datacontainer
    private PolyLineContainer _polyLineData;
    // line render component
    private UILineRenderer _lineRenderer;
    // If line is dynamic
    private List<Symbol> _dynamicObjects = new();
    // If line is static
    private List<Vector2> _staticPositions = new();

    public PolyLineContainer Data => _polyLineData;

    private void Awake()
    {
        _uiInfo = ResourceManager.GetInterface<UI_RootInterface>();
    }

    // Setup line with points. In that case the line moves with the movement of its points
    public void Init(List<Symbol> dynamicObjects, PolyLineContainer data, string lineName = "")
    {
        _lineRenderer = GetComponent<UILineRenderer>();
        
        // store connectors
        _dynamicObjects = dynamicObjects;
        
        // set positions of dynamic objects as linerender points, this will happend in each updatestep too
        _lineRenderer.Points = new Vector2[dynamicObjects.Count];
        for (int i = 0; i < _dynamicObjects.Count; i++)
        {
            try
            {
                _lineRenderer.Points[i] = dynamicObjects[i].RectTransform.anchoredPosition;
            }
            catch
            {
                break;
            }

        }
        
        // store your own datacontainer
        _polyLineData = data;

        // name the line if no name was given
        if (string.IsNullOrEmpty(lineName))
        {
            string startPointName = dynamicObjects[0].NauticObject.Data.ObjectName;
            if (string.IsNullOrEmpty(startPointName))
                startPointName = dynamicObjects[0].NauticObject.Data.name;
            string endPointName = dynamicObjects[1].NauticObject.Data.ObjectName;
            if (string.IsNullOrEmpty(endPointName))
                endPointName = dynamicObjects[1].NauticObject.Data.name;
            lineName = startPointName + " - " + endPointName;
        }
        _polyLineData.ObjectName = lineName;

        // tell all connections the id of this polyline
        foreach (Symbol ecdisMapSymbol in dynamicObjects)
        {
            ecdisMapSymbol.NauticObject.Data.PolylineIds.Add(Data.UID);
            Data.PointIds.Add(ecdisMapSymbol.NauticObject.Data.UID);
        }
        
        // after polyline was created tell observers that something changed
        dynamicObjects[0].NauticObject.Data.OnEcdisChanged?.Invoke();
        dynamicObjects[1].NauticObject.Data.OnEcdisChanged?.Invoke();
    }

    // Setup line with positions. The line will not move
    public void Init(List<double2> latlon, PolyLineContainer data, string lineName = "")
    {
        _lineRenderer = GetComponent<UILineRenderer>();
        
        // store datacontainer
        _polyLineData = data;
        // set given name
        _polyLineData.ObjectName = lineName;
        
        // convert lat and lon to unity and then to ectispositions an set them as connectors
        _staticPositions = new List<Vector2>();
        ScenarioInterface scenarioInfo = ResourceManager.GetInterface<ScenarioInterface>();
        UI_RootInterface uiInfo = ResourceManager.GetInterface<UI_RootInterface>();
        foreach (var position in latlon)
        {
            double3 unityPosition = scenarioInfo.WorldToUnityPoint(position);
            _staticPositions.Add(uiInfo.WorldToEcdisPosition(new Vector3((float)unityPosition.x, (float)unityPosition.y, (float)unityPosition.z)));
        }
        _lineRenderer.Points = _staticPositions.ToArray();
    }
    

    private void Update()
    {
        // all lines refresh their color and their linethickness
        _lineRenderer.color = _polyLineData.Color;
        _lineRenderer.LineThickness = _polyLineData.LineThickness / _uiInfo.EcdisMapScale.x;
        
        // if dynamicObjects are set they need to get updated
        if (_dynamicObjects.Count == 0)
            return;
        
        for (int i = 0; i < _dynamicObjects.Count; i++)
        {
            //_lineRenderer.Points[i] = _dynamicObjects[i].RectTransform.anchoredPosition;
            try
            {
                _lineRenderer.Points[i] = _dynamicObjects[i].RectTransform.anchoredPosition;
            }
            catch
            {
                break;
            }
        }
    }
}
