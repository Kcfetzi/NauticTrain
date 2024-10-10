using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectedDebug : MonoBehaviour
{
    [Header("Debug")] 
    [SerializeField] private TMP_Text _debug1;
    [SerializeField] private TMP_Text _debug2;
    [SerializeField] private TMP_Text _debug3;
    [SerializeField] private TMP_Text _debug4;
    
    
    private ObjectContainer _selectedData;
        
    // Set data from selecteddata into ui
    public void DisplayData(ObjectContainer selectedData)
    {
        _selectedData = selectedData;
    }
    
    public void Update()
    {
        if (_selectedData == null)
            return;
            
        _debug1.text = _selectedData.Debug1;
        _debug2.text = _selectedData.Debug2;
        _debug3.text = _selectedData.Debug3;
        _debug4.text = _selectedData.Debug4;
    }
}
