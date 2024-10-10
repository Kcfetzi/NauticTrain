using MPUIKIT;
using UnityEngine;
using UnityEngine.UI;

/**
 * This is a dropdownmenu used in the datadisplay.
 */

public class DropDownMenu : MonoBehaviour
{
    [SerializeField] protected Button _ownButton;
    [SerializeField] private GameObject _content;
    [SerializeField] private MPImage _icon;
    
    private void Awake()
    {
    }

    // Activate / Deactivate a dropdownmenu
    public void TogglePanel()
    {
        _content.SetActive(!_content.activeSelf);
        _icon.ShapeRotation = _content.activeSelf ? 180f : 270f;
    }

    public void ClosePanel()
    {
        _content.SetActive(false);
        _icon.ShapeRotation = 270f;
    }

    public void OpenPanel()
    {
        _content.SetActive(true);
        _icon.ShapeRotation = 180f;
    }

    // disable the dropdownmenu button and view.
    public void Disable()
    {
        _ownButton.interactable = false;
        _icon.ShapeRotation = 270f;
        _content.SetActive(false);
    }
}
