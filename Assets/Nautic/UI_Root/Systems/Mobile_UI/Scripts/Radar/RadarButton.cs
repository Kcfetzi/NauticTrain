using System.Collections;
using System.Collections.Generic;
using MPUIKIT;
using TMPro;
using UnityEngine;

public class RadarButton : MonoBehaviour
{
    [SerializeField] private Color _green;
    
    [SerializeField] private MPImage _backGround;
    [SerializeField] private TMP_Text _text;

    public void SetActive(bool active)
    {
        _backGround.color = _green;
        _backGround.StrokeWidth = active ? 0 : 2;
        _text.color = active ? Color.black : _green;
    }
}
