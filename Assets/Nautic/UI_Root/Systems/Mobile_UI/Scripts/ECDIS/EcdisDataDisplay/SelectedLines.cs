using System.Collections.Generic;
using UnityEngine;

/**
 * Lines dropdownmenu.
 */
public class SelectedLines : DropDownMenu
{
    private List<LineEntry> _activeLineEntries = new List<LineEntry>();

    [SerializeField] private LineEntry _lineEntry;
    [SerializeField] private Transform _container;
    public void DisplayData(List<PolyLine> linkedLines)
    {
        _ownButton.interactable = true;
        
        foreach (LineEntry entry in _activeLineEntries)
        {
            if (entry != null)
                Destroy(entry.gameObject);
        }
        
        _activeLineEntries.Clear();
        
        foreach (PolyLine ecdisMapPolyLine in linkedLines)
        {
            LineEntry entry = Instantiate(_lineEntry, _container);
            entry.Init(ecdisMapPolyLine);
            _activeLineEntries.Add(entry);
        }
            

    }
}
