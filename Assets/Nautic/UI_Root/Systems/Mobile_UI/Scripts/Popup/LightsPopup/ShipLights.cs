using System.Collections;
using System.Collections.Generic;
using MPUIKIT;
using UnityEngine;

public class ShipLights : MonoBehaviour
{
    [SerializeField] private LightsButton _top;
    [SerializeField] private LightsButton _topleft;
    [SerializeField] private LightsButton _topRight;
    [SerializeField] private LightsButton _mid;
    [SerializeField] private LightsButton _bottom;
    [SerializeField] private LightsButton _bottomLeft;
    [SerializeField] private LightsButton _bottomRight;

    [SerializeField] private MPImage _border;

    public void SetActive(bool active)
    {
        _border.enabled = active;
    }

    /**
     * Returns a list with all lights set in the Lightpopup. [0] = top, [1] = topLeft, [2] = topRight, [3] = mid, [4] = bottom, [5] = bottomLeft, [6] = bottomRight,
     */
    public List<int> GetLights()
    {
        List<int> activeLights = new List<int>();

        activeLights.Add((int)_top.Light);
        activeLights.Add((int)_topleft.Light);
        activeLights.Add((int)_topRight.Light);
        activeLights.Add((int)_mid.Light);
        activeLights.Add((int)_bottom.Light);
        activeLights.Add((int)_bottomLeft.Light);
        activeLights.Add((int)_bottomRight.Light);

        return activeLights;
    }
}