using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class RuderKnob : MonoBehaviour
{
    [SerializeField] private TMP_Text _rotationText;
    
    [SerializeField] private RectTransform _rotationObject;
    [SerializeField] private RectTransform _touchZone;

    // the angle shown on the display
    [SerializeField] private float _maxDisplayedAngle;
    // the angle that points to the maxDisplayedAngle
    [SerializeField] private float _maximalRotatableAngle;

    private Vector2 _rotationObjectScreenPosition;
    private bool isRotating = false;

    private UnityAction<int> _applyRotationCallback;

    private int _snapingValue;
    
    
    // use this to get the real value, because the image rotation values are not real
    public float Rotation
    {
        get
        {
            float angle = 0;
            if (_rotationObject.eulerAngles.z < 180)
            {
                angle = Mathf.Round((_rotationObject.eulerAngles.z) % 360);
            }
            else
            {
                angle = Mathf.Round((_rotationObject.eulerAngles.z - 360) % 360);
            }
            return Mathf.Round(angle * (_maxDisplayedAngle / _maximalRotatableAngle));
        }
    }
    
    // set callback after rotation is registered and applied
    public void Init(UnityAction<int> applyRotationCallback)
    {
        _applyRotationCallback = applyRotationCallback;
    }
    
    void Update()
    {
        // Check for mouse input (for testing in Editor)
        if (Input.GetMouseButtonDown(0))
        {
            if (IsTouchInsideRect(Input.mousePosition))
            {
                isRotating = true;
                CalcAndApplyRotation();
            }
        }
        else if (Input.GetMouseButton(0) && isRotating)
        {
            CalcAndApplyRotation();
            _applyRotationCallback?.Invoke((int)-Rotation);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Stop rotating when the mouse button is released
            isRotating = false;
        }
    }

    public void SetRotation(float rotation)
    {
        _rotationObject.localRotation = Quaternion.Euler(new Vector3(0, 0, rotation));
        _rotationText.text = Mathf.Abs(Rotation) + "\u00B0";
    }
    
    // Method to check if the touch or click is inside the RectTransform
    private bool IsTouchInsideRect(Vector2 position)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _touchZone,
            position,
            null,
            out Vector2 localPoint
        );
        return _touchZone.rect.Contains(localPoint);
    }

    private void CalcAndApplyRotation()
    {
        // Calculate the direction of the movement
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _touchZone,
            Input.mousePosition,
            null,
            out Vector2 localPoint
        );
            
        // Rotate the object around the z-axis based on the movement direction
        float angle = Mathf.Atan2(localPoint.y, localPoint.x) * Mathf.Rad2Deg;
            
        // adjust cause worldcoordinates and screencoordinates are not in the same reference system
        angle -=  90;

        if (_snapingValue != 0)
        {
            int angleInGrafic = Mathf.RoundToInt(angle * (_maxDisplayedAngle / _maximalRotatableAngle));
            
            // snap to value
            if (_snapingValue == 5)
            {
                if (angleInGrafic < 4 && angleInGrafic > -4)
                    angleInGrafic = 0;
                else if (angle > 0)
                    angleInGrafic = Mathf.RoundToInt((angleInGrafic + 4) / _snapingValue) * _snapingValue;
                else
                    angleInGrafic = Mathf.RoundToInt((angleInGrafic - 4) / _snapingValue) * _snapingValue;
            }

            // snap to value    
            if (_snapingValue == 10)
            {
                if (angleInGrafic < 9 && angleInGrafic > -9)
                    angleInGrafic = 0;
                else if (angleInGrafic >= 0)
                    angleInGrafic = Mathf.RoundToInt((angleInGrafic + 9) / _snapingValue) * _snapingValue;
                else
                    angleInGrafic = Mathf.RoundToInt((angleInGrafic - 9) / _snapingValue) * _snapingValue;
            }

            angle = angleInGrafic * (_maximalRotatableAngle / _maxDisplayedAngle);
        }

        // clamp the rotation to the scale maximum
        if (angle < 90 && angle > -180)
            angle = Mathf.Clamp(angle, -_maximalRotatableAngle, _maximalRotatableAngle);
        else 
            angle = Mathf.Clamp(angle + 360, -_maximalRotatableAngle, _maximalRotatableAngle);
        // Apply rotation
        SetRotation(Mathf.Round(angle));
    }

    // set the knob to snap to eigther 5 or 10 deg. Used from the two toggels on the ruderknob
    public void SetSnaping(int value)
    {
        _snapingValue = _snapingValue == value ? 0 : value;
    }
}
