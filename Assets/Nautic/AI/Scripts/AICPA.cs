

public class CCPA
{
    public CShipPhysicalData krit_SPD1;
    public CShipPhysicalData krit_SPD2;

    public double _entry_zeit = -1d;
    public double _cpa_zeit = -1d;
    public double _cpa_dist = -1d;
    public double dist_nicht_zu_unterschreiten = -1d;
    public double _krit_dist = -1d;
    public double exit_zeit=-1d;
    public int gegnerindex = -1;
    public string gegnername;
    public bool gehe_achter_durch = false;
      
    public CShipPhysicalData cpa_SPD1;
    public CShipPhysicalData cpa_SPD2;
    
    
    public CCPA(CShipPhysicalData krit_SPD1, CShipPhysicalData krit_SPD2, CShipPhysicalData cpa_SPD1, CShipPhysicalData cpa_SPD2)
    {
        this.krit_SPD1 = krit_SPD1;
        this.krit_SPD2 = krit_SPD2;
        
        this.cpa_SPD1 = cpa_SPD1;
        this.cpa_SPD2 = cpa_SPD2;
    }

    public double cpa_gegner_peilt_mich()
    {
        return AIMath.Relativpeilung_Peilung_Punkt(cpa_SPD2.x, cpa_SPD2.z, cpa_SPD2.KdW, cpa_SPD1.x, cpa_SPD1.z);
    }
    
    
    public double krit_zeit()
    {
        return krit_SPD1.timestamp;
    }
    
    public double cpa_zeit()
    {
        return cpa_SPD1.timestamp;
    }
    
    public double krit_distanz()
    {
        return AIMath.Dist_XZ(krit_SPD1.x,krit_SPD1.z,krit_SPD2.x,krit_SPD2.z);
    }
    
    public double cpa_distanz()
    {
        return AIMath.Dist_XZ(cpa_SPD1.x,cpa_SPD1.z,cpa_SPD2.x,cpa_SPD2.z);
    }
    
    
}
