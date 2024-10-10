using MPUIKIT;
using UnityEngine;
using Utility.Pooling;

public class RadarSymbol : PooledMonobehaviour
{
    [SerializeField] private MPImage _pointer;
    [SerializeField] private MPImage _marker;    
    
    private NauticObject _obj;
    private UI_RootInterface _uiInterface;
    private RectTransform _rectTransform;

    private Radar _radar;
    
    private float _updateTimer = 3;
    
    public void Init(NauticObject obj, UI_RootInterface uiInterface, Radar radar)
    {
        _obj = obj;
        _uiInterface = uiInterface;
        _rectTransform = GetComponent<RectTransform>();
        _radar = radar;
    }

    private void Update()
    {
        
        if (_updateTimer > 3)
        {
            _pointer.rectTransform.sizeDelta = new Vector2(4, Mathf.RoundToInt(10 * (float)_obj.Data.ActualVelocity));
            _updateTimer = 0;
        }
        else
        {
            _updateTimer += Time.deltaTime;
        }
        
        Vector3 pos = _obj.Data.Position.UnityPositionFloat;
        Vector2 radarPos = _uiInterface.WorldToRadarPosition(pos);
        
        _rectTransform.anchoredPosition = radarPos;
        _rectTransform.eulerAngles = new Vector3(0, 0, -_obj.Data.ActualCourse);

    }

    public void OnClick()
    {
        _radar.SetTarget(_obj);
        _marker.enabled = true;
    }

    public void Unmark()
    {
        _marker.enabled = false;
    }
}
