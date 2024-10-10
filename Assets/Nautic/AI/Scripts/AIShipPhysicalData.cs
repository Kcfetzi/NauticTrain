using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CShipPhysicalData //---------------------------Klasse--------CShipPhysicalData--------------------
{
    public double lat;
    public double lon;
    public double Fahrstufe;
    public double Ruderlage;
    public double FdW;
    public double KdW;
    public double KuG;
    public double Winkelv;
    public double timestamp;

    public CShipPhysicalData(double lat, double lon, double Fahrstufe, double Ruderlage, double FdW, double KdW, double Winkelv)
    {
        this.lat = lat;
        this.lon = lon;
        this.Fahrstufe=Fahrstufe;
        this.Ruderlage=Ruderlage;
        this.FdW=FdW;
        this.KdW=KdW;
        this.Winkelv=Winkelv;
    }
    public CShipPhysicalData Copy(double timestamp)
    {
        CShipPhysicalData Tmp=new CShipPhysicalData(lat,lon,Fahrstufe,Ruderlage,FdW,KdW,Winkelv);
        Tmp.timestamp = timestamp;
        return Tmp;
    }
    public CShipPhysicalData Copy() {return Copy(timestamp);}
       
    public double x { get { return lon* AIglobal.lon_to_m; } set { lon = value/AIglobal.lon_to_m; } }
    public double z { get { return lat* AIglobal.lat_to_m; } set { lat = value/AIglobal.lat_to_m; } }
}
