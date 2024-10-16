
using Groupup;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _back;
    [SerializeField] private GameObject _next;

    [SerializeField] private MenuPanel _scenarios;
    [SerializeField] private MenuPanel _weather;
    [SerializeField] private MenuPanel _time;
    
    private MainMenuInterface _mainmenuinterface;

    private bool _rdyToStart;
    private int _scenarioChoice = 0;
    private int _weatherChoice = 0;
    private int _timeChoice = 0;

    void Awake()
    {
        // Get own interface and subscribe
        _mainmenuinterface = ResourceManager.GetInterface<MainMenuInterface>();
        if (_mainmenuinterface)
        {

        }
        else
        {
            Debug.Log("Could not subscribe to interface in _mainmenuinterface");
        }
    }

    private void Start()
    {
        _scenarios.Init(SetScenario);
        _weather.Init(SetWeather);
        _time.Init(SetTime);
        
        SetToScenarioMenu();
        
        // Tell all listeners that the service loaded.
        _mainmenuinterface.SceneLoaded();
    }

    void OnDestroy()
    {
        // Unsubscribe to interface
        if (_mainmenuinterface)
        {
            _mainmenuinterface.IsActive = false;
        }
    }

    
    public void SetScenario(int scenario)
    {
        _scenarioChoice = scenario;
        _scenarios.SetButtonActive(scenario);
    }

    public void SetWeather(int weather)
    {
        _weatherChoice = weather;
        _weather.SetButtonActive(weather);
    }

    public void SetTime(int time)
    {
        _timeChoice = time;
        _time.SetButtonActive(time);
    }
    
    public void SetToScenarioMenu()
    {
        _scenarios.gameObject.SetActive(true);
        _weather.gameObject.SetActive(false);
        _time.gameObject.SetActive(false);
        
        _back.SetActive(false);
        _next.SetActive(true);

        _rdyToStart = false;
    }
    
    public void SetToWeatherMenu()
    {        
        _scenarios.gameObject.SetActive(false);
        _weather.gameObject.SetActive(true);
        _time.gameObject.SetActive(true);
        
        _back.SetActive(true);
        _next.SetActive(true);

        _rdyToStart = true;
    }

    public void OnClick_Back()
    {
        SetToScenarioMenu();
    }
    public void OnClick_Next()
    {
        if(!_rdyToStart)
            SetToWeatherMenu();
        else
        {
            ScenarioInterface scenarioInterface = ResourceManager.GetInterface<ScenarioInterface>();
            scenarioInterface.StartWeather = (ScenarioInterface.Weather)_weatherChoice;
            scenarioInterface.StartTime = (ScenarioInterface.SunSet)_timeChoice;

            AIInterface aiInterface = ResourceManager.GetInterface<AIInterface>();
            aiInterface.ScenarioChoice = ++_scenarioChoice;
            
            SceneLoader.Instance.LoadPresetByName("Scenario_Mobile_Messina", true);
        }
    }
}
        