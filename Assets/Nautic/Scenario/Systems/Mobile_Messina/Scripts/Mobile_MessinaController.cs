using System.Collections;
using Groupup;
using StylizedWater2;
using UnityEngine;

public class Mobile_MessinaController : MonoBehaviour
{
    [SerializeField] private MapRenderer _mapRenderer;
    [SerializeField] private Transform _magneticNorth;
    
    [SerializeField] private Transform _pointsSpawnParent;
    [SerializeField] private Transform _objectSpawnParent;

    [SerializeField] private Texture2D _map;
    [SerializeField] private Terrain _lastTerrainTile;
    
    private Mobile_MessinaInterface _mobile_Messinainterface;
    // root interface
    private ScenarioInterface _scenariointerface;
    private AIInterface _aiInterface;
    
    
    [SerializeField] private WaterGrid _waterGrid;

    [SerializeField] private ObjectPlacer _objectPlacer;
    void Awake()
    {
        // Get own interface and subscribe
        _mobile_Messinainterface = ResourceManager.GetInterface<Mobile_MessinaInterface>();
        _aiInterface = ResourceManager.GetInterface<AIInterface>();
        _scenariointerface = ResourceManager.GetInterface<ScenarioInterface>();
        
        if (_mobile_Messinainterface)
        {
        }
        else
        {
            Debug.Log("Could not subscribe to interface in _mobile_Messinainterface");
        }
    }

    private void Start()
    {
        // setup the values needed for relative movementspeed etc.
        ResourceManager.GetInterface<ScenarioInterface>().CalculateValues(_lastTerrainTile);
        
        // set magnetic north
        _scenariointerface.MagneticNorth = _magneticNorth;
        
        // place tons and static objects
        _objectPlacer.Init();
        
        // init weather
        _scenariointerface.SetWeather();
        
        // Setup ui
        UI_RootInterface uiInterface = ResourceManager.GetInterface<UI_RootInterface>();
        if (uiInterface)
        {
            if (uiInterface.IsActive)
            {
                uiInterface.InitUI(_map);
                _scenariointerface.SetSpawnParents(_pointsSpawnParent, _objectSpawnParent);
                _scenariointerface.SceneLoaded();
            }
            else
            {
                uiInterface.OnSceneLoaded += () => uiInterface.InitUI(_map);
                uiInterface.OnSceneLoaded += () => _scenariointerface.SetSpawnParents(_pointsSpawnParent, _objectSpawnParent);
                uiInterface.OnSceneLoaded += () => _scenariointerface.SceneLoaded();
            }
        }
        
        // Tell all listeners that the service loaded.
        _mobile_Messinainterface.SceneLoaded();

        if (SceneLoader.Instance.PresetFullyLoaded)
            _aiInterface.Init_Szenario();
        else
            SceneLoader.Instance.OnActivePresetFullyLoaded += _aiInterface.Init_Szenario;
    }

    void OnDestroy()
    {
        // Unsubscribe to interface
        if (_mobile_Messinainterface)
        {
            _mobile_Messinainterface.IsActive = false;
        }
    }
    
    
}
        