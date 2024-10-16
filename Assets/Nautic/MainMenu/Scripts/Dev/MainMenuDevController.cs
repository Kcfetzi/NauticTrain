
using UnityEngine;
using Groupup;

public class MainMenuDevController : MonoBehaviour
{
    private MainMenuInterface _mainmenuinterface;

    void Awake()
    {
        // Get own interface
        _mainmenuinterface = ResourceManager.GetInterface<MainMenuInterface>();
        if (!_mainmenuinterface)
        {
            Debug.Log("Could not subscribe to interface in MainMenuDevController");
        }
        else
        {
            _mainmenuinterface.OnSceneLoaded += ReadyForAction;
        }
    }

    private void OnDestroy()
    {
        _mainmenuinterface.OnSceneLoaded -= ReadyForAction;
    }

    private void ReadyForAction()
    {
        Debug.Log("MainMenuInterface is loaded");
    }
}
        