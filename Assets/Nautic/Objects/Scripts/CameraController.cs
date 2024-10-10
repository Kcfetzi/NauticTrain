using System;
using System.Collections.Generic;
using UnityEngine;
using ResourceManager = Groupup.ResourceManager;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Cinemachine.CinemachineVirtualCamera _cockpitCamera;

    private ScenarioInterface _scenarioInterface;
    

    private void Awake()
    {
        _scenarioInterface = ResourceManager.GetInterface<ScenarioInterface>();
    }

    private void Start()
    {
        _cockpitCamera.m_Lens.FarClipPlane = _scenarioInterface.ViewRange;
    }
    

    public void SetSelected(bool active)
    {
        _cockpitCamera.Priority = active ? 5 : 0;
    }

    public void SetZoom(int level)
    {
        _cockpitCamera.m_Lens.FieldOfView = level;
    }

    public void SetRotation(Quaternion rotation)
    {
        _cockpitCamera.transform.rotation = rotation;
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
}
