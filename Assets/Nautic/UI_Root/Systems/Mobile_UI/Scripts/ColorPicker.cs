using MPUIKIT;
using UnityEngine;
using UnityEngine.UI;

/**
 * Colorpicker for ui
 */

public class ColorPicker : MonoBehaviour
{
    [SerializeField] private Slider _colorSlider;
    [SerializeField] private MPImage _colorRange;
    [SerializeField] private MPImage _actualColor;

    private void Awake()
    {
        _colorSlider.minValue = 0;
        _colorSlider.maxValue = _colorRange.sprite.texture.width;
    }

    private void Update()
    {
        if (!_colorRange.enabled)
            return;

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) )
        {
            SampleColor();
        }
    }

    // Get the color of the pixel at slider pos.
    private void SampleColor()
    {
        float pos = _colorSlider.value;
        Color color = _colorRange.sprite.texture.GetPixel((int) pos, _colorRange.sprite.texture.height / 2);
        _actualColor.color = color;
    }
    
    // Show and hide colorslider
    public void ToggleColorRange()
    {
        _colorSlider.gameObject.SetActive(!_colorSlider.gameObject.activeSelf);
    }
}
