using System.Collections;
using System.Collections.Generic;
using Groupup;
using Newtonsoft.Json;
using Unity.Mathematics;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private TextAsset _mapTons;

    public void Init()
    {
        ObjectsInterface objectsInterface = ResourceManager.GetInterface<ObjectsInterface>();


        List<Ton> tons = JsonConvert.DeserializeObject<List<Ton>>(_mapTons.text);

        foreach (Ton ton in tons)
        {
            NauticObject obj = objectsInterface.SpawnObjectLatLon(NauticType.Ton, new double2(ton.Lat, ton.Lon), Vector3.zero);
            obj.Data.ObjectName = ton.TonName;
            obj.name = ton.TonName;
        }
    }
}
