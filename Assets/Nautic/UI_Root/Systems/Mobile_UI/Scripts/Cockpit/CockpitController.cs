using System;
using Groupup;
using UnityEngine;

public class CockpitController : MonoBehaviour
{
    [SerializeField] private Mobile_UIController _uiController;   
    
    [SerializeField] private CockpitModel _minesweeperCockpit;
    [SerializeField] private CockpitModel _cargoCockpit;

    private CockpitModel _activeCockpitModel;
    private NauticObject _selectedObject;
    
    private void Awake()
    {
        DisableAllModels();
    }

    private void DisableAllModels()
    {
        _minesweeperCockpit.gameObject.SetActive(false);
        _cargoCockpit.gameObject.SetActive(false);
    }
    
    public void SetCockpitLayout(NauticObject obj)
    {
        if (_activeCockpitModel)
        {
            _activeCockpitModel.SetActive(false);
        }
        _selectedObject = obj;
        DisableAllModels();
        switch (obj.Data.ShipType)
        {
            case NauticType.Cargo:
                _activeCockpitModel = _cargoCockpit;
                break;
            case NauticType.MineSweeper:
                _activeCockpitModel = _minesweeperCockpit;
                break;
            default:
                _activeCockpitModel = null;
                break;
        }

        if (_activeCockpitModel)
        {
            _activeCockpitModel.gameObject.SetActive(true);
            _activeCockpitModel.SetActive(true);
        }
    }

    private void LateUpdate()
    {
        if (!_selectedObject)
            return;

        transform.rotation = _selectedObject.RotationObject.rotation;
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void OnClick_Left()
    {
        if (!_activeCockpitModel)
        {
            return;
        }
        
        CockpitCameraPosition pos = _activeCockpitModel.MoveLeft(_selectedObject);
        _uiController.FreeLook(pos != CockpitCameraPosition.Front);
    }
    
    public void OnClick_Right()
    {
        if (!_activeCockpitModel)
        {
            return;
        }
        
        CockpitCameraPosition pos = _activeCockpitModel.MoveRight(_selectedObject);
        _uiController.FreeLook(pos != CockpitCameraPosition.Front);
    }
}
