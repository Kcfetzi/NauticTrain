using System;
using System.Collections.Generic;
using Groupup;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/**
 * The menu that popups up, if ecdismap is rightclicked
 */
public class RightclickMenu : MonoBehaviour
{
    [SerializeField] private EcdisMapDisplay _ecdisMapDisplay;
    
    [SerializeField] private TMP_Text _titleText; 
    
    [SerializeField] private Button _createButton;
    [SerializeField] private Button _distanceButton;
    [SerializeField] private TMP_Text _distanceButtonText;
    [SerializeField] private Button _deleteButton;

    // bool to tell if user wants to messure distance
    private bool _distanceMessureMode;
    
    private Position _position;
    private Symbol _symbol;
    private RectTransform _ownRectTransform;



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            gameObject.SetActive(false);
    }
    
    public void Init(UnityAction<Position> onClick_create, UnityAction<Position> onClick_distance, UnityAction<Symbol> onClick_delete)
    {
        _createButton.onClick.AddListener(() => onClick_create(_position));
        _distanceButton.onClick.AddListener(() => onClick_distance(_position));
        _deleteButton.onClick.AddListener(() => onClick_delete(_symbol));
    }
    
    // Show the menu on given screenpos and show lat lon of given pos
    public void Show(Vector2 screenPos, Position pos, bool isMessuringDistance)
    {
        _position = pos;
        _titleText.text = _position.Lat + ", " + _position.Lon;
        _distanceButtonText.text = isMessuringDistance ? "Entfernung messen stoppen" : "Entfernung messen starten";
        _symbol = null;
        
        SetPosition(screenPos);
        SetButtons();
        gameObject.SetActive(true);
    }
    
    // Show the menu on given screenpos and shows the name of the object
    public void Show(Vector2 screenPos, Symbol obj, bool isMessuringDistance)
    {
        _symbol = obj;
        _titleText.text = obj.NauticObject.Data.ObjectName;
        _distanceButtonText.text = isMessuringDistance ? "Entfernung messen stoppen" : "Entfernung messen starten";
        _position = obj.NauticObject.Data.Position;
        
        SetPosition(screenPos);
        SetButtons();
        gameObject.SetActive(true);
    }

    // Clickcallback on title to copy into clipboard
    public void CopyToClipboard()
    {
        if (_symbol)
            GUIUtility.systemCopyBuffer = _symbol.NauticObject.Data.Position.Lat.ToString().Replace(',','.') + ", " +
                                          _symbol.NauticObject.Data.Position.Lon.ToString().Replace(',','.');
        else
            GUIUtility.systemCopyBuffer = _position.Lat.ToString().Replace(',','.') + ", " + _position.Lon.ToString().Replace(',','.');
    }


    
    // set the screenposition of the menu. Offsets the pos so that menu is always visible
    private void SetPosition(Vector2 screenPosition)
    {
        if (!_ownRectTransform)
            _ownRectTransform = GetComponent<RectTransform>();
        
        Rect boxSize = _ownRectTransform.rect;
        screenPosition.x = Mathf.Clamp(screenPosition.x,  0, Screen.width - boxSize.width);
        screenPosition.y = Mathf.Clamp(screenPosition.y,  boxSize.height, Screen.height);
        _ownRectTransform.position = screenPosition;
    }
    
    // set the correct options for this click
    private void SetButtons()
    {
        _createButton.interactable = _position != null;
        _distanceButton.interactable = _ecdisMapDisplay.SelectedObject != null;
        _deleteButton.interactable = _symbol != null;
    }

    
}
