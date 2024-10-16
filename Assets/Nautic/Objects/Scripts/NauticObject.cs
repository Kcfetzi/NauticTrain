using System;
using Groupup;
using StylizedWater2;
using UnityEngine;


/**
 * This class register the holding gameobject in ecdis view.
 */
public class NauticObject : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private LightController _lightController;
    [SerializeField] private SoundController _soundController;

    [SerializeField] private Transform _rotationsObject;
    [SerializeField] private MeshRenderer _renderer;
    
    private ScenarioInterface _scenarioInterface;
    private CockpitController _cockpitController;

    private CFzg _aiFzg;
    private int _wayPointIndex;
    private double _timer;
    
    public ObjectContainer Data;
    private Symbol _symbol;


    public MeshRenderer Renderer => _renderer;
    public LightController LightController => _lightController;
    public SoundController SoundController => _soundController;
    public CameraController CameraController => _cameraController;
    public Transform RotationObject => _rotationsObject;

    public Symbol Symbol
    {
        get => _symbol;
        set => _symbol = value;
    }
    
    public void SetAI(CFzg fzg)
    {
        _aiFzg = fzg;
        _wayPointIndex = 0;
        if (_aiFzg.PresentationTrack(0).ListeSPD.Count > 0)
            _timer = _aiFzg.PresentationTrack(0).ListeSPD[_wayPointIndex].timestamp;
    }
    
    public void Init(ObjectContainer data)
    {
        Data = data;

        Data.Position = new Position(transform);
        _scenarioInterface = ResourceManager.GetInterface<ScenarioInterface>();
        
        UI_RootInterface _uiRootInterface = ResourceManager.GetInterface<UI_RootInterface>();
        if (_uiRootInterface.IsActive)
            _uiRootInterface.RegisterNauticObject(this);
        else
            _uiRootInterface.OnSceneLoaded += () => _uiRootInterface.RegisterNauticObject(this);
    }
    
    public void SetSelected(bool active)
    {
        _cameraController.SetSelected(active);
        FindObjectOfType<WaterGrid>().followTarget =  active ? transform : null;
        if (_rotationsObject.childCount > 1)
            _rotationsObject.GetChild(0).gameObject.SetActive(!active);
    }
    
    public void SetCourse(float direction)
    {
        Data.WantedCourse = direction;
    }

    private void Update()
    {
        if (_aiFzg == null || _aiFzg.PresentationTrack(0).ListeSPD.Count == 0 || _wayPointIndex>=_aiFzg.PresentationTrack(0).ListeSPD.Count)
            return;

        _timer = Time.timeSinceLevelLoad;
        CTrack track = _aiFzg.PresentationTrack(0);
        
        double lerpTime;
        if (_wayPointIndex == 0)
        {
            lerpTime = track.ListeSPD[_wayPointIndex].timestamp - 0;
        }
        else
        {
            lerpTime = track.ListeSPD[_wayPointIndex].timestamp -
                       track.ListeSPD[_wayPointIndex - 1].timestamp;
        }

        double progress = _timer - track.ListeSPD[_wayPointIndex].timestamp;
        lerpTime = progress / lerpTime;

        if (lerpTime > 1f)
        {
            _wayPointIndex++;
            if (_aiFzg.PresentationTrack(0).ListeSPD.Count>=_wayPointIndex) return;
            progress = _timer - track.ListeSPD[_wayPointIndex].timestamp;
            lerpTime = progress / lerpTime;
        }

        if (_wayPointIndex != 0)
        {
            float newX = (float)_scenarioInterface.Lon2unityX(Extensions.Lerp(track.ListeSPD[_wayPointIndex - 1].lon,
                track.ListeSPD[_wayPointIndex].lon, lerpTime));
            
            float newZ = (float)_scenarioInterface.Lat2unityZ(Extensions.Lerp(track.ListeSPD[_wayPointIndex - 1].lat,
                track.ListeSPD[_wayPointIndex].lat, lerpTime));

            Data.ActualVelocity = Math.Round(track.ListeSPD[_wayPointIndex].FdW) ;
            Data.ActualCourse = (float)Math.Round(track.ListeSPD[_wayPointIndex].KdW) ;
            
            Vector3 newPos = new Vector3(newX, transform.position.y, newZ);
            transform.position = newPos;

            float rotationInY = Mathf.LerpAngle((float)track.ListeSPD[_wayPointIndex - 1].KdW,
                (float)track.ListeSPD[_wayPointIndex].KdW, (float)lerpTime);
            _rotationsObject.localRotation = Quaternion.Euler(new Vector3(0, rotationInY, 0));
        }
    }
}

public enum NauticType
{
    MineSweeper, Cargo, Ton, Point
}
