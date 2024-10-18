
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Groupup;
using Unity.Mathematics;

/** 
   * Structurerfile
   * This is a generated file from Structurer.
   * If you edit this file, stick to the format from Funcs, UnityActions and methods, to keep on getting all benefits from Strukturer.
   */ 
public class UI_RootInterface : InterfaceSOBase
{
   #region Radar

   [Header("Radar")] 
   public Vector2 RadarToWorldRatio;
   public Vector2 WorldToRadarRatio;
   public Vector2 MapSize;
   
   /**
 * ChannelMethod convert a unityworldposition in ecdisposition
 */
   public Vector2 WorldToRadarPosition(Vector3 pos)
   {
      return new ( pos.x * WorldToRadarRatio.x - MapSize.x / 2, pos.z * WorldToRadarRatio.y - MapSize.y / 2);
   }

   /**
    * ChannelMethod convert an ecdisposition into unityworldposition
    */
   public Vector3 RadarToWorldPosition(Vector2 pos)
   {
      return new ((pos.x + MapSize.x / 2) * RadarToWorldRatio.x, 0,
         (pos.y + MapSize.y / 2) * RadarToWorldRatio.y);
   }
   
   #endregion


   #region Ecdis
   
   [Header("Ecdis")]
   public Vector2 WorldToEcdisRatio;
   public Vector2 EcdisToWorldRatio;
   public Vector2 EcdisMapSize;
   public Vector2 EcdisMapScale;
   
   public List<PolyLine> ActiveEcdisLines = new List<PolyLine>();
   public List<Area> ActiveEcdisAreas = new List<Area>();
   public List<Symbol> ActiveEcdisSymbols = new List<Symbol>();

   private void OnEnable()
   {
      ActiveEcdisLines.Clear();
      ActiveEcdisAreas.Clear();
      ActiveEcdisSymbols.Clear();
   }

   /**
 * ChannelMethod convert a unityworldposition in ecdisposition
 */
   public Vector2 WorldToEcdisPosition(Vector3 pos)
   {
      return new(pos.x * WorldToEcdisRatio.x - EcdisMapSize.x / 2, pos.z * WorldToEcdisRatio.y - EcdisMapSize.y / 2);
   }

   /**
    * ChannelMethod convert an ecdisposition into unityworldposition
    */
   public Vector3 EcdisToWorldPosition(Vector2 pos)
   {
      return new ((pos.x + EcdisMapSize.x / 2) * EcdisToWorldRatio.x, 0,
         (pos.y + EcdisMapSize.y / 2) * EcdisToWorldRatio.y);
   }

   #endregion
   
   #region observer

   public UnityAction<Texture2D> OnInitUI;
   public UnityAction<ObjectContainer> OnInitShipValues;
   
   public UnityAction<NauticObject> OnRegisterNauticObject;

   public Func<List<Symbol>, PolyLine> OnSpawnDynamicPolyline;
   public Func<List<double2>, PolyLine> OnSpawnStaticPolyline;
   public UnityAction<string> OnDeletePolyLine;


   public void Reset()
   {
      OnInitUI = null;
      OnInitShipValues = null;
      OnRegisterNauticObject = null;
      OnSpawnDynamicPolyline = null;
      OnSpawnStaticPolyline = null;
      OnDeletePolyLine = null;
   }
   
   
   /**
    * Init the ui
    */
   public void InitUI(Texture2D map)
   {
      OnInitUI?.Invoke(map);
   }
   
   
   /**
    * Init ecdis with the map image and terrainsize. It will calculate transformation consts for worldtoecdis and ecdistoworld
    */
   public void InitShipValues(ObjectContainer container)
   {
      OnInitShipValues?.Invoke(container);
   }

   /**
    * Register a Ecdiselement
    */
   public void RegisterNauticObject(NauticObject obj)
   {
      OnRegisterNauticObject?.Invoke(obj);
   }


   /**
    * Delete a Lineelement
    */
   public void DeletePolyLine(string id)
   {
      OnDeletePolyLine?.Invoke(id);
   }

   public PolyLine SpawnStaticPolyline(List<double2> points)
   {
      return OnSpawnStaticPolyline?.Invoke(points);
   }
   public PolyLine SpawnDynamicPolyline(List<Symbol> symbols)
   {
      return OnSpawnDynamicPolyline?.Invoke(symbols);
   }
   #endregion
   
   #region Funk

   public UnityAction<string, bool, bool> OnSetFunkMessage;
    
   public void SetFunkMessage(string msg, bool openChannel, bool incoming)
   {
      OnSetFunkMessage?.Invoke(msg, openChannel, incoming);
   }

   #endregion
}
