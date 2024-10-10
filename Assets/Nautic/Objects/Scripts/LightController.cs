using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    [Header("Lights")]
    [SerializeField] private GameObject _topLight;
    [SerializeField] private GameObject _midLight;
    [SerializeField] private GameObject _midLeftLight;
    [SerializeField] private GameObject _midRightLight;
    [SerializeField] private GameObject _botLight;
    [SerializeField] private GameObject _leftSideLight;
    [SerializeField] private GameObject _rightSideLight;

    private MeshRenderer _topLightRenderer;
    private MeshRenderer _midLightRenderer;
    private MeshRenderer _midLeftLightRenderer;
    private MeshRenderer _midRightLightRenderer;
    private MeshRenderer _botLightRenderer;
    private MeshRenderer _leftSideLightRenderer;
    private MeshRenderer _rightSideLightRenderer;

    [SerializeField] private Material _whiteLightMaterial;
    [SerializeField] private Material _greenLightMaterial;
    [SerializeField] private Material _redLightMaterial;

    
    [Header("Symbols")]
    [SerializeField] private GameObject _topSymbol;
    [SerializeField] private GameObject _topLeftSymbol;
    [SerializeField] private GameObject _topRightSymbol;
    [SerializeField] private GameObject _midLeftSymbol;
    [SerializeField] private GameObject _midBotLeftSymbol;

    [SerializeField] private GameObject _triangleUpPrefab;
    [SerializeField] private GameObject _triangleDownPrefab;
    [SerializeField] private GameObject _circlePrefab;
    [SerializeField] private GameObject _hashPrefab;
    [SerializeField] private GameObject _rectAnglePrefab;

    private void Awake()
    {
        _topLightRenderer = _topLight.GetComponent<MeshRenderer>();
        _topLight.SetActive(false);
        
        _midLightRenderer = _midLight.GetComponent<MeshRenderer>();
        _midLight.SetActive(false);
        
        _midLeftLightRenderer = _midLeftLight.GetComponent<MeshRenderer>();
        _midLeftLight.SetActive(false);
        
        _midRightLightRenderer = _midRightLight.GetComponent<MeshRenderer>();
        _midRightLight.SetActive(false);
        
        _botLightRenderer = _botLight.GetComponent<MeshRenderer>();
        _botLight.SetActive(false);
        
        _leftSideLightRenderer = _leftSideLight.GetComponent<MeshRenderer>();
        _leftSideLight.SetActive(false);
        
        _rightSideLightRenderer = _rightSideLight.GetComponent<MeshRenderer>();
        _rightSideLight.SetActive(false);
        
        
        _topSymbol.SetActive(false);
        _topLeftSymbol.SetActive(false);
        _topRightSymbol.SetActive(false);
        _midLeftSymbol.SetActive(false);
        _midBotLeftSymbol.SetActive(false);
    }

    /**
     * The last int is a bool telling if lights or symbols are used
     */
    public void SetLightsAndSymbols(List<int> lightsOrSymbols)
    {
        // player use lights
        if (lightsOrSymbols[lightsOrSymbols.Count - 1] == 1)
        {
            ToggleLights(lightsOrSymbols);
        }
        // player use symbols
        else
        {
            ToggleSymbols(lightsOrSymbols);
        }
    }
    
    /**
     * All lights with color. Top[0], Mid[1], MidLeft[2], MidRight[3] Bot[4], Left[5], Right[6]
     * The number tell about color defined as enum in LightsButton
     */
    public void ToggleLights(List<int> lights)
    {
        // toplight
        if (lights[0] > 0)
        {
            _topLight.SetActive(true);
            switch (lights[0])
            {
                // white
                case 1:
                    _topLightRenderer.material = _whiteLightMaterial;
                    break;
                // red
                case 2:
                    _topLightRenderer.material = _redLightMaterial;
                    break;
                // green
                case 3:
                    _topLightRenderer.material = _greenLightMaterial;
                    break;
            }
        }
        else
        {
            _topLight.SetActive(false);
        }
        
        // midlight
        if (lights[1] > 0)
        {
            _midLight.SetActive(true);
            switch (lights[1])
            {
                // white
                case 1:
                    _midLightRenderer.material = _whiteLightMaterial;
                    break;
                // red
                case 2:
                    _midLightRenderer.material = _redLightMaterial;
                    break;
                // green
                case 3:
                    _midLightRenderer.material = _greenLightMaterial;
                    break;
            }
        }
        else
        {
            _midLight.SetActive(false);
        }
        
        // midLeftlight
        if (lights[2] > 0)
        {
            _midLeftLight.SetActive(true);
            switch (lights[2])
            {
                // white
                case 1:
                    _midLeftLightRenderer.material = _whiteLightMaterial;
                    break;
                // red
                case 2:
                    _midLeftLightRenderer.material = _redLightMaterial;
                    break;
                // green
                case 3:
                    _midLeftLightRenderer.material = _greenLightMaterial;
                    break;
            }
        }
        else
        {
            _midLeftLight.SetActive(false);
        }
        
        // midRightlight
        if (lights[3] > 0)
        {
            _midRightLight.SetActive(true);
            switch (lights[3])
            {
                // white
                case 1:
                    _midRightLightRenderer.material = _whiteLightMaterial;
                    break;
                // red
                case 2:
                    _midRightLightRenderer.material = _redLightMaterial;
                    break;
                // green
                case 3:
                    _midRightLightRenderer.material = _greenLightMaterial;
                    break;
            }
        }
        else
        {
            _midRightLight.SetActive(false);
        }
        
        // botlight
        if (lights[4] > 0)
        {
            _botLight.SetActive(true);
            switch (lights[4])
            {
                // white
                case 1:
                    _botLightRenderer.material = _whiteLightMaterial;
                    break;
                // red
                case 2:
                    _botLightRenderer.material = _redLightMaterial;
                    break;
                // green
                case 3:
                    _botLightRenderer.material = _greenLightMaterial;
                    break;
            }
        }
        else
        {
            _botLight.SetActive(false);
        }
        
        // leftSidelight
        if (lights[5] > 0)
        {
            _leftSideLight.SetActive(true);
            switch (lights[5])
            {
                // white
                case 1:
                    _leftSideLightRenderer.material = _whiteLightMaterial;
                    break;
                // red
                case 2:
                    _leftSideLightRenderer.material = _redLightMaterial;
                    break;
                // green
                case 3:
                    _leftSideLightRenderer.material = _greenLightMaterial;
                    break;
            }
        }
        else
        {
            _leftSideLight.SetActive(false);
        }
        
        // rightSidelight
        if (lights[6] > 1)
        {
            _rightSideLight.SetActive(true);
            switch (lights[6])
            {
                // white
                case 1:
                    _rightSideLightRenderer.material = _whiteLightMaterial;
                    break;
                // red
                case 2:
                    _rightSideLightRenderer.material = _redLightMaterial;
                    break;
                // green
                case 3:
                    _rightSideLightRenderer.material = _greenLightMaterial;
                    break;
            }
        }
        else
        {
            _rightSideLight.SetActive(false);
        }
    }

    /**
     * All symbols with shape, [0] = top, [1] = topLeft, [2] = topRight, [3] = midLeft, [4] = midbotLeft, [5] = bottomLeft, [6] = bottomRight,
     * The number tell about symbol defined as enum in SymbolsButton
     */
    public void ToggleSymbols(List<int> symbols)
    {
        if (symbols[0] > 0)
        {
            _topSymbol.SetActive(true);
            foreach (Transform child in _topSymbol.transform)
            {
                Destroy(child.gameObject);
            }
            
            switch (symbols[0])
            {
                case 1:
                    Instantiate(_triangleUpPrefab, _topSymbol.transform);
                    break;
                case 2:
                    Instantiate(_triangleDownPrefab, _topSymbol.transform);
                    break;
                case 3:
                    Instantiate(_circlePrefab, _topSymbol.transform);
                    break;
                case 4:
                    Instantiate(_hashPrefab, _topSymbol.transform);
                    break;
                case 5:
                    Instantiate(_rectAnglePrefab, _topSymbol.transform);
                    break;
            }
        }
        else
        {
            _topSymbol.SetActive(false);
        }
        
        if (symbols[1] > 0)
        {
            _topLeftSymbol.SetActive(true);
            foreach (Transform child in _topLeftSymbol.transform)
            {
                Destroy(child.gameObject);
            }
            
            switch (symbols[1])
            {
                case 1:
                    Instantiate(_triangleUpPrefab, _topLeftSymbol.transform);
                    break;
                case 2:
                    Instantiate(_triangleDownPrefab, _topLeftSymbol.transform);
                    break;
                case 3:
                    Instantiate(_circlePrefab, _topLeftSymbol.transform);
                    break;
                case 4:
                    Instantiate(_hashPrefab, _topLeftSymbol.transform);
                    break;
                case 5:
                    Instantiate(_rectAnglePrefab, _topLeftSymbol.transform);
                    break;
            }
        }
        else
        {
            _topLeftSymbol.SetActive(false);
        }
        
        if (symbols[2] > 0)
        {
            _topRightSymbol.SetActive(true);
            foreach (Transform child in _topRightSymbol.transform)
            {
                Destroy(child.gameObject);
            }
            
            switch (symbols[2])
            {
                case 1:
                    Instantiate(_triangleUpPrefab, _topRightSymbol.transform);
                    break;
                case 2:
                    Instantiate(_triangleDownPrefab, _topRightSymbol.transform);
                    break;
                case 3:
                    Instantiate(_circlePrefab, _topRightSymbol.transform);
                    break;
                case 4:
                    Instantiate(_hashPrefab, _topRightSymbol.transform);
                    break;
                case 5:
                    Instantiate(_rectAnglePrefab, _topRightSymbol.transform);
                    break;
            }
        }
        else
        {
            _topRightSymbol.SetActive(false);
        }
        
        if (symbols[3] > 0)
        {
            _midLeftSymbol.SetActive(true);
            foreach (Transform child in _midLeftSymbol.transform)
            {
                Destroy(child.gameObject);
            }
            
            switch (symbols[3])
            {
                case 1:
                    Instantiate(_triangleUpPrefab, _midLeftSymbol.transform);
                    break;
                case 2:
                    Instantiate(_triangleDownPrefab, _midLeftSymbol.transform);
                    break;
                case 3:
                    Instantiate(_circlePrefab, _midLeftSymbol.transform);
                    break;
                case 4:
                    Instantiate(_hashPrefab, _midLeftSymbol.transform);
                    break;
                case 5:
                    Instantiate(_rectAnglePrefab, _midLeftSymbol.transform);
                    break;
            }
        }
        else
        {
            _midLeftSymbol.SetActive(false);
        }
        
        if (symbols[4] > 0)
        {
            _midBotLeftSymbol.SetActive(true);
            foreach (Transform child in _midBotLeftSymbol.transform)
            {
                Destroy(child.gameObject);
            }
            
            switch (symbols[4])
            {
                case 1:
                    Instantiate(_triangleUpPrefab, _midBotLeftSymbol.transform);
                    break;
                case 2:
                    Instantiate(_triangleDownPrefab, _midBotLeftSymbol.transform);
                    break;
                case 3:
                    Instantiate(_circlePrefab, _midBotLeftSymbol.transform);
                    break;
                case 4:
                    Instantiate(_hashPrefab, _midBotLeftSymbol.transform);
                    break;
                case 5:
                    Instantiate(_rectAnglePrefab, _midBotLeftSymbol.transform);
                    break;
            }
        }
        else
        {
            _midBotLeftSymbol.SetActive(false);
        }
    }
}
