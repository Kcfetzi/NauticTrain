using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class GeoUtils
{
    private const double EarthRadiusKm = 6371e3;// Radius der Erde in Metern
    /*Haversine formula:
       a = sin²(Δφ/2) + cos φ1 ⋅ cos φ2 ⋅ sin²(Δλ/2)
       c = 2 ⋅ atan2( √a, √(1−a) )
       d = R ⋅ c
       where	φ is latitude, λ is longitude, R is earth’s radius (mean radius = 6,371km);
       note that angles need to be in radians to pass to trig functions!
    */    
    public static double CalculateRealWorldDistance(Position p1, Position p2)
    {
        double delta_lat = (p2.LatLon[0] - p1.LatLon[0]) * Mathf.PI / 180.0;
        double delta_lon = (p2.LatLon[1] - p1.LatLon[1]) * Mathf.PI / 180.0;
    
        double lat1_rad = p1.LatLon[0] * Mathf.PI / 180.0;
        double lat2_rad = p2.LatLon[0] * Mathf.PI / 180.0;
        
        double a = Math.Pow(Math.Sin(delta_lat / 2.0), 2.0) +
                   Math.Cos(lat1_rad) * Math.Cos(lat2_rad) *
                   Math.Pow(Math.Sin(delta_lon / 2.0), 2.0);
        double c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));
        return EarthRadiusKm * c;
    }
    
    
    // Funktion zur Berechnung der Distanz zwischen zwei geographischen Punkten
    public static double CalculateRealWorldDistance(double lat1, double lon1, double lat2, double lon2)
    { 
        // Konvertiere Grad in Radians
        double lat1_rad = lat1 * Math.PI / 180.0;
        double lon1_rad = lon1 * Math.PI / 180.0;
        double lat2_rad = lat2 * Math.PI / 180.0;
        double lon2_rad = lon2 * Math.PI / 180.0;

        double delta_lat = lat2_rad - lat1_rad;
        double delta_lon = lon2_rad - lon1_rad;

        // Haversine-Formel
        var a = Math.Pow(Math.Sin(delta_lat / 2), 2) +
                Math.Cos(lat1_rad) * Math.Cos(lat2_rad) *
                Math.Pow(Math.Sin(delta_lon / 2), 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c; 
    }
    
}
