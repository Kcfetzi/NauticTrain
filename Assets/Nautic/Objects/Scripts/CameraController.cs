using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;

    private static CameraController _instance;

    public static CameraController Instance => _instance;

    private void Awake()
    {
        // Verhindere, dass mehrere Instanzen dieses Objekts erstellt werden
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject); // Zerstöre doppelte Instanzen
        }
    }

    public void AlignCameraTo(Transform transform, bool instant = false)
    {
        _mainCamera.transform.parent = transform;

        if (instant)
        {
            _mainCamera.transform.localPosition = Vector3.zero;
            _mainCamera.transform.rotation = Quaternion.identity;
        }
        else
        {
            _mainCamera.transform.DOLocalMove(Vector3.zero, 1f);
            _mainCamera.transform.DOLocalRotateQuaternion(Quaternion.identity, 1f);
        }
    }

    public void SetFieldOfView(int level)
    {
        _mainCamera.fieldOfView = level;
    }
    
    public void SetOverlayCamera(Camera overlayCamera)
    {
        _mainCamera.GetUniversalAdditionalCameraData().cameraStack.Add(overlayCamera);
    }

    public void SetActive(bool active)
    {
        _mainCamera.enabled = active;
    }
}
