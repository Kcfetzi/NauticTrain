using System.Collections.Generic;
using UnityEngine;
using Groupup;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine.Events;

/** 
   * Structurerfile
   * This is a generated file from Structurer.
   * If you edit this file, stick to the format from Funcs, UnityActions and methods, to keep on getting all benefits from Strukturer.
   */ 
public class ObjectsInterface : InterfaceSOBase
{

    [SerializeField] private NauticObject _mineSweeper;
    [SerializeField] private NauticObject _cargo;
    [SerializeField] private NauticObject _ton;
    [SerializeField] private NauticObject _point;
    
    private Transform _interfaceParent;
    private Transform _objectParent;

    private NauticObject _selectedObject;
    private List<NauticObject> _activeNauticObjects = new List<NauticObject>();
    
    // this is used for an unique id. It starts at 2 because the 2 repoints on the map are 0 and 1
    private int _uID = 2;
    
    public List<NauticObject> ActiveObjects => _activeNauticObjects;    
    public NauticObject SelectedObject => _selectedObject;
    
    public void Reset()
    {
        _activeNauticObjects.Clear();
        ScenarioInterface scenarioInterface = ResourceManager.GetInterface<ScenarioInterface>();
        if (scenarioInterface.IsActive)
        {
            _interfaceParent = scenarioInterface.PointsSpawnParent;
            _objectParent = scenarioInterface.ObjectSpawnParent;
        }
        else
        {
            scenarioInterface.OnSceneLoaded += () => _interfaceParent = scenarioInterface.PointsSpawnParent;
            scenarioInterface.OnSceneLoaded += () => _objectParent = scenarioInterface.ObjectSpawnParent;
        }
    }
    
    [PropertySpace(SpaceBefore = 50)]
    [Button(ButtonSizes.Large, Stretch = false), GUIColor(0, 1, 0)]
    public NauticObject SpawnObjectUnityPos(NauticType type, Vector3 unityPosition, Vector3 rotation)
    {
        ObjectContainer container = CreateInstance<ObjectContainer>();
        container.ShipType = type;
        container.UID = "o" + _uID++;
        
        NauticObject obj = SpawnObject(container, unityPosition, rotation);
        obj.Init(container);
        return obj;
    }
    
    [Button(ButtonSizes.Large, Stretch = false), GUIColor(0, 1, 0)]
    public NauticObject SpawnObjectLatLon(NauticType type, double2 latLon, Vector3 rotation)
    {
        ObjectContainer container = CreateInstance<ObjectContainer>();
        
        container.ShipType = type;
        container.UID = "o" + _uID++;

        double3 unityPos = ResourceManager.GetInterface<ScenarioInterface>().WorldToUnityPoint(latLon);
        NauticObject obj = SpawnObject(container, new Vector3((float)unityPos.x, (float)unityPos.y, (float)unityPos.z) ,rotation);
        obj.Init(container);
        return obj;
    }

    private NauticObject SpawnObject(ObjectContainer container, Vector3 position, Vector3 rotation)
    {
        NauticObject obj = null;
        switch (container.ShipType)
        {
            case NauticType.Cargo:
                obj = Instantiate(_cargo, position, Quaternion.Euler(rotation), _objectParent);
                break;
            case NauticType.MineSweeper:
                obj = Instantiate(_mineSweeper, position, Quaternion.Euler(rotation), _objectParent);
                break;
            case NauticType.Point:
                obj = Instantiate(_point, position, Quaternion.Euler(rotation), _interfaceParent);
                break;
            case NauticType.Ton:
                obj = Instantiate(_ton, position, Quaternion.Euler(rotation), _objectParent);
                break;
            
            default:
                obj = Instantiate(_point, position, Quaternion.Euler(rotation), _interfaceParent);
                break;
        }
        
        _activeNauticObjects.Add(obj);
        return obj;
    }
    
    // Returns a nauticobject with given id
    public NauticObject GetNauticObjectForId(string id)
    {
        foreach (NauticObject nauticObject in _activeNauticObjects)
        {
            if (nauticObject.Data.UID == id)
                return nauticObject;
        }

        return null;
    }

    #region observer
    
    public UnityAction<NauticObject> OnNauticObjectSelected;
    public void SelectNauticObject(NauticObject obj)
    {
        _selectedObject = obj;
        OnNauticObjectSelected?.Invoke(obj);
    }

    public UnityAction<NauticObject> OnDeleteNauticObject;

    public void DeleteNauticObject(NauticObject obj)
    {
        OnDeleteNauticObject?.Invoke(obj);

        _activeNauticObjects.Remove(obj);
        Destroy(obj.gameObject);
    }

    #endregion
}
