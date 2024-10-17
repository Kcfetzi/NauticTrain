
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Groupup;
using Sirenix.OdinInspector;
using Unity.Mathematics;

/** 
   * Structurerfile
   * This is a generated file from Structurer.
   * If you edit this file, stick to the format from Funcs, UnityActions and methods, to keep on getting all benefits from Strukturer.
   */ 
public class ScenarioInterface : InterfaceSOBase
{
    // count own time, because time.timesincelevelload is shared between all scenes. Because root scene is always active it will stack up time after restart.
    public float ScenarioTime;
    
    #region Map
    
    private Position LeftBottomPoint;
    private Position RightTopPoint;

    public Vector3 TileSize;
    public Vector3 MapSize;
    public Transform MagneticNorth;

    public Transform PointsSpawnParent;
    public Transform ObjectSpawnParent;
    
    [BoxGroup("Unity units in meters (should be near 1)", centerLabel: true)]
    public double UnityXInMeters;
    [BoxGroup("Unity units in meters (should be near 1)")]
    public double UnityZInMeters;
    
    [BoxGroup("Ratios", centerLabel: true)]
    public float UnityToRealworldRatio;
    [BoxGroup("Ratios")]
    public float RealworldToUnityRatio;
    
    [BoxGroup("Lat and Lon values", centerLabel: true)]
    [Title("111000m Konstant")]
    public double LatInMeters;
    [BoxGroup("Lat and Lon values")]
    [Title("55802m - 111321m")]
    public double LonInMeters;
    
    [PropertySpace(SpaceBefore = 30)]
    [Button(ButtonSizes.Large, Stretch = false), GUIColor(0, 1, 0)]
    public void CalculateValues(Terrain lastTile)
    {
        TileSize = lastTile.terrainData.size;
        MapSize = lastTile.transform.position + new Vector3(TileSize.x, 0, TileSize.z);
        
        CalculateMapRatio();
        SetKmPerUnit();
    }

    public void SetSpawnParents(Transform pointsSpawn, Transform objectSpawn)
    {
        PointsSpawnParent = pointsSpawn;
        ObjectSpawnParent = objectSpawn;
    }
    
    // Calculate the ratios between unity and real world. This is mostly used to check for correct refpoints
    private void CalculateMapRatio()
    {
        List<RefPoint> refPoints = FindObjectsOfType<RefPoint>().ToList();

        if (refPoints.Count < 2)
        {
            Debug.Log("Need at least two refpoints in scene!");
            return;
        }

        if (refPoints.Count > 2)
        {
            Debug.Log("There are more than two refpoints in scene!");
            return;
        }
        
        if (refPoints[0].transform.position.x < refPoints[1].transform.position.x)
        {
            LeftBottomPoint = refPoints[0].Position;
            RightTopPoint = refPoints[1].Position;
        }
        else
        {
            LeftBottomPoint = refPoints[1].Position;
            RightTopPoint = refPoints[0].Position;
        }

        double realWorldDistance = GeoUtils.CalculateRealWorldDistance(refPoints[0].Position, refPoints[1].Position);
        float unitDistance = Vector3.Distance(new Vector3(refPoints[0].transform.position.x, 0, refPoints[0].transform.position.z), new Vector3(refPoints[1].transform.position.x, 0, refPoints[1].transform.position.z));

        UnityToRealworldRatio = (float)(realWorldDistance / unitDistance);
        RealworldToUnityRatio = (float)(unitDistance / realWorldDistance);
    }
    
    // Calculate the ratio from km into latlon and unity units
    private void SetKmPerUnit()
    {   
        Position P1 = new Position(LeftBottomPoint.LatLon[0], RightTopPoint.LatLon[1]);
        P1.UnityPositionDouble = new double3 {x = LeftBottomPoint.UnityPositionDouble.x, y = 0, z = RightTopPoint.UnityPositionDouble.z};
       
        
        LatInMeters = Math.Abs(GeoUtils.CalculateRealWorldDistance(P1, RightTopPoint)/(LeftBottomPoint.LatLon[0] - RightTopPoint.LatLon[0]));
        LonInMeters = Math.Abs(GeoUtils.CalculateRealWorldDistance(P1, LeftBottomPoint)/(LeftBottomPoint.LatLon[1] - RightTopPoint.LatLon[1]));
        UnityXInMeters = Math.Abs(GeoUtils.CalculateRealWorldDistance(P1, RightTopPoint)/(LeftBottomPoint.UnityPositionDouble.z - RightTopPoint.UnityPositionDouble.z));
        UnityZInMeters = Math.Abs(GeoUtils.CalculateRealWorldDistance(P1, LeftBottomPoint)/(LeftBottomPoint.UnityPositionDouble.x - RightTopPoint.UnityPositionDouble.x));
    }
    
    
    // Transform a point given in latlon into x,y,z
    public double3 WorldToUnityPoint(double2 latLon)
    {
        double x = Lon2unityX(latLon.y);
        double z = Lat2unityZ(latLon.x);
        
        
        double3 pos = new double3
        {
            x = x,
            y = 0,
            z = z
        };


        double distanceRealworld =
            GeoUtils.CalculateRealWorldDistance(latLon.x, latLon.y, LeftBottomPoint.Lat, LeftBottomPoint.Lon);
        float distanceUnity = Vector3.Distance(new Vector3((float)pos.x, 0, (float)pos.z), new Vector3(LeftBottomPoint.UnityPositionFloat.x, 0, LeftBottomPoint.UnityPositionFloat.z));

        

        return pos;
    }
    
    // Transform a point given in x,y,z into latlon
    public double2 UnityToWorldPoint(double3 unityPosition)
    {
        return new double2
        {
            x = LeftBottomPoint.LatLon[0] + (unityPosition.z - LeftBottomPoint.UnityPositionDouble.z) / (RightTopPoint.UnityPositionDouble.z - LeftBottomPoint.UnityPositionDouble.z) * (RightTopPoint.LatLon[0] - LeftBottomPoint.LatLon[0]),
            y = LeftBottomPoint.LatLon[1] + (unityPosition.x - LeftBottomPoint.UnityPositionDouble.x) / (RightTopPoint.UnityPositionDouble.x - LeftBottomPoint.UnityPositionDouble.x) * (RightTopPoint.LatLon[1] - LeftBottomPoint.LatLon[1])
        };
    }
    
    // Transform a Lon to unityx
    public double Lon2unityX(double Lon)
    {
        double normalizedLon = (Lon - LeftBottomPoint.LatLon[1]) / (RightTopPoint.LatLon[1] - LeftBottomPoint.LatLon[1]);
        double unityX = LeftBottomPoint.UnityPositionDouble.x + normalizedLon * (RightTopPoint.UnityPositionDouble.x - LeftBottomPoint.UnityPositionDouble.x);
    
        return unityX;
    }
    // Transform a Lat to unityz
    public double Lat2unityZ(double Lat)
    {
        double normalizedLat = (Lat - LeftBottomPoint.LatLon[0]) / (RightTopPoint.LatLon[0] - LeftBottomPoint.LatLon[0]);
        double unityZ = LeftBottomPoint.UnityPositionDouble.z + normalizedLat * (RightTopPoint.UnityPositionDouble.z - LeftBottomPoint.UnityPositionDouble.z);
    
        return unityZ;
    }    
    // Transforms a Lon to unity x
    public double unityX2Lon(double unityX)
    {
        double normalizedUnityX = (unityX - LeftBottomPoint.UnityPositionDouble.x) / (RightTopPoint.UnityPositionDouble.x - LeftBottomPoint.UnityPositionDouble.x);
        double lon = LeftBottomPoint.LatLon[1] + normalizedUnityX * (RightTopPoint.LatLon[1] - LeftBottomPoint.LatLon[1]);
    
        return lon;
    }
    // Transforms a Lat to unity z
    public double unityZ2Lat(double unityZ)
    {
        double normalizedUnityZ = (unityZ - LeftBottomPoint.UnityPositionDouble.z) / (RightTopPoint.UnityPositionDouble.z - LeftBottomPoint.UnityPositionDouble.z);
        double lat = LeftBottomPoint.LatLon[0] + normalizedUnityZ * (RightTopPoint.LatLon[0] - LeftBottomPoint.LatLon[0]);
    
        return lat;
    }  
    

    #endregion

    #region Weather

    public int ViewRange;
    
    [Title("Active Weather")]
    [EnumToggleButtons]
    public Weather StartWeather = Weather.Klar;
    
    [Title("Time")]
    [EnumToggleButtons]
    public SunSet StartTime = SunSet.Lunch;
    
    public enum Weather
    {
        Klar = 0, Regen = 1, Schnee = 2, Nebel = 3, Rotation = 4
    }
    
    public enum SunSet
    {
        Morning = 3, Lunch = 0, Evening = 4, Night = 1, Rotaton = 2
    }
    
    [PropertySpace(SpaceBefore = 30)]
    [Button(ButtonSizes.Large, Stretch = false), GUIColor(0, 1, 0)]
    public void SetWeather()
    {
        switch (StartWeather)
        {
            case Weather.Klar:
                foreach (EnviroWeatherPreset weatherWeatherPreset in EnviroSkyMgr.instance.Weather.currentActiveZone.zoneWeatherPresets)
                {
                    if (weatherWeatherPreset.Name == "Clear Sky")
                    {
                        EnviroSkyMgr.instance.Weather.startWeatherPreset = weatherWeatherPreset;
                        break;
                    }
                }
                break;
            case Weather.Nebel:
                foreach (EnviroWeatherPreset weatherWeatherPreset in EnviroSkyMgr.instance.Weather.currentActiveZone.zoneWeatherPresets)
                {
                    if (weatherWeatherPreset.Name == "Foggy 1") {
                        EnviroSkyMgr.instance.Weather.startWeatherPreset = weatherWeatherPreset;
                        break;
                    }
                }
                break;
            case Weather.Regen:
                foreach (EnviroWeatherPreset weatherWeatherPreset in EnviroSkyMgr.instance.Weather.currentActiveZone.zoneWeatherPresets)
                {
                    if (weatherWeatherPreset.Name == "Heavy Rain"){
                        EnviroSkyMgr.instance.Weather.startWeatherPreset = weatherWeatherPreset;
                        break;
                    }
                }
                break;
            case Weather.Schnee:
                foreach (EnviroWeatherPreset weatherWeatherPreset in EnviroSkyMgr.instance.Weather.currentActiveZone.zoneWeatherPresets)
                {
                    if (weatherWeatherPreset.Name == "Heavy Snow") {
                        EnviroSkyMgr.instance.Weather.startWeatherPreset = weatherWeatherPreset;
                        break;
                    }
                }
                break;
        }
        
        EnviroSkyMgr.instance.SetAutoWeatherUpdates(StartWeather == Weather.Rotation);

        switch (StartTime)
        {
            case SunSet.Lunch:
                EnviroSkyMgr.instance.SetTime(2024, 1, 13, 0, 0);
                break;
            case SunSet.Night:
                EnviroSkyMgr.instance.SetTime(2024, 1, 23, 0, 0);
                break;
        }

        EnviroSkyMgr.instance.Time.ProgressTime =
            StartTime == SunSet.Rotaton ? EnviroTime.TimeProgressMode.Simulated : EnviroTime.TimeProgressMode.None;
    }

    #endregion
    
}
