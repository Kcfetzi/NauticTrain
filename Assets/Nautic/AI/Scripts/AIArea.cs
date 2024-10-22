using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;
using System.Data.SqlTypes;

public class CArea          //-----------------------------------------Klasse--------CArea-----------------------
   {
       public string name;
       public int typ; //cAreaTyp_VTG
       public List<double2> Punkte;
       private PolyLine PL ;
       private List<PolyLine> listePL = new List<PolyLine>();
       private List<Symbol> ListeSymbols = new List<Symbol>();
       public Color bordercolor = Color.white;
       public double richtung = double.NaN;
       public double strength;
       
       
       
       public const double grad = AIConst.grad; //RAD->GRAD
       public const double kn=AIConst.kn;  //0.514 kn->m/s
       public const double sm = AIConst.sm; //meter
       
       public CArea(string name, int typ, List<double2> Punkte, Color bordercolor, double strength, double richtung= double.NaN)
       {
          this.name=name;
          this.typ=typ;
          this.Punkte=Punkte;
          this.bordercolor=  bordercolor;
          this.richtung= richtung;
          this.strength = strength;
          AIglobal.Gebiete.Add(this); 
       }
       
       public void Map_Anzeige()
       {
           List<double2> ld2 = new List<double2>(Punkte);
           if (Punkte[0].x!=Punkte[^1].x || Punkte[0].y!=Punkte[^1].y) ld2.Add(new double2(Punkte[0].x,Punkte[0].y));

           listePL.Add  (AIMap.Polylinie(ld2,strength,bordercolor,name,typ.ToString(),ListeSymbols ));
                      
           //Pfeil
           if (typ == AIConst.cAreaTyp_VTG)
           {
               double2 mitte = Mittelpunkt();
               double llat = max_lat() - min_lat();
               double llon = max_lon() - min_lon();
               double llen = (llat + llon) * .1d;
               double2 pfsp=new double2(mitte.x + llen * Math.Cos(richtung * grad),mitte.y + llen * Math.Sin(richtung * grad));//Pfeilspitze
               listePL.Add(AIMap.Linie(mitte.x - llen * Math.Cos(richtung * grad), mitte.y - llen * Math.Sin(richtung * grad), pfsp.x,pfsp.y, strength, bordercolor));
               listePL.Add(AIMap.Linie(pfsp.x,pfsp.y,pfsp.x + llen * Math.Cos((richtung-160) * grad), pfsp.y + llen * Math.Sin((richtung-160) * grad),  strength, bordercolor));
               listePL.Add(AIMap.Linie(pfsp.x,pfsp.y,pfsp.x + llen * Math.Cos((richtung-200) * grad), pfsp.y + llen * Math.Sin((richtung-200) * grad),  strength, bordercolor));
           }
       }

       public void Map_Entferne()
       {
           foreach (PolyLine PL in listePL) if (PL) AIglobal.m_channel_ui.DeletePolyLine(PL.Data.UID);
       }
       
       
       
       /*
        * public static bool IsPointInPolygon4(PointF[] polygon, PointF testPoint)
    {
        bool result = false;
        int j = polygon.Count() - 1;
        for (int i = 0; i < polygon.Count(); i++)
        {
            if (polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y || polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y)
            {
                if (polygon[i].X + (testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < testPoint.X)
                {
                    result = !result;
                }
            }
            j = i;
        }
        return result;
    }
        */
       //                               x           y
       public bool Pos_enthalten(double lat, double lon)
       {
           bool result = false;//
           int j = Punkte.Count- 1;//
           double2 P1 = Punkte[j];
           for (int i = 0; i < Punkte.Count; i++)//
           {
               double2 P0 = Punkte[i];
               /*if (Punkte[i].y < lon && Punkte[j].y >= lon || Punkte[j].y < lon && Punkte[i].y >= lon)  //
               {
                   if (Punkte[i].x + ((lon - Punkte[i].y) / (Punkte[j].y - Punkte[i].y)) * (Punkte[j].x - Punkte[i].x) <lat)
                   {
                       result = !result;
                   }
               }*/
               if (P0.y < lon && P1.y >= lon || P1.y < lon && P0.y >= lon)  //
               {
                   if (P0.x + ((lon - P0.y) / (P1.y - P0.y)) * (P1.x - P0.x) <lat)
                   {
                       result = !result;
                   }
               }
               j = i;
               P1 = Punkte[j];
           }
           return result;
       }
       
       public bool faehrt_rein(double px, double pz, double kurs ,ref double dmin, ref double dmin_x, ref double dmin_z, ref double richtung)
       {
           bool result = false;//
           int j = Punkte.Count- 1;//
           double2 P0 = Punkte[j];
           double sx = 0, sz = 0;
           bool bschnittauf = false;
           double d = 0;
           dmin=-1;
           
           
           for (int i = 0; i < Punkte.Count; i++)//
           {
               double2 P1 = Punkte[i]; //P.x ist lat ->entspricht y-Achse    , P.y ist lon  -> entspricht x-Achse

               bschnittauf=AIMath.Schnitt_Richtung1_auf_Gerade23(px, pz, kurs, P0.y * AIglobal.lon_to_m, P0.x * AIglobal.lat_to_m,
                                                               P1.y * AIglobal.lon_to_m, P1.x * AIglobal.lat_to_m, ref sx, ref sz);

               if (bschnittauf)
               {
                   d= AIMath.Dist_XZ(px, pz, sx, sz);
                   if (d < dmin || dmin < 0)
                   {
                       dmin = d;
                       dmin_x = sx;
                       dmin_z = sz;
                       richtung = AIMath.Absolut_Peilung_Punkt(P0.x, P0.y, P1.x, P1.y);
                   }
               }
               
               P0 = P1;
           }
           
           return result;
       }
       
       
       public bool Gerade_schneidet(double p1x, double p1z,double p2x,double p2z)
              {
                  bool result = false;//
                  int j = Punkte.Count- 1;//
                  double2 P0 = Punkte[j];
                  double sx = 0, sz = 0;
                  bool bschneidet = false;
                  
                  for (int i = 0; i < Punkte.Count; i++)//
                  {
                      double2 P1 = Punkte[i]; //P.x ist lat ->entspricht y-Achse    , P.y ist lon  -> entspricht x-Achse
       
                      bschneidet = AIMath.Schnitt_Gerade1XZ_Gerade2XZ(p1x, p1z, p2x, p2z, P0.y * AIglobal.lon_to_m, P0.x * AIglobal.lat_to_m,
                          P1.y * AIglobal.lon_to_m, P1.x * AIglobal.lat_to_m, ref sx, ref sz);

                      if (bschneidet) return true;
                      
                      P0 = P1;
                  }
                  
                  return false;
              }
       
       
       
       
       
       
       
       
       
       double2 Mittelpunkt()
       {
           if (Punkte.Count == 0) return new double2(double.NaN, double.NaN);
           double2 summe = new double2(0d, 0d);
           foreach (double2 pt in Punkte) summe += pt;
           return summe/Punkte.Count;
       }
       double max_lat()
       {
           double max = double.NaN; foreach (double2 pt in Punkte) if (pt.x>max || double.IsNaN((max))) max=pt.x;
           return max;
       }
       double max_lon()
       {
           double max = double.NaN; foreach (double2 pt in Punkte) if (pt.y>max || double.IsNaN((max))) max=pt.y;
           return max;
       }
       double min_lat()
       {
           double min = double.NaN; foreach (double2 pt in Punkte) if (pt.x<min || double.IsNaN((min))) min=pt.x;
           return min;
       }
       double min_lon()
       {
           double min = double.NaN; foreach (double2 pt in Punkte) if (pt.y<min || double.IsNaN((min))) min=pt.y;
           return min;
       }
   }
   
