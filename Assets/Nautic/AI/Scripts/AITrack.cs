using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class CTrack            //-----------------------------------------Klasse--------CTrack-----------------------
   {
       public List<CShipPhysicalData> ListeSPD=new List<CShipPhysicalData>();
       public string name  ="";
       private List<NauticObject> ListeNO = new List<NauticObject>();
       private List<Symbol> ListeSymbols = new List<Symbol>();
       private PolyLine PL;   
       public int itmp = 0;
       public double expiry_time = AIglobal.sim_duration;
       public bool displayed = false;
       public CShipPhysicalData SPD(int i)
       {
           try
           {
               if (i < ListeSPD.Count) return ListeSPD[i];else return ListeSPD[^1];
           }
           catch
           {
               
           }

           return null;
       }
       
       public CShipPhysicalData SPD(double itime)
       { //kann noch schneller mit Dreisatz erstes Element.timestapm und letztes Element.timestamp
         //kann auch smoother mit  Dreisatz zwischen den 2 zugehörigen SPD-Elementen
         //oder noch smoother mit simulate(SPD) mit delta_t=(t-t0)
           for (int i = 0; i < ListeSPD.Count; i++)
           {
               if (ListeSPD[i].timestamp >= itime) return ListeSPD[i];
           }
           return null; //nicht so weit vorausgerechnet? Fehler!
       }
       public int index(double itime)
       { //kann noch schneller mit Dreisatz erstes Element.timestapm und letztes Element.timestamp
           //kann auch smoother mit  Dreisatz zwischen den 2 zugehörigen SPD-Elementen
           //oder noch smoother mit simulate(SPD) mit delta_t=(t-t0)
           for (int i = 0; i < ListeSPD.Count; i++)
           {
               if (ListeSPD[i].timestamp >= itime) return i;
           }

           AIglobal.Fehler("index in Track nicht gefunden");
           return ListeSPD.Count-1; //nicht so weit vorausgerechnet? Fehler!
       }

       public double wegstrecke(int ab,int stepsize=1)  //in m
       {
           double dist = 0;
           CShipPhysicalData SPD1=null, SPD2=null; 
           
           
           for (int i = ab; i < ListeSPD.Count-1; i+=stepsize)
           {
               SPD2= ListeSPD[i];
               if (SPD1!=null)  dist+=AIMath.Dist_XZ(SPD1.x,SPD1.z,SPD2.x,SPD2.z);
               SPD1 = SPD2;
           }
           return dist;
       }
       
       public double gesamtzeit()
       {
           return ListeSPD[^1].timestamp;
       }
       
       //linesinsteadofpoints to save ressurces for Martin
       public void Map_Anzeige(double zeitzwischen2punkten, Color? col=null,int abindex=0, bool linesinsteadofpoints=false)
       {
           if (abindex >= ListeSPD.Count) return;
           double t0 = ListeSPD[abindex].timestamp;
           CShipPhysicalData oldSPD = null;
           CShipPhysicalData firstSPD = null;
           List<double2> ld2 = new List<double2>();
           List<CShipPhysicalData> lSPD = new List<CShipPhysicalData>();
           
           for(int i= abindex;i<ListeSPD.Count;i++)
           {
               CShipPhysicalData SPD = ListeSPD[i];
               if (true) //(SPD.timestamp >= t0)
               {
                   if (linesinsteadofpoints)
                   {
                       if (oldSPD != null)
                       {
                           if (((oldSPD.Fahrstufe == SPD.Fahrstufe) && (oldSPD.Ruderlage == SPD.Ruderlage))&& (i!=ListeSPD.Count-1) && ld2.Count>1)
                           {
                               ld2.RemoveAt(ld2.Count-1);
                               lSPD.RemoveAt(ld2.Count-1);
                           }
                       }
                       ld2.Add(new double2(SPD.lat,SPD.lon));
                       //Symbol S = new Symbol();
                       
                       //S.NauticObject.Data.Debug1 = name;
                       //S.NauticObject.Data.Debug2 = "R:" + SPD.Ruderlage + "/F:" + SPD.Fahrstufe.ToString("0.00") + "/KdW:" + SPD.KdW.ToString("0.00") + "/FdW:" + (SPD.FdW / AIConst.kn).ToString("0.00") + "kn";
                       //NauticObject NO = AIMap.Punkt(SPD.lat, SPD.lon, 2, col ?? Color.cyan);
                       //NO.Data.Debug2 = "R:" + SPD.Ruderlage + "/F:" + SPD.Fahrstufe.ToString("0.00") + "/KdW:" + SPD.KdW.ToString("0.00") + "/FdW:" + (SPD.FdW / AIConst.kn).ToString("0.00") + "kn";
                       //ListeSymbols.Add(S);
                       lSPD.Add(SPD);
                       oldSPD = SPD;
                   }
                   else
                   {
                      ListeNO.Add(AIMap.Punkt(SPD.lat, SPD.lon, 2, col ?? Color.cyan));
                   }
                   
                   t0 += zeitzwischen2punkten; 
               }
           }

           if (ld2.Count < 2)
           {
               
           }

           
           
           if (linesinsteadofpoints) PL=AIMap.Polylinie(ld2, 2, col ?? Color.cyan,name,ListeSymbols,lSPD);
       }

       public void Map_Entferne()
       {
           try
           {
               foreach (NauticObject NO in ListeNO)
                   if (NO)
                       AIglobal.m_ObjSpawnerSO.DeleteNauticObject(NO);
               ListeNO.Clear();

               if (PL) AIglobal.m_channel_ui.DeletePolyLine(PL.Data.UID);
           }
           catch
           {
               
           }
           ObjectsInterface OI=Groupup.ResourceManager.GetInterface<ObjectsInterface>();
           //ListeSymbols
           foreach (Symbol SO in ListeSymbols)
               if (SO)
                   OI.DeleteNauticObject(SO.NauticObject);
           
       }
       
       public CTrack copy()
       {
           CTrack NTrack=new CTrack();
           NTrack.name = name;
           NTrack.expiry_time = expiry_time;
           foreach(CShipPhysicalData SPD in ListeSPD) NTrack.ListeSPD.Add(SPD.Copy());
           return NTrack;
       }
       
       public CTrack copyflat_until_index(int untilindex)
       {
           int i= 0;
           CTrack NTrack=new CTrack();
           NTrack.name = name;
           foreach (CShipPhysicalData SPD in ListeSPD)
           {
               NTrack.ListeSPD.Add(SPD);
               i++;
               if (i >= untilindex) break;
           }
           return NTrack;
       }
       
       public CTrack copyflat()
       {
           int i= 0;
           int untilindex=ListeSPD.Count;
           ;
           CTrack NTrack=new CTrack();
           NTrack.name = name;
           foreach (CShipPhysicalData SPD in ListeSPD)
           {
               NTrack.ListeSPD.Add(SPD);
               i++;
               if (i >= untilindex) break;
           }
           return NTrack;
       }
   }

