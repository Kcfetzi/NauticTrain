using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.ResourceManagement;
using Groupup;

public static class AIMap 
{
    public static UI_RootInterface m_channel_ui;
    
    
    public static PolyLine Linie(double lat1, double lon1,double lat2, double lon2,double strength, Color color)
    {
        if (AIglobal.bsuppressmapoutput) return null;
        List<double2> ld2= new List<double2>();
        ld2.Add(new double2(lat1,lon1));ld2.Add(new double2(lat2,lon2));
        PolyLine Polyline2= m_channel_ui.SpawnStaticPolyline(ld2);
        Polyline2.Data.LineThickness = (float) strength;
        Polyline2.Data.Color = color;
        return Polyline2;
    }

    public static PolyLine Polylinie(List<double2> ld2,double strength, Color color, string bezeichner="",string beschreibung="", List<Symbol> LS=null,List<CShipPhysicalData> ListSPD = null)
    {
        int i = 0;
        CShipPhysicalData iSPD;
        Symbol iSymbol;
        if (AIglobal.bsuppressmapoutput) return null;
        ObjectsInterface OI=Groupup.ResourceManager.GetInterface<ObjectsInterface>();
        //List<Symbol> LS = new List<Symbol>();
        //Symbol ObjSymbol = OI.SpawnObjectUnityPos(EcdisType.point, new Vector3((float)ld2[0].x, 0f, (float)ld2[0].y),
        //    Vector3.zero).Symbol;
        foreach (double2 pt in ld2)
        {
            double3 UnityPosition = AIglobal.m_channel_map.WorldToUnityPoint(pt);
            iSymbol=OI.SpawnObjectUnityPos(NauticType.Point, new Vector3((float) UnityPosition.x,(float) UnityPosition.y,(float) UnityPosition.z ),Vector3.zero).Symbol;
            LS?.Add(iSymbol);
            string txt = bezeichner;
            if (txt.Contains("||")) txt = txt.Split("||")[0];
            iSymbol.NauticObject.Data.ObjectName = txt;
            iSymbol.NauticObject.Data.EcdisColor = color;
            iSymbol.NauticObject.Data.EcdisSize=(float) (strength/3);

            if (ListSPD != null)
            {
                iSPD = ListSPD[i];

                iSymbol.NauticObject.Data.Debug1 = beschreibung;
                iSymbol.NauticObject.Data.Debug2="R:" + iSPD.Ruderlage + "/F:" + iSPD.Fahrstufe.ToString("0.0") + "/KdW:" 
                                                 + iSPD.KdW.ToString("0.00") + "/FdW:" + (iSPD.FdW / AIConst.kn).ToString("0.0");
                iSymbol.NauticObject.Data.Debug3 = iSPD.timestamp.ToString("N0") + "s";
                iSymbol.NauticObject.Data.Debug4 = (i + 1.ToString()) + ".Punkt";
            }

            i++;

        }
        //PolyLine Polyline2= m_channel_ui.SpawnStaticPolyline(ld2);
        PolyLine Polyline2= m_channel_ui.SpawnDynamicPolyline(LS);
        Polyline2.Data.LineThickness = (float) strength;
        Polyline2.Data.Color = color;
        return Polyline2;
    }
    
    public static NauticObject Punkt(double lat1, double lon1,double strength, Color color, string description="")
    {
        if (AIglobal.bsuppressmapoutput) return null;
        NauticObject Pt=AIglobal.m_ObjSpawnerSO.SpawnObjectLatLon(NauticType.Point, new double2(lat1,lon1), Vector3.zero);
        Pt.Data.EcdisSize = (float) (strength/80d);
        Pt.Data.EcdisColor = color;
        Pt.Data.Debug1 = "\n\n"+description;
        return Pt;
    }
   
    
}
