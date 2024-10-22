using System.Collections;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;

/*
Teste MsgBox mit LatLon-Highlight

*/

public static class AIglobal
{
    public static ObjectsInterface m_ObjSpawnerSO;
    public static ScenarioInterface m_channel_map; 
    public static UI_RootInterface m_channel_ui;
    
    
    
    public static List<CArea> Gebiete = new List<CArea>();
    public static List<CFzg> Fahrzeuge = new List<CFzg>();

    public static bool busy = false;
    public static bool reset_timescale = false;
    
    public static double lon_to_m ; 
    public static double lat_to_m ;
    public const bool bsuppressmapoutput=false;
    public const bool bsuppressMsgBox=true;
    public const double deletetrackoptionsafter = 45;
    public const int reactionsteps = 40;
    public const int ES_reactionsteps = 0;
    public const double sim_duration=3600;//in Sekunden
    public static double Windrichtung = 271;
    
    public static List<string> FzgUpdate= new List<string>();
    public static void Fehler(string Meldung="Fehler")
    {
        //Debug.Log(Meldung);
    }
    
    
}
