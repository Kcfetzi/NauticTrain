using System.Collections.Generic;
using Groupup;
using MPUIKIT;
using TMPro;
using UnityEngine;

/**
 * This is a symbol, visible on the ectis view.
 */
public class Symbol : MonoBehaviour
{
    private UI_RootInterface _uiInfo;
    
    private NauticObject _nauticObject;
    private RectTransform _rectTransform;
    
    [SerializeField] private Transform _symbolTransform;
    private MPImage _symbol;
    [SerializeField] private TMP_Text _objectName;
    [SerializeField] private MPImage _marker;
    
    public RectTransform RectTransform => _rectTransform;
    public NauticObject NauticObject => _nauticObject;

    public void SetSelected(bool active)
    {
        _marker.enabled = active;
    }
    
    public void ToggleObjectName(bool active)
    {
        _objectName.gameObject.SetActive(active);
    }
    

    public void Init(NauticObject obj, bool visible)
    {
        _rectTransform = GetComponent<RectTransform>();
        _uiInfo = ResourceManager.GetInterface<UI_RootInterface>();
        
        _nauticObject = obj;
        obj.Symbol = this;
        _rectTransform.anchoredPosition = _uiInfo.WorldToEcdisPosition(_nauticObject.Data.Position.UnityPositionFloat);
        _symbol = _symbolTransform.GetComponent<MPImage>();
        ToggleObjectName(visible);
    }

    private void LateUpdate()
    {
        Vector3 parentScale = _uiInfo.EcdisMapScale;
        if (!_nauticObject)
        {
            _rectTransform.localScale = new Vector3(1 / parentScale.x, 1 /parentScale.y, 1);
            return;
        }
        
        Vector3 symobolScale = new Vector3(1 * _nauticObject.Data.EcdisSize / parentScale.x, 1 * _nauticObject.Data.EcdisSize /parentScale.y, 1);
        _rectTransform.localScale = symobolScale;
        _objectName.text = _nauticObject.Data.ObjectName;
        _rectTransform.anchoredPosition = _uiInfo.WorldToEcdisPosition(_nauticObject.Data.Position.UnityPositionFloat);
        Vector3 rotation = new Vector3(0, 0, -_nauticObject.Data.ActualCourse);
        _symbolTransform.eulerAngles = rotation;
        _symbol.color = _nauticObject.Data.EcdisColor;
        
        
    }
}
