using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json; 

public class test : MonoBehaviour
{
    private void Start()
    {
        List<Ton> tons = new List<Ton>();
        
        Ton ton = new Ton();
        ton.Type = TonType.lighted;
        ton.TonName = "tonname";
        ton.Lat = 38.42575;
        ton.Lon = 15.87878;
        
        Ton ton2 = new Ton();
        ton.Type = TonType.unlighted;
        ton.TonName = "tonname2";
        ton.Lat = 38.12575;
        ton.Lon = 15.87878;
        
        tons.Add(ton);
        tons.Add(ton2);


        Debug.Log(JsonConvert.SerializeObject(tons));
    }
}
