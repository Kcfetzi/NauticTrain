
using Groupup;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI.Extensions;

public class ObjectsController : MonoBehaviour
{
    private ObjectsInterface _objectsinterface;

    private NauticObject _selectedNauticObject;
    void Awake()
    {
        // Get own interface and subscribe
        _objectsinterface = ResourceManager.GetInterface<ObjectsInterface>();
        if (_objectsinterface)
        {
            _objectsinterface.OnNauticObjectSelected += SetSelectedNauticObject;
            _objectsinterface.OnDeleteNauticObject += DeleteNauticObject;
        }
        else
        {
            Debug.Log("Could not subscribe to interface in _objectsinterface");
        }
        
        ScenarioInterface scenarioInterface = ResourceManager.GetInterface<ScenarioInterface>();
        if (scenarioInterface.IsActive)
        {
            _objectsinterface.Reset();
        }
        else
        {
            scenarioInterface.OnSceneLoaded += () => _objectsinterface.Reset();
        }
    }

    private void Start()
    {
        // Tell all listeners that the service loaded.
        _objectsinterface.SceneLoaded();
    }

    void OnDestroy()
    {
        // Unsubscribe to interface
        if (_objectsinterface)
        {
            _objectsinterface.IsActive = false;
            _objectsinterface.OnNauticObjectSelected = null;
            _objectsinterface.OnDeleteNauticObject = null;
        }
    }

    public void SetSelectedNauticObject(NauticObject obj)
    {
        if (_selectedNauticObject)
            _selectedNauticObject.SetSelected(false);
        
        _selectedNauticObject = obj;
        _selectedNauticObject.SetSelected(true);
    }

    public void DeleteNauticObject(NauticObject obj)
    {
        if (_selectedNauticObject == obj)
            _selectedNauticObject = null;
    }
}
        