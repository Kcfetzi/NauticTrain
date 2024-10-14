using System.Collections.Generic;
using Groupup;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Mobile_UIController : MonoBehaviour
{
    [SerializeField] private Radar _radar;
    [SerializeField] private Ecdis _ecdis;
    [SerializeField] private CockpitUIController _cockpitUIController;
    [SerializeField] private GameObject _controllPanel;
    [SerializeField] private Diopter _diopterView;
    [SerializeField] private UINavigationController _navigationMenu;
    [SerializeField] private CockpitController _cockpitController;
    
    [SerializeField] private Camera _overlayCamera;
    private Camera _mainCamera;
    
    // take the root interface for ui and register yourself
    private UI_RootInterface _uiinterface;
    private ObjectsInterface _objectsInterface;

    private NauticObject _selectedObject;

    void Awake()
    {
        // Get own interface and subscribe
        _uiinterface = ResourceManager.GetInterface<UI_RootInterface>();
        if (_uiinterface)
        {
            // 
            _uiinterface.OnInitUI += InitUI;
        }
        else
        {
            Debug.Log("Could not subscribe to interface in _mobile_uiinterface");
        }
        
        _objectsInterface = ResourceManager.GetInterface<ObjectsInterface>();
        _objectsInterface.OnNauticObjectSelected += SetSelectedNauticObject;
        
        ScenarioInterface scenarioInterface = ResourceManager.GetInterface<ScenarioInterface>();
        
        if (scenarioInterface.IsActive)
        {
            _mainCamera = Camera.main;
            _mainCamera.GetUniversalAdditionalCameraData().cameraStack.Add(_overlayCamera);
        }
        else
        {
            scenarioInterface.OnSceneLoaded += () =>
            {
                _mainCamera = Camera.main;
                _mainCamera.GetUniversalAdditionalCameraData().cameraStack.Add(_overlayCamera);
            };
        }
    }

    private void Start()
    {
        // Tell all listeners that the service loaded.
        _uiinterface.SceneLoaded();

        if (SceneLoader.Instance.PresetFullyLoaded)
        {

        }
        else
        {

        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe to interface
        if (_uiinterface)
        {
            _uiinterface.IsActive = false;
        }
    }

    public void InitUI(Texture2D map)
    {
        _ecdis.Init(map);
        _radar.Init(map);
    }
    
    public void SetSelectedNauticObject(NauticObject obj)
    {
        _selectedObject = obj;
        
        _cockpitUIController.gameObject.SetActive(_selectedObject);
        _controllPanel.SetActive(_selectedObject);
        
        _ecdis.SetSelectedNauticObject(_selectedObject);
        _radar.SetSelectedNauticObject(_selectedObject);
        _cockpitUIController.SetSelectedNauticObject(_selectedObject);
        _cockpitController.SetCockpitLayout(obj.Data.ShipType);
        _diopterView.SetSelectedNauticObject(_selectedObject);
    }
    
    
    public void ActivateRadar(bool closeUI)
    {
        _navigationMenu.gameObject.SetActive(!closeUI);
        _radar.gameObject.SetActive(!closeUI);
        _diopterView.SetActive(false);
        _ecdis.gameObject.SetActive(false);
        _controllPanel.SetActive(closeUI);
        _cockpitController.SetActive(closeUI);
        _mainCamera.enabled = closeUI;
        _overlayCamera.enabled = closeUI;
        
        // set checkbox in navigationmenu to radar
        if (!closeUI)
            _navigationMenu.SetToRadar();
    }

    // only temp till inputcontroller available
    public void ActivateEcdis(bool closeUI)
    {
        _navigationMenu.gameObject.SetActive(!closeUI);
        _ecdis.gameObject.SetActive(!closeUI);
        _diopterView.SetActive(false);
        _radar.gameObject.SetActive(false);
        _controllPanel.SetActive(closeUI);
        _cockpitController.SetActive(closeUI);
        _mainCamera.enabled = closeUI;
        _overlayCamera.enabled = closeUI;
        
        // set checkbox in navigationmenu to radar
        if (!closeUI)
            _navigationMenu.SetToEcdis();
    }

    public void ActivateDiopter(bool closeUI)
    {
        _navigationMenu.gameObject.SetActive(!closeUI);
        _diopterView.SetActive(!closeUI);
        _ecdis.gameObject.SetActive(false);
        _radar.gameObject.SetActive(false);
        _controllPanel.SetActive(closeUI);
        _cockpitController.SetActive(closeUI);
        _mainCamera.enabled = true;
        _overlayCamera.enabled = closeUI;
        
        // set checkbox in navigationmenu to radar
        if (!closeUI)
            _navigationMenu.SetToDiopter();
    }

    public void ToggleLightsSignals()
    {
        PopupManager.Instance.ShowLightsSignalsPopup();
    }

    public void ToggleSoundsSignals()
    {
       PopupManager.Instance.ShowSoundsSignalPopup();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Answer firstAnswer = new Answer("Falsche Antwort", false);
            Answer secondAnswer = new Answer("Falsche Antwort", false);
            Answer thirdAnswer = new Answer("Richtige Antwort", true);
            Answer fourthAnswer = new Answer("Richtige Antwort", true);
            
            
            PopupManager.Instance.ShowQuestionPopup("Fragentitel", "Hier könnte ihre Frage stehen", new List<Answer>(){firstAnswer, secondAnswer, thirdAnswer,fourthAnswer}, null);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _uiinterface.SetFunkMessage("Eine sehr lange Testmessage");
        }
        
    }
    
}
        