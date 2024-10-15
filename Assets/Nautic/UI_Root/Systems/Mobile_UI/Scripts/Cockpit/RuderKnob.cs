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
    
    private Vector3 _rotationVector = Vector3.zero;

    private Vector2 _rotationObjectScreenPosition;
    private bool isRotating = false;

    private UnityAction<int> _applyRotationCallback;
    
    
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
            _applyRotationCallback?.Invoke((int)Rotation);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Stop rotating when the mouse button is released
            isRotating = false;
        }
    }

    public void SetRotation(float rotation)
    {
        _rotationVector.z = Mathf.Round((105f / 50f) * rotation);
        _rotationObject.rotation = Quaternion.Euler(_rotationVector);
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
            
        // adjust cause worldcoordinates and screencoordinates are nocht in the same reference system
        _rotationVector.z = angle - 90;

        if (_rotationVector.z < 90 && _rotationVector.z > -180)
            _rotationVector.z = Mathf.Clamp(_rotationVector.z, -_maximalRotatableAngle, _maximalRotatableAngle);
        else 
            _rotationVector.z = Mathf.Clamp(_rotationVector.z + 360, -_maximalRotatableAngle, _maximalRotatableAngle);
        // Apply rotation
        _rotationObject.rotation = Quaternion.Euler(0, 0, _rotationVector.z);
        _rotationText.text = Mathf.Abs(Rotation) + "\u00B0";
    }
}
