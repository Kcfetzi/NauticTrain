using System;
using Groupup;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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

    [SerializeField] private Button _markObjectButton;
    
    private NauticObject _selectedObject;
    private NauticObject _focusObject;

    // this is only if diopter was openend in kontext of a question
    private Question _kontextQuestion;
    private UnityAction _questionCallback;
    
    private Camera _mainCamera;
    
    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        _ruderKnob.Init(Rotate);
    }

    // if a callback is given, diopter was opened from a questionkontext
    public void Init(Question question, UnityAction questionCallback)
    {
        _kontextQuestion = question;
        _questionCallback = questionCallback;
        SetActive(question != null);
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(_clickZone, Input.mousePosition))
                _focusObject = _selectedObject.CameraController.RaycastIntoSzene(Input.mousePosition);
        }
    }

    public void LateUpdate()
    {
        if (_focusObject)
        {
            _markObjectButton.interactable = true;
            
            Vector3 directionToTarget = _focusObject.RotationObject.position - _selectedObject.RotationObject.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            _selectedObject.CameraController.SetRotation(targetRotation);
            Vector3 directionWithOffset = _selectedObject.RotationObject.localRotation.eulerAngles - targetRotation.eulerAngles;
            _courseText.text = Mathf.Abs(directionWithOffset.y).ToString("F0") + "\u00B0" + (directionWithOffset.y > 0 ? " SB" : " BB");
            _ruderKnob.SetRotation(directionWithOffset.y);
            
            MeshRenderer renderer = _focusObject.Renderer;
            if (renderer != null)
            {
                Vector3 minScreenPoint = _mainCamera.WorldToScreenPoint(renderer.bounds.min);
                Vector3 maxScreenPoint = _mainCamera.WorldToScreenPoint(renderer.bounds.max);

                // Setze die Position und Größe des UI-Rechtecks
                Vector2 size = minScreenPoint - maxScreenPoint;
                _marker.sizeDelta = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));

                // Position des Rechtecks auf die Mitte setzen
                _marker.position = new Vector3(minScreenPoint.x - size.x / 2, minScreenPoint.y - size.y / 2, 0);
            }
        }
        else
        {
            _marker.sizeDelta = Vector3.zero;
            _markObjectButton.interactable = false;
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
        _selectedObject.CameraController.SetLocalRotationRotation(Quaternion.Euler(new Vector3(0, rotation, 0)));
    }

    // user clickt to mark a target, if a _markcallback is set, diopter was openend in kontext of a question. So return the choise. Otherwise set a markentry
    public void SetEntry()
    {
        if (!_focusObject)
            return;

        // if entry set with contextQuestion, close diopter and show questionpopup again
        if (_kontextQuestion != null)
        {
            PopupManager.Instance.ShowQuestionPopup(_kontextQuestion, _questionCallback, _focusObject.Data.ObjectName);
            ResourceManager.GetInterface<UI_RootInterface>().CloseDiopter();
        }
        else
        {
            DiopterEntry entry = Instantiate(_diopterEntry, _entryHolder);
            entry.Init(_focusObject.Data.ObjectName + " | " + _courseText.text);
        }
    }
}
