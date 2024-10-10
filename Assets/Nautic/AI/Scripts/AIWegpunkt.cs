using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWegpunkt                //-----------------------------------------Klasse--------CWegpunkt
{
    public double lat;
    public double lon;
    public int auftrag;                           //0=Fahre zu,              1=Warte Zeit,        2=Fahre zurück zu Punkt
    public double auftrag_geschwindigkeit;       //bei 0:Geschwindigkeit,   bei 1: Wartezeit(s)  
    public double auftrag_folgewegpunkt;               //anderer Wegpunkt
    public double auftrag_wartezeit;
    public CWegpunkt(double lat, double lon, int auftrag, double auftrag_geschwindigkeit,double auftrag_folgewegpunkt,double auftrag_wartezeit)
    {
        this.lat = lat;
        this.lon = lon;
        this.auftrag=auftrag;
        this.auftrag_geschwindigkeit=auftrag_geschwindigkeit;
        this.auftrag_folgewegpunkt=auftrag_folgewegpunkt;
        this.auftrag_wartezeit=auftrag_wartezeit;
    }
    public double x { get { return lon* AIglobal.lon_to_m; } set { lon = value/AIglobal.lon_to_m; } }
    public double z { get { return lat* AIglobal.lat_to_m; } set { lat = value/AIglobal.lat_to_m; } }
}
