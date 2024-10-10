
using Groupup;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private AIInterface _aiinterface;

    void Awake()
    {
        // Get own interface and subscribe
        _aiinterface = ResourceManager.GetInterface<AIInterface>();
        if (_aiinterface)
        {

        }
        else
        {
            Debug.Log("Could not subscribe to interface in _aiinterface");
        }
    }

    private void Start()
    {
        // Tell all listeners that the service loaded.
        _aiinterface.SceneLoaded();
    }

    void OnDestroy()
    {
        // Unsubscribe to interface
        if (_aiinterface)
        {
            _aiinterface.IsActive = false;
        }
    }
}
        