using Groupup;
using MPUIKIT;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * One entry for a line in Datadisplay line
 */

public class LineEntry : MonoBehaviour
{
    [SerializeField] private TMP_InputField _lineName;
    [SerializeField] private TMP_Text _distanz;
    [SerializeField] private TMP_Text _lineWidthText;
    [SerializeField] private Slider _lineWidthSlider;
    [SerializeField] private MPImage _color;

    private Symbol _startSymbol;
    private Symbol _endSymbol;
    
    private PolyLineContainer _polyLineData;
    public void Init(PolyLine line)
    {
        _polyLineData = line.Data;
        _lineWidthSlider.SetValueWithoutNotify(_polyLineData.LineThickness);
        _polyLineData.LineThickness = _lineWidthSlider.value;
        _color.color = _polyLineData.Color;

        ObjectsInterface objectInterface = ResourceManager.GetInterface<ObjectsInterface>();
        _startSymbol = objectInterface.GetNauticObjectForId(_polyLineData.PointIds[0]).Symbol;
        _endSymbol = objectInterface.GetNauticObjectForId(_polyLineData.PointIds[1]).Symbol;
    }

    private void Update()
    {
        if (!_polyLineData)
            return;
        
        _lineName.SetTextWithoutNotify(_polyLineData.ObjectName);

        _distanz.text = (Extensions.Distance(_startSymbol.NauticObject.Data.Position,
            _endSymbol.NauticObject.Data.Position) * 0.53996f).ToString("F2") + " sm";
        _color.color = _polyLineData.Color;
        _lineWidthText.text = _polyLineData.LineThickness.ToString("F2");
    }

    public void SaveData()
    {
        _polyLineData.ObjectName = _lineName.text;
        _polyLineData.Color = _color.color;
        _polyLineData.LineThickness = _lineWidthSlider.value;
    }

    public void Delete()
    {
        ResourceManager.GetInterface<UI_RootInterface>().DeletePolyLine(_polyLineData.UID);
        Destroy(gameObject);
    }
}
