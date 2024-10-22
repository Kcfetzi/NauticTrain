using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum CockpitCameraPosition {LeftBack, Left, Front, Right, RightBack}

public class CockpitModel : MonoBehaviour
{
    [SerializeField] private Transform _uiCamera;
    
    [SerializeField] private Transform _frontCamera;
    [SerializeField] private Transform _leftCamera;
    [SerializeField] private Transform _leftBackCamera;
    [SerializeField] private Transform _rightCamera;
    [SerializeField] private Transform _rightBackCamera;

    private CockpitCameraPosition _activePosition = CockpitCameraPosition.Front;

    public void SetActive(bool active)
    {
        if (!_frontCamera)
            return;
        _uiCamera.position = _frontCamera.position;
        _uiCamera.rotation = _frontCamera.rotation;
    }
    
    public CockpitCameraPosition MoveLeft(NauticObject obj)
    {
        --_activePosition;
        if (_activePosition < 0)
            _activePosition += 5;
        _activePosition = (CockpitCameraPosition)((int)(_activePosition) % 5);

        
        // tell main camera to move
        obj.NauticCameraController.MoveTo(_activePosition);
        // point and ton have no cockpit
        if (_frontCamera != null)
            MoveTo(_activePosition);

        return _activePosition;
    }

    public CockpitCameraPosition MoveRight(NauticObject obj)
    {
        _activePosition = (CockpitCameraPosition)((int)(++_activePosition) % 5);
        
        // tell main camera to move
        obj.NauticCameraController.MoveTo(_activePosition);
        // point and ton have no cockpit
        if (_frontCamera != null)
            MoveTo(_activePosition);
        
        return _activePosition;
    }

    private void MoveTo(CockpitCameraPosition position)
    {
        if (_frontCamera == null)
            return;
        switch (position)
        {
            case CockpitCameraPosition.Front:
                _uiCamera.DOMove(_frontCamera.position, 1f);
                _uiCamera.DORotateQuaternion(_frontCamera.rotation, 1f);
                break;
            case CockpitCameraPosition.Left:
                _uiCamera.DOMove(_leftCamera.position, 1f);
                _uiCamera.DORotateQuaternion(_leftCamera.rotation, 1f);
                break;
            case CockpitCameraPosition.LeftBack:
                _uiCamera.DOMove(_leftBackCamera.position, 1f);
                _uiCamera.DORotateQuaternion(_leftBackCamera.rotation, 1f);
                break;
            case CockpitCameraPosition.Right:
                _uiCamera.DOMove(_rightCamera.position, 1f);
                _uiCamera.DORotateQuaternion(_rightCamera.rotation, 1f);
                break;
            case CockpitCameraPosition.RightBack:
                _uiCamera.DOMove(_rightBackCamera.position, 1f);
                _uiCamera.DORotateQuaternion(_rightBackCamera.rotation, 1f);
                break;
        }
    }
}
