using System.Collections;
using System.Collections.Generic;
using MPUIKIT;
using UnityEngine;

public class SymbolsButton : MonoBehaviour
{
    [Header("Symbols")] 
    [SerializeField] private MPImage _triangleUp;
    [SerializeField] private MPImage _triangleDown;
    [SerializeField] private MPImage _circle;
    [SerializeField] private MPImage _hash;
    [SerializeField] private MPImage _rectAngle;
    [SerializeField] private MPImage _none;
    
    private SymbolSignal _ownSymbol = SymbolSignal.None;

    public SymbolSignal Symbol => _ownSymbol;

    public void SetSymbol(SymbolSignal symbol)
    {
        _triangleUp.enabled = symbol == SymbolSignal.TriangleUp;
        _triangleDown.enabled = symbol == SymbolSignal.TriangleDown;
        _circle.enabled = symbol == SymbolSignal.Circle;
        _hash.enabled = symbol == SymbolSignal.Hash;
        _rectAngle.enabled = symbol == SymbolSignal.Rectangle;
        _none.enabled = symbol == SymbolSignal.None;
    }
    
    // light button was pressed by user
    public void PressSignal()
    {
        _ownSymbol++;
        if ((int)_ownSymbol == 6)
            _ownSymbol = 0;

        SetSymbol(_ownSymbol);
    }

    /*
     * light and symbol encoding.
     */
    public enum SymbolSignal
    {
        None = 0,
        TriangleUp = 1,
        TriangleDown = 2,
        Circle = 3,
        Hash = 4,
        Rectangle = 5
    }
}
