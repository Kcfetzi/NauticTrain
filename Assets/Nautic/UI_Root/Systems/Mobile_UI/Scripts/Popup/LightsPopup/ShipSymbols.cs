using System.Collections;
using System.Collections.Generic;
using MPUIKIT;
using UnityEngine;

public class ShipSymbols : MonoBehaviour
{
    [SerializeField] private SymbolsButton _top;
    [SerializeField] private SymbolsButton _topleft;
    [SerializeField] private SymbolsButton _topRight;
    [SerializeField] private SymbolsButton _midLeft;
    [SerializeField] private SymbolsButton _midBotLeft;

    
    [SerializeField] private MPImage _border;

    public void SetActive(bool active)
    {
        _border.enabled = active;
    }

    /**
     * Returns a list with all symbols set in the Lightpopup. [0] = top, [1] = topLeft, [2] = topRight, [3] = midLeft, [4] = midBotLeft, [5] = bottomLeft, [6] = bottomRight,
     */
    public List<int> GetSymbols()
    {
        List<int> activeLights = new List<int>();
        
        activeLights.Add((int)_top.Symbol);
        activeLights.Add((int)_topleft.Symbol);
        activeLights.Add((int)_topRight.Symbol);
        activeLights.Add((int)_midLeft.Symbol);
        activeLights.Add((int)_midBotLeft.Symbol);

        return activeLights;
    }
}
