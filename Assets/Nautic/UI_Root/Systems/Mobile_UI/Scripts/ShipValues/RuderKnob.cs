using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RuderKnob : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private CockpitUIController _cockpitUIController;
    [SerializeField] private RectTransform _ruderHandleImage;
    
    [SerializeField] private TMP_Text _rotationText;
    
    [SerializeField] private float _rotationSpeed;
    
    private Vector3 _rotationVector = Vector3.zero;
    
    private bool _isDragging;

    public float Rotation => Mathf.Round((45f / 105f) * _rotationVector.z);
    
    void Update()
    {
        if (_isDragging)
        {
            float mouseX = Input.GetAxis("Mouse X");
            _rotationVector.z -= mouseX * _rotationSpeed;
            _rotationVector.z = Mathf.Round(_rotationVector.z);
            _rotationVector.z = Mathf.Clamp(_rotationVector.z, -105, 105);

            _ruderHandleImage.rotation = Quaternion.Euler(_rotationVector);
            _cockpitUIController.SetWantedRuder(_rotationVector.z);
            _rotationText.text = Mathf.Abs(Rotation) + "\u00B0";
        }
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        _isDragging = false;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragging = true;
    }

    public void SetRotation(float rotation)
    {
        _rotationVector.z = rotation;
        _ruderHandleImage.rotation = Quaternion.Euler(_rotationVector);
    }
}
