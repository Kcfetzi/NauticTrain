
using System;
using UnityEngine;
using Groupup;

public class Mobile_UIDevController : MonoBehaviour
{
    private Mobile_UIInterface _mobile_uiinterface;
    private UI_RootInterface _uiinterface;
    
    void Awake()
    {
        // Get own interface
        _mobile_uiinterface = ResourceManager.GetInterface<Mobile_UIInterface>();
        _uiinterface = ResourceManager.GetInterface<UI_RootInterface>();
        if (!_mobile_uiinterface)
        {
            Debug.Log("Could not subscribe to interface in Mobile_UIDevController");
        }
        else
        {
            _mobile_uiinterface.OnSceneLoaded += ReadyForAction;
        }
    }

    private void OnDestroy()
    {
        _mobile_uiinterface.OnSceneLoaded -= ReadyForAction;
    }

    private void ReadyForAction()
    {
        Debug.Log("Mobile_UIInterface is loaded");
    }

    private void Update()
    {

    }
}
        