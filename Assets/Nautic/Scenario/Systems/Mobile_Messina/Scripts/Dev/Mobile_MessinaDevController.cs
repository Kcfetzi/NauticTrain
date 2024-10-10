
using System;
using UnityEngine;
using Groupup;

public class Mobile_MessinaDevController : MonoBehaviour
{

    
    private Mobile_MessinaInterface _map_mobileinterface;

    void Awake()
    {
        // Get own interface
        _map_mobileinterface = ResourceManager.GetInterface<Mobile_MessinaInterface>();
        if (!_map_mobileinterface)
        {
            Debug.Log("Could not subscribe to interface in Mobile_MessinaDevController");
        }
        else
        {
            _map_mobileinterface.OnSceneLoaded += ReadyForAction;
        }
    }

    private void OnDestroy()
    {
        _map_mobileinterface.OnSceneLoaded -= ReadyForAction;
    }

    private void ReadyForAction()
    {
        Debug.Log("Mobile_MessinaInterface is loaded");
    }
}
        