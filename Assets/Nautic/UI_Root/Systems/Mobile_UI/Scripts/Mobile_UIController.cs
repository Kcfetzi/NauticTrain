using System.Collections.Generic;
using Groupup;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;
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
    
    [SerializeField] private QuestionController _questionController;
    
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
            _uiinterface.OnOpenDiopter += OpenDiopter;
            _uiinterface.OnCloseDiopter += CloseDiopter;
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
            _uiinterface.Reset();
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
    
    // ui navigation klick radar
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

    // ui navigation klick diopter
    public void ActivateDiopter(bool closeUI)
    {
        _navigationMenu.gameObject.SetActive(!closeUI);
        _diopterView.gameObject.SetActive(!closeUI);
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

    // this is the channel method. It is used by the questionpopup to get context
    private void OpenDiopter(Question question, UnityAction submitAction)
    {
        _navigationMenu.gameObject.SetActive(false);
        _diopterView.Init(question, submitAction);
        _ecdis.gameObject.SetActive(false);
        _radar.gameObject.SetActive(false);
        _controllPanel.SetActive(false);
        _cockpitController.SetActive(false);
        _mainCamera.enabled = true;
        _overlayCamera.enabled = false;
    }

    private void CloseDiopter()
    {
        _navigationMenu.gameObject.SetActive(false);
        _diopterView.Init(null, null);
        _ecdis.gameObject.SetActive(false);
        _radar.gameObject.SetActive(false);
        _controllPanel.SetActive(true);
        _cockpitController.SetActive(true);
        _mainCamera.enabled = true;
        _overlayCamera.enabled = true;
    }

    #region Clicklistener

    public void OnClick_LightsSignals()
    {
        PopupManager.Instance.ShowLightsSignalsPopup(null, null);
    }

    public void OnClick_SoundsSignals()
    {
        PopupManager.Instance.ShowSoundsSignalPopup(null, null);
    }

    public void OnClick_Settings()
    {
        PopupManager.Instance.ShowSettingsPopup();
    }

    public void OnClickClosePopup()
    {
        PopupManager.Instance.Hide();
    }

    public void OnClick_Question()
    {
        PopupManager.Instance.ShowQuestionPopup(_questionController.GetRandomQuestion(), null, "");
    }

    public void OnClick_Funk()
    {
        _uiinterface.SetFunkMessage("Nachricht im offenen Funkverkehr", true, false);
        //_uiinterface.SetFunkMessage("Persönliche Nachricht vom Funk", false, true);
        PopupManager.Instance.ShowCommunicationPopup("Persönliche Nachricht über Funk", false, false, 3, AnswerFunk);
    }

    public void AnswerFunk()
    {
        //_uiinterface.SetFunkMessage("Persönliche Antwort vom Funk", false, false);
        PopupManager.Instance.ShowCommunicationPopup("Persönliche Antwort über Funk", false, true, 3);
    }
    
    #endregion
    
    
}
        