using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class Container : ScriptableObject
{
    // first letter is an identifier for this object. "o" = nauticobject, "l" = polyline, "a" = area
    public string UID;
    public string ObjectName = "";
}
