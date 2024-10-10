
using UnityEngine;
using Groupup;

public class AIDevController : MonoBehaviour
{
    private AIInterface _aiinterface;

    void Awake()
    {
        // Get own interface
        _aiinterface = ResourceManager.GetInterface<AIInterface>();
        if (!_aiinterface)
        {
            Debug.Log("Could not subscribe to interface in AIDevController");
        }
        else
        {
            _aiinterface.OnSceneLoaded += ReadyForAction;
        }
    }

    private void OnDestroy()
    {
        _aiinterface.OnSceneLoaded -= ReadyForAction;
    }

    private void ReadyForAction()
    {
        Debug.Log("AIInterface is loaded");
    }
}
        