using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Diopter : MonoBehaviour
{
    [SerializeField] private Slider _zoomSlider;
    [SerializeField] private RuderKnob _ruderKnob;

    [SerializeField] private RectTransform _marker;
    [SerializeField] private TMP_Text _courseText;

    [SerializeField] private Transform _entryHolder;
    [SerializeField] private DiopterEntry _diopterEntry;
    
    [SerializeField] private RectTransform _clickZone;
    
    private NauticObject _selectedObject;
    private NauticObject _focusObject;

    private Camera _mainCamera;
    
    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        _ruderKnob.Init(Rotate);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(_clickZone, Input.mousePosition))
                _focusObject = _selectedObject.CameraController.RaycastIntoSzene(Input.mousePosition);
        }
        
        if (_focusObject)
        {
            Vector3 directionToTarget = _focusObject.RotationObject.position - _selectedObject.RotationObject.position;
            float angle = Vector3.SignedAngle(_selectedObject.RotationObject.forward, directionToTarget, Vector3.up);
            _courseText.text = Mathf.Abs(angle).ToString("F1") + (angle > 0 ? "SB" : "BB");
            
            Vector3 direction = _focusObject.transform.position - _selectedObject.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            _selectedObject.CameraController.SetRotation(targetRotation);
            
            MeshRenderer renderer = _focusObject.Renderer;
            if (renderer != null)
            {
                Vector3 minScreenPoint = _mainCamera.WorldToScreenPoint(renderer.bounds.min);
                Vector3 maxScreenPoint = _mainCamera.WorldToScreenPoint(renderer.bounds.max);

                // Setze die Position und Größe des UI-Rechtecks
                Vector2 size = minScreenPoint - maxScreenPoint;
                _marker.sizeDelta = new Vector2(Mathf.Abs(size.x) * 2, Mathf.Abs(size.y) * 2);

                // Position des Rechtecks auf die Mitte setzen
                _marker.position = new Vector3(minScreenPoint.x - size.x / 2, minScreenPoint.y - size.y / 2, 0);
            }
        }
        else
        {
            _marker.sizeDelta = Vector3.zero;
        }
    }
    
    
    public void SetSelectedNauticObject(NauticObject obj)
    {
        _selectedObject = obj;
        _zoomSlider.onValueChanged.AddListener(Zoom);
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
        if (active)
        {
            Rotate(Mathf.RoundToInt(_ruderKnob.Rotation));
            _selectedObject.CameraController.SetZoom(Mathf.RoundToInt(_zoomSlider.value));
        }
        else
        {
            _selectedObject.CameraController.SetZoom(60);
            Rotate(0);
        }
    }

    private void Zoom(float level)
    {
        _selectedObject.CameraController.SetZoom(Mathf.RoundToInt(level));
    }

    private void Rotate(int rotation)
    {
        _selectedObject.CameraController.SetRotation(Quaternion.Euler(new Vector3(0, -rotation, 0)));
        _courseText.text = Mathf.Abs(rotation).ToString("F1") + (rotation > 0 ? "SB" : "BB");
    }

    public void SetEntry()
    {
        if (!_focusObject)
            return;

        DiopterEntry entry = Instantiate(_diopterEntry, _entryHolder);
        entry.Init(_focusObject.Data.ObjectName + " | " + _courseText.text);
    }
}
