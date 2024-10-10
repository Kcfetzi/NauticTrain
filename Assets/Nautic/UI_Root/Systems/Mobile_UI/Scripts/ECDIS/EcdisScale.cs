using Groupup;
using TMPro;
using UnityEngine;
public class EcdisScale : MonoBehaviour
{
    [SerializeField] private RectTransform _lenthIndicator;
    [SerializeField] private RectTransform _leftMark;
    
    [SerializeField] private TMP_Text _lengthText;

    [SerializeField] private float _lenthMaxPixel;
    private float _ecdisToRealWorldRatio;

    private UI_RootInterface _uiRootInterface;
    private ScenarioInterface _scenarioInterface;
    
    private void Awake()
    {
        _scenarioInterface = ResourceManager.GetInterface<ScenarioInterface>();
        _uiRootInterface = ResourceManager.GetInterface<UI_RootInterface>();

        if (_scenarioInterface.IsActive)
        {
            if (_uiRootInterface.IsActive)
            {
                ServicesLoaded();
            }
        }
        else
        {
            _uiRootInterface.OnSceneLoaded += ServicesLoaded;
            _scenarioInterface.OnSceneLoaded += ServicesLoaded;
        }
    }

    private void ServicesLoaded()
    {
        _scenarioInterface = ResourceManager.GetInterface<ScenarioInterface>();
        _uiRootInterface = ResourceManager.GetInterface<UI_RootInterface>();
        
        if (!_scenarioInterface.IsActive || !_uiRootInterface.IsActive)
        {
            return;
        }

        Vector3 terrainSize = _scenarioInterface.MapSize;
        Vector2 mapSize = _uiRootInterface.EcdisMapSize;
        
        _ecdisToRealWorldRatio = mapSize.x / ((terrainSize.x * (float)_scenarioInterface.UnityXInMeters * 0.001f) * 0.53996f);
    }
    
    private void Update()
    {
        if (5 * _ecdisToRealWorldRatio * _uiRootInterface.EcdisMapScale.x  < _lenthMaxPixel)
        {
            _lenthIndicator.sizeDelta = new Vector2(5 * _ecdisToRealWorldRatio * _uiRootInterface.EcdisMapScale.x, 5);
            _lengthText.text = "5 sm";

        } else if (2 * _ecdisToRealWorldRatio * _uiRootInterface.EcdisMapScale.x < _lenthMaxPixel)
        {
            _lenthIndicator.sizeDelta = new Vector2(2 * _ecdisToRealWorldRatio * _uiRootInterface.EcdisMapScale.x, 5);
            _lengthText.text = "2 sm";
        }else if (1 * _ecdisToRealWorldRatio * _uiRootInterface.EcdisMapScale.x < _lenthMaxPixel)
        {
            
            _lenthIndicator.sizeDelta = new Vector2(1 * _ecdisToRealWorldRatio * _uiRootInterface.EcdisMapScale.x, 5);
            _lengthText.text = "1 sm";
        }else if (0.5f * _ecdisToRealWorldRatio * _uiRootInterface.EcdisMapScale.x < _lenthMaxPixel)
        {
            _lenthIndicator.sizeDelta = new Vector2(0.5f * _ecdisToRealWorldRatio * _uiRootInterface.EcdisMapScale.x, 5);
            _lengthText.text = "0.5 sm";
        }
        else if (0.3f * _ecdisToRealWorldRatio * _uiRootInterface.EcdisMapScale.x < _lenthMaxPixel)
        {
            _lenthIndicator.sizeDelta = new Vector2(0.3f * _ecdisToRealWorldRatio * _uiRootInterface.EcdisMapScale.x, 5);
            _lengthText.text = "0.3 sm";
        }
        else if (0.2f * _ecdisToRealWorldRatio * _uiRootInterface.EcdisMapScale.x < _lenthMaxPixel)
        {
            _lenthIndicator.sizeDelta = new Vector2(0.2f * _ecdisToRealWorldRatio * _uiRootInterface.EcdisMapScale.x, 5);
            _lengthText.text = "0.2 sm";
        }
        else if (0.1f * _ecdisToRealWorldRatio * _uiRootInterface.EcdisMapScale.x < _lenthMaxPixel)
        {
            _lenthIndicator.sizeDelta = new Vector2(0.1f * _ecdisToRealWorldRatio * _uiRootInterface.EcdisMapScale.x, 5);
            _lengthText.text = "0.1 sm";
        }
        
        _leftMark.anchoredPosition = new Vector2(_lenthIndicator.anchoredPosition.x - _lenthIndicator.rect.width, 5);
    }
}
