using Groupup;
using Unity.Mathematics;
using UnityEngine;

/**
 * Position of object inside of nautictrain.
 * If no boundTransform is set, this object expect to be static and dont move.
 */

[System.Serializable]
public class Position
{
    public double3 UnityPosition;
    public double2 LatLon;

    private Transform _boundTransform;
    private ScenarioInterface _rootScenarioInterface;

    public Position(double2 latLon)
    {
        LatLon = latLon;
        
        _rootScenarioInterface = ResourceManager.GetInterface<ScenarioInterface>();
        if (_rootScenarioInterface.IsActive)
            UnityPosition = ResourceManager.GetInterface<ScenarioInterface>().WorldToUnityPoint(LatLon);
        else
        {
            _rootScenarioInterface.OnSceneLoaded += () => UnityPosition = ResourceManager.GetInterface<ScenarioInterface>().WorldToUnityPoint(LatLon);
        }
    }

    // refactor
    public Position(double lat, double lon, Vector3 unityPosition)
    {
        LatLon = new double2(lat, lon);
        UnityPosition = new double3(unityPosition.x, unityPosition.y, unityPosition.z);
    }
    
    public Position(double lat, double lon)
    {
        LatLon = new double2
        {
            x = lat,
            y = lon
        };
        
        _rootScenarioInterface = ResourceManager.GetInterface<ScenarioInterface>();
        if (_rootScenarioInterface.IsActive)
            UnityPosition = ResourceManager.GetInterface<ScenarioInterface>().WorldToUnityPoint(LatLon);
        else
        {
            _rootScenarioInterface.OnSceneLoaded += () => UnityPosition = ResourceManager.GetInterface<ScenarioInterface>().WorldToUnityPoint(LatLon);
        }
    }

    public Position(double3 unityPosition)
    {
        UnityPosition = unityPosition;

        _rootScenarioInterface = ResourceManager.GetInterface<ScenarioInterface>();
        if (_rootScenarioInterface.IsActive)
            LatLon = ResourceManager.GetInterface<ScenarioInterface>().UnityToWorldPoint(UnityPosition);
        else
        {
            _rootScenarioInterface.OnSceneLoaded += () => LatLon = ResourceManager.GetInterface<ScenarioInterface>().UnityToWorldPoint(UnityPosition);
        }
    }
    
    public Position(Transform parent)
    {
        //m_MapChannelSo = ServiceManager.Instance.GetChannel<MapChannelSO>();
        
        _boundTransform = parent;
        _rootScenarioInterface = ResourceManager.GetInterface<ScenarioInterface>();
    }

    public Position(Vector3 unityPosition)
    {
        UnityPosition = new double3(unityPosition.x, unityPosition.y, unityPosition.z);
        
        _rootScenarioInterface = ResourceManager.GetInterface<ScenarioInterface>();
        if (_rootScenarioInterface.IsActive)
            LatLon = ResourceManager.GetInterface<ScenarioInterface>().UnityToWorldPoint(UnityPosition);
        else
        {
            _rootScenarioInterface.OnSceneLoaded += () => LatLon = ResourceManager.GetInterface<ScenarioInterface>().UnityToWorldPoint(UnityPosition);
        }
    }

    // Constructor used by refpoint.cs
    public Position(double3 unityPosition, double2 latLon)
    {
        UnityPosition = unityPosition;
        LatLon = latLon;
        _rootScenarioInterface = ResourceManager.GetInterface<ScenarioInterface>();
    }
    
    /*
     * The position inside of unity. Stored as double for more accurate calculations.
     */
    public double3 UnityPositionDouble
    {
        get {
            if (_boundTransform)
            {
                Vector3 transformPos = _boundTransform.position;
                return new double3
                {
                    x = transformPos.x,
                    y = transformPos.y,
                    z = transformPos.z
                };
            }

            return UnityPosition;
        }
        set
        {
            UnityPosition = value;
            //m_MapChannelSo = ServiceManager.Instance.GetChannel<MapChannelSO>();
            //if (m_MapChannelSo.m_IsActive)
                //m_LatLon = m_MapChannelSo.UnityToWorldPoint(value);
            //else
            //{
            //    m_MapChannelSo.OnSceneLoaded += () => m_LatLon = m_MapChannelSo.UnityToWorldPoint(value);
            //}
            
        } 
    }

    /*
     * The position inside of unity as native float
     */
    public Vector3 UnityPositionFloat
    {
        get
        {
            if (_boundTransform)
            {
                Vector3 transformPos = _boundTransform.position;
                return new Vector3
                {
                    x = transformPos.x,
                    y = transformPos.y,
                    z = transformPos.z
                };
            }

            return new Vector3{x = (float)UnityPosition.x, y = (float) UnityPosition.y, z = (float) UnityPosition.z};
        }

        set => UnityPositionDouble = new double3 {
                x = value.x,
                y = value.y,
                z = value.z
            };
    }

    /*
     * Position in world coordinates. X is lon Y is lat.
     */
    public double2 WorldPosition
    {
        get
        {
            if (_boundTransform)
            {
                return ResourceManager.GetInterface<ScenarioInterface>().UnityToWorldPoint(new double3(_boundTransform.position));
            }

            return LatLon;
        }
        set
        {
            LatLon = value;
            if (_rootScenarioInterface.IsActive)
                UnityPosition = ResourceManager.GetInterface<ScenarioInterface>().WorldToUnityPoint(value);
            else
            {
                _rootScenarioInterface.OnSceneLoaded += () => UnityPosition = ResourceManager.GetInterface<ScenarioInterface>().WorldToUnityPoint(value);
            }
        }
    }

    public double Lon
    {
        get
        {
            return WorldPosition.y;
        }
        set
        {
            LatLon.x = value;
            if (_rootScenarioInterface.IsActive)
                UnityPosition.x = ResourceManager.GetInterface<ScenarioInterface>().Lon2unityX(value);
            else
            {
                _rootScenarioInterface.OnSceneLoaded +=
                    () => UnityPosition = UnityPosition.x = ResourceManager.GetInterface<ScenarioInterface>().Lon2unityX(value);
            }
        }
    }

    public double Lat
    {
        get
        {
            return WorldPosition.x;
        }
        set
        {
            LatLon.y = value;
            if (_rootScenarioInterface.IsActive)
                UnityPosition.z = ResourceManager.GetInterface<ScenarioInterface>().Lat2unityZ(value);
            else
            {
                _rootScenarioInterface.OnSceneLoaded +=
                    () => UnityPosition = UnityPosition.z = ResourceManager.GetInterface<ScenarioInterface>().Lat2unityZ(value);
            }
        }
    }

    public double x
    {
        get { return UnityPositionDouble.x; }
    }

    public double y => UnityPositionDouble.y;
    public double z => UnityPositionDouble.z;
}