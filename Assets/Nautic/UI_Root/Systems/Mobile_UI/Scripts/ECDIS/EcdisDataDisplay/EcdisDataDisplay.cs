using System;
using System.Collections.Generic;
using Groupup;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/**
 * Display that shows objectdata to the user.
 * Most data is displayed in Dropdownmenus.
 * Also it shows date and time
 */

public class EcdisDataDisplay : MonoBehaviour, IPointerClickHandler
{
    private ObjectContainer _selectedContainer;
    private List<PolyLine> _selectedDataLines = new List<PolyLine>();

    [SerializeField] private TMP_Text _dateText;
    [SerializeField] private TMP_Text _timeText;

    [SerializeField] private SelectedObject _objectData;
    [SerializeField] private SelectedLines _linesData;
    [SerializeField] private SelectedDebug _debugData;
    [SerializeField] private Toggle _debugToggle;


    private void Awake()
    {
        _debugToggle.onValueChanged.AddListener(ToggleDebug);
    }

    /**
     * Init display with data.
     * If object switched default panel gets opened
     */
    public void SetSelectedObject(NauticObject obj)
    {
        ObjectContainer container = obj ? obj.Data : null;
        
        if (!_selectedContainer && container)
        {
            _objectData.OpenPanel();
            _linesData.ClosePanel();
        }
        
        if (_selectedContainer)
            _selectedContainer.OnEcdisChanged -= UpdateView;
        
        _selectedContainer = container;
        
        if (_selectedContainer)
        {
            UpdateView();
            _selectedContainer.OnEcdisChanged += UpdateView;
        }
    }
    

    // A nauticObject will be deleted, if its selected object stop displaying
    public void DeleteNauticObject(NauticObject obj)
    {
        if (_selectedContainer == obj.Data)
        {
            _selectedContainer = null;
            _selectedDataLines.Clear();
        }
    }
    
    // Set date and time in datadisplay header
    private void SetDateAndTime()
    {
        _dateText.text = DateTime.Now.ToShortDateString();
        _timeText.text = DateTime.Now.ToShortTimeString();
    }

    /**
     * Callback if some data get changed by the user
     */
    public void UpdateView()
    {
        UI_RootInterface uiInterface = ResourceManager.GetInterface<UI_RootInterface>();
        
        _selectedDataLines.Clear();
        if (_selectedContainer == null) return;
        foreach (PolyLine ecdisMapPolyLine in uiInterface.ActiveEcdisLines)
        {
            if (_selectedContainer.PolylineIds.Contains(ecdisMapPolyLine.Data.UID))
            {
                _selectedDataLines.Add(ecdisMapPolyLine);
            }
        }
        
        _objectData.DisplayData(_selectedContainer);
        _linesData.DisplayData(_selectedDataLines);
        _debugData.DisplayData(_selectedContainer);
    }
    
    // Close all dropdownpanels
    public void CloseAllDropDown()
    {
        _objectData.ClosePanel();
        _linesData.ClosePanel();
    }

    private void Update()
    {
        SetDateAndTime();
    }
    
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            HandleLeftClickInput(eventData);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            HandleRightClickInput(eventData);
        }
    }
    
    
    /**
     * Raycast into the ui and determine actions based on the names from hitten objects.
     */
    private void HandleRightClickInput(PointerEventData eventData)
    {
    }
    
    /**
     * Raycast into the ui and determine actions based on the names from hitten objects.
     */
    private void HandleLeftClickInput(PointerEventData eventData)
    {
    }

    // toggle for input was changed
    public void ToggleDebug(bool active)
    {
        _debugData.gameObject.SetActive(active);
    }

}
