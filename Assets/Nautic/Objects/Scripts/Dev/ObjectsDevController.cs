
using UnityEngine;
using Groupup;

public class ObjectsDevController : MonoBehaviour
{
    private ObjectsInterface _objectsinterface;

    void Awake()
    {
        // Get own interface
        _objectsinterface = ResourceManager.GetInterface<ObjectsInterface>();
        if (!_objectsinterface)
        {
            Debug.Log("Could not subscribe to interface in ObjectsDevController");
        }
        else
        {
            _objectsinterface.OnSceneLoaded += ReadyForAction;
        }
    }

    private void OnDestroy()
    {
        _objectsinterface.OnSceneLoaded -= ReadyForAction;
    }

    private void ReadyForAction()
    {
        Debug.Log("ObjectsInterface is loaded");
    }
}
        