using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Extensions
{
       public static double Distance(Position p1, Position p2)
       {
           double delta_lat = p2.Lat - p1.Lat; delta_lat *= Mathf.PI / 180;
           double delta_lon = p2.Lon - p1.Lon; delta_lon*= Mathf.PI / 180;
    
           double a = Math.Pow(Math.Sin(delta_lat / 2d),2d) + Math.Cos(p1.Lat) * Math.Cos(p2.Lat) * Math.Pow(Math.Sin(delta_lon / 2.0d),2d);
           double c = 2d * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
           return 6371f * c;
       }
       
       // Sort list of recttransform by their position in a circle pattern, started left top
       public static List<RectTransform> SortCircular(this List<RectTransform> objects, Vector2 mesureFrom)
       {
           List<RectTransform> above = new List<RectTransform>();
           List<RectTransform> below = new List<RectTransform>();

           foreach (RectTransform rectTransform in objects)
           {
               if (rectTransform.anchoredPosition.y >= mesureFrom.y) 
                   above.Add(rectTransform);
               else
                   below.Add(rectTransform);
           }

           above = above.OrderBy(value => value.anchoredPosition.x).ToList();
           below = below.OrderByDescending(value => value.anchoredPosition.x).ToList();
           return above.Union(below).ToList();
       }
       
       // Find the middlepoint in given transforms
       public static Vector2 MiddlePosition(this List<RectTransform> objects)
       {
           int count = objects.Count;
           float x = 0f;
           float y = 0f;

           foreach (RectTransform transform in objects)
           {
               x += transform.anchoredPosition.x;
               y += transform.anchoredPosition.y;
           }

           return new Vector2(x / count, y / count);
       }
       
       // GetSize of a rect including all given recttransform
       public static Vector2 GetRectSize(this List<RectTransform> objects)
       {
           Vector2 highest = Vector2.zero;
           Vector2 lowest = Vector2.zero;

           foreach (RectTransform rectTransform in objects)
           {
               Vector2 pos =  rectTransform.anchoredPosition;

               highest.x = pos.x > highest.x ? pos.x : highest.x;
               highest.y = pos.y > highest.y ? pos.y : highest.y;
               
               lowest.x = pos.x < lowest.x ? pos.x : lowest.x;
               lowest.y = pos.y < lowest.y ? pos.y : lowest.y;
           }

           return new Vector2(Mathf.Abs(lowest.x - highest.x), Mathf.Abs(lowest.y - highest.y));
       }
       
       // Rotate a Vector2
       public static Vector2 Rotate(this Vector2 v, float degrees) {
           float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
           float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
           float tx = v.x;
           float ty = v.y;
           v.x = (cos * tx) - (sin * ty);
           v.y = (sin * tx) + (cos * ty);
           return v;
       }
       
       // Lerp-Methode für double-Werte
       public static double Lerp(double a, double b, double t)
       {
           t = Clamp01(t); // Clamp t zwischen 0 und 1, um sicherzustellen, dass es sich um eine gültige Interpolationsgröße handelt
           return a + (b - a) * t;
       }
       
       // Clamp-Methode für double-Werte
       public static double Clamp01(double value)
       {
           if (value < 0.0) return 0.0;
           if (value > 1.0) return 1.0;
           return value;
       }
}
