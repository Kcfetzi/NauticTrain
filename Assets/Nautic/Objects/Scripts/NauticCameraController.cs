using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using ResourceManager = Groupup.ResourceManager;

public class NauticCameraController : MonoBehaviour
{
    private Camera _mainCamera;
    private Transform _mainCameraTransform;
    
    [SerializeField] private Transform _frontCamera;
    [SerializeField] private Transform _leftCamera;
    [SerializeField] private Transform _leftBackCamera;
    [SerializeField] private Transform _rightCamera;
    [SerializeField] private Transform _rightBackCamera;

    private ScenarioInterface _scenarioInterface;
    

    private void Awake()
    {
        _scenarioInterface = ResourceManager.GetInterface<ScenarioInterface>();
    }
    
    public void SetSelected(bool active)
    {
        CameraController.Instance.AlignCameraTo(_frontCamera);
    }

    public void SetLocalRotation(Quaternion rotation)
    {
        _frontCamera.transform.localRotation = rotation;
    }

    public void SetRotation(Quaternion rotation)
    {
        _frontCamera.transform.rotation = rotation;
    }

    public NauticObject RaycastIntoSzene(Vector2 touchpos)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchpos);
        RaycastHit hit;
        NauticObject hitObject = null;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.transform.parent && hit.collider.transform.parent.parent)
                hitObject = hit.collider.transform.parent.parent.GetComponent<NauticObject>();
        }

        return hitObject;
    }
    
    public void MoveTo(CockpitCameraPosition position)
    {
        switch (position)
        {
            case CockpitCameraPosition.Front:
                CameraController.Instance.AlignCameraTo(_frontCamera);
                break;
            case CockpitCameraPosition.Left:
                CameraController.Instance.AlignCameraTo(_leftCamera);
                break;
            case CockpitCameraPosition.LeftBack:
                CameraController.Instance.AlignCameraTo(_leftBackCamera);
                break;
            case CockpitCameraPosition.Right:
                CameraController.Instance.AlignCameraTo(_rightCamera);
                break;
            case CockpitCameraPosition.RightBack:
                CameraController.Instance.AlignCameraTo(_rightBackCamera);
                break;
        }
    }
}
