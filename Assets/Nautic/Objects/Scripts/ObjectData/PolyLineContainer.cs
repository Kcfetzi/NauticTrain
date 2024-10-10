using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PolyLineData")]
public class PolyLineContainer : Container
{
    // thickness of the line
    public float LineThickness = 5;
    // ids of all objects connected via this line
    public List<string> PointIds = new List<string>();
    // color of the line
    public Color Color;
    
    [Header("Init Data")]
    public PolyLine EcdisPolyLinePrefab;
}
