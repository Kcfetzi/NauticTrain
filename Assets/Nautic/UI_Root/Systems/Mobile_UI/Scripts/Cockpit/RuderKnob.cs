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

    private float _mouseStartX;
    private bool _isDragging;

    // use this to get the real value, because the image rotation values are not real
    public float Rotation => Mathf.Round((50f / 105f) * _rotationVector.z);
    
    void Update()
    {
        if (_isDragging)
        {
            float diffX = Input.GetAxis("Mouse X") - _mouseStartX;
            _rotationVector.z -= diffX * _rotationSpeed;
            _rotationVector.z = Mathf.Round(_rotationVector.z);
            _rotationVector.z = Mathf.Clamp(_rotationVector.z, -105, 105);

            _ruderHandleImage.rotation = Quaternion.Euler(_rotationVector);
            _rotationText.text = Mathf.Abs(Rotation) + "\u00B0";
            
            _cockpitUIController.SetWantedRuder((int)Rotation);
        }
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        _isDragging = false;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragging = true;
        _mouseStartX = Input.GetAxis("Mouse X");
    }

    public void SetRotation(float rotation)
    {
        _rotationVector.z = Mathf.Round((105f / 50f) * rotation);
        _ruderHandleImage.rotation = Quaternion.Euler(_rotationVector);
        _rotationText.text = Mathf.Abs(Rotation) + "\u00B0";
    }
}
