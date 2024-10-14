using System;
using System.Collections.Generic;
using Groupup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * This class display the given radardata to the user
 */
public class Radar : MonoBehaviour
{
    #region buttons

    [SerializeField] private RadarButton _05_scale;
    [SerializeField] private RadarButton _1_scale;
    [SerializeField] private RadarButton _3_scale;
    [SerializeField] private RadarButton _6_scale;
    [SerializeField] private RadarButton _12_scale;
    
    [SerializeField] private RadarButton _card;
    [SerializeField] private RadarButton _ring;

    #endregion
    
    [SerializeField] private RectTransform _playerRadarSymbol;
    [SerializeField] private RectTransform _map;
    [SerializeField] private RectTransform _radarImage;
    private RawImage _mapImage;

    [SerializeField] private RadarObjData _ownerData;
    [SerializeField] private RadarObjData _targetData;

    [SerializeField] private RadarRings _rings;
    
    [SerializeField] private RadarSymbol _radarSymbolPrefab;
    
    // The scrollview
    [SerializeField] private ScrollRect _radarMapView;
    private RectTransform _radarMapTransform;
    
    private UI_RootInterface _UIInterface;
    private ScenarioInterface _scenarioInterface;

    private NauticObject _selectedObject;
    private NauticObject _targetObject;
    private List<RadarSymbol> _activeObjects = new List<RadarSymbol>();
    
    private float _mapPixelPerSm;
    public float _radarRangeRadius;

    public float Scale => _map.localScale.x;
    
    /**
     * Setup important variables.
     */
    public void Init(Texture2D map)
    {
        _UIInterface = ResourceManager.GetInterface<UI_RootInterface>();
        _scenarioInterface = ResourceManager.GetInterface<ScenarioInterface>();
        _radarMapTransform = _radarMapView.GetComponent<RectTransform>();

        _mapImage = _map.GetComponent<RawImage>();
        _mapImage.texture = map;
        
        Rect mapRect = _map.rect;
        _UIInterface.MapSize = mapRect.size;
        Vector3 terrainSize = ResourceManager.GetInterface<ScenarioInterface>().MapSize;

        _UIInterface.WorldToRadarRatio = _UIInterface.MapSize / new Vector2(terrainSize.x, terrainSize.z);
        _UIInterface.RadarToWorldRatio = new Vector2(terrainSize.x, terrainSize.z) / _UIInterface.MapSize;
        _UIInterface.OnRegisterNauticObject += RegisterNauticObject;
        
        // how many pixel does a km have on scale 1
        _mapPixelPerSm = mapRect.width / ((terrainSize.x * (float)ResourceManager.GetInterface<ScenarioInterface>().UnityXInMeters * 0.001f) * 0.53996f);
    }

    private void OnEnable()
    {
        SetRangeRadius(3f);
    }

    public void RegisterNauticObject(NauticObject obj)
    {
        if (obj.Data.ShipType == NauticType.Point)
            return;
        
        RadarSymbol newObject = _radarSymbolPrefab.Get<RadarSymbol>(_map);
        newObject.Init(obj, _UIInterface, this);
        _activeObjects.Add(newObject);
    }
    
    public void SetSelectedNauticObject(NauticObject obj)
    {
        _selectedObject = obj;
    }

    public void SetTarget(NauticObject obj)
    {
        _targetObject = obj;

        foreach (RadarSymbol activeObject in _activeObjects)
        {
            activeObject.Unmark();
        }
        
        _targetData.SetActive(true);
    }

    public void DeleteTarget()
    {
        _targetData.SetActive(false);
        _targetObject = null;
        
        foreach (RadarSymbol activeObject in _activeObjects)
        {
            activeObject.Unmark();
        }
    }

    private void LateUpdate()
    {
        if (!_selectedObject)
            return;
        
        _ownerData.DisplayData(_selectedObject);
        _targetData.DisplayData(_targetObject);

        _playerRadarSymbol.Rotate(-Vector3.forward * Time.deltaTime * 100, Space.Self);
        _playerRadarSymbol.anchoredPosition = _UIInterface.WorldToRadarPosition(_selectedObject.transform.position);
        _playerRadarSymbol.localScale = new Vector3(1 / Scale, 1 / Scale, 1);
        
        foreach (RadarSymbol activeObject in _activeObjects)
        {
            activeObject.transform.localScale = new Vector3(1 / Scale, 1 / Scale, 1);
        }
        
        CenterContentToTarget(_playerRadarSymbol.position);
    }

    public void SetRangeRadius(float radius)
    {
        _05_scale.SetActive(radius == 0.5f);
        _1_scale.SetActive(radius == 1f);
        _3_scale.SetActive(radius == 3f);
        _6_scale.SetActive(radius == 6f);
        _12_scale.SetActive(radius == 12f);
        
        _radarRangeRadius = radius;
        float radarPixelPerSm =  _radarMapTransform.rect.width / _radarRangeRadius * 0.53996f;
        _map.localScale = new Vector3( radarPixelPerSm / _mapPixelPerSm , radarPixelPerSm / _mapPixelPerSm, 1);
        _radarImage.localScale = _map.localScale;
        
        _rings.SetRings(radius, radarPixelPerSm);
    }
    

    // Centeres the radar view on given recttransform
    private void CenterContentToTarget(Vector3 target)
    {
        _map.anchoredPosition =
            (Vector2)_radarMapView.transform.InverseTransformPoint(_map.position)
            - (Vector2)_radarMapView.transform.InverseTransformPoint(target);

        _radarImage.anchoredPosition = _map.anchoredPosition;
    }
    
    public void ToggleCard()
    {
        _mapImage.enabled = !_mapImage.enabled;
        _card.SetActive(_mapImage.enabled);
    }

    public void ToggleRings()
    {
        _rings.SetActive(!_rings.gameObject.activeSelf);
        _ring.SetActive(_rings.gameObject.activeSelf);
    }
}
