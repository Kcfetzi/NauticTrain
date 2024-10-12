using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Ton
{
    public TonType Type;
    public string TonName;
    public double Lon;
    public double Lat;
}


public enum TonType
{
    lighted, unlighted
}