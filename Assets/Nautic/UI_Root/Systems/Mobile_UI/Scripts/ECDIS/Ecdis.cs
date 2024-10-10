using System;
using System.Collections.Generic;
using Groupup;
using UnityEngine;
/**
 * The ECDIS system.
 */
public class Ecdis : MonoBehaviour
{
    [Header("Ecdiscomponents")]
    [SerializeField]
    private EcdisMapDisplay _ecdisMapDisplay;
    // The data display
    [SerializeField] private EcdisDataDisplay _ecdisDataDisplay;

    public EcdisMapDisplay EcdisMapDisplay => _ecdisMapDisplay;
    public EcdisDataDisplay DataDisplay => _ecdisDataDisplay;
    

    // Setup Ecdis, store terrainsize (this should not change during gameplay)
    public void Init(Texture2D map)
    {
        //_ecdisDataDisplay.Init();
        _ecdisMapDisplay.Init(map);

        ObjectsInterface objectsInterface = ResourceManager.GetInterface<ObjectsInterface>();
        objectsInterface.OnDeleteNauticObject += DeleteNauticObject;
        
        UI_RootInterface UIInterface = ResourceManager.GetInterface<UI_RootInterface>();
        UIInterface.OnDeletePolyLine += DeletePolyLine;
        UIInterface.OnSpawnStaticPolyline += _ecdisMapDisplay.SpawnStaticPolyline;
        UIInterface.OnSpawnDynamicPolyline += _ecdisMapDisplay.SpawnDynamicPolyline;
        UIInterface.OnRegisterNauticObject += _ecdisMapDisplay.RegisterNauticObject;
    }
    
    /**
     * OnClick in symbol, this method is called and shows the object from symbol.
     * Set to null if no object is selected
     */
    public void SetSelectedNauticObject(NauticObject obj)
    {
        // set selected object on the map
        _ecdisMapDisplay.SetSelectedObject(obj);
        
        // set the selected objects on dataDisplay
        _ecdisDataDisplay.SetSelectedObject(obj);
    }
    
    private void DeleteNauticObject(NauticObject obj)
    {
        _ecdisDataDisplay.DeleteNauticObject(obj);
        _ecdisMapDisplay.DeleteNauticObject(obj);
    }

    private void DeletePolyLine(string id)
    {
        _ecdisMapDisplay.DeletePolyLine(id);
        //_ecdisDataDisplay.UpdateView();
    }
    
}
