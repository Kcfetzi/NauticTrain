
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;
using System.Net.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Groupup;
using Unity.Android.Types;

//TODO CPA: check, ob dazwischen eine NOGO-Area liegt, dann kein CPA: Schnitt_Gerade1XZ_Gerade2XZ



public class CFzg
{      //time fix von martin
       private ScenarioInterface _scenarioInterface;
       public const double grad = AIConst.grad; //RAD->GRAD
       public const double kn=AIConst.kn;  //0.514 kn->m/s
       public const double sm = AIConst.sm; //meter
       
       
       public double t_lastupdate = -1; //not set yet
       
       
       public NauticObject ObjSC;      //Map-Objekt, das das Schiff repräsentiert
       
       public string name = "";         //Schiffsname
       public Color farbe = Color.grey;
       public int ich = -1;             //Index aus der Liste an fahrzeugen  
       public bool busy = false;
       public double tmp_calc_time=0;
       
       public CShipPhysicalData firstSPD;  //Objekt mit den Schiffsdaten(lat/Lon,Bearing/Geschwindigkeit); Objekt, da auch in Track verwendet
       public CPlan Plan=new CPlan();      //mit den ganzen Wegpunkten und Anweisungen, wo und wie zu fahren ist
       public CTrack Track = new CTrack(); //Track Liste mit CShipPhysicalData und Repräsentation auf der Map
       public CTrack DarstellungTrack= new CTrack(); //Track Liste mit CShipPhysicalData und Repräsentation auf der Map
       
       //Verhalten
       public double Nahbereich=0.7*sm; //(Meter) ab wann wird das Notfallausweichmanöver durchgeführt, wenn überhaupt bei Gegnerschiffen(?) bzw nur Gegner vs Gegner(?)
       public double Entf_Ausweichen = 1.5d * sm;               //(Meter) spätestens bei #Entf_Ausweichen Abstand > Beginn des Ausweichmanövers  ...     bei Sichtbedingungen und bestehender Ausweichpflicht:
       public double faktor_Korrektur = 0.5;      //(Meter) spätestens bei faktor_Korrektur*krit_Entf Abstand > zweites Ausweichmanöver  
       public double faktor_LetztesManoever= 0.25;//(Meter) spätestens bei faktor_LetztesManoever*krit_Entf  > aufstoppen und warten
                                                 
       public double krit_Entf_kreuz = 1 * sm;
       public double krit_Entf_entgegen = 0.8 * sm;
       public double krit_Entf_ueberholen = 0.5 * sm;
       public double krit_Entf_ortsfest = 0.1 * sm;
       
       public double krit_Entf_kreuz_VTG = 1 * sm;
       public double krit_Entf_entgegen_VTG = 0.8 * sm;
       public double krit_Entf_ueberholen_VTG = 0.5 * sm;
       public double krit_Entf_ortsfest_VTG = 0.1 * sm;

       public int std_ruderlage = 15; //kursaenderungen finden mit einer standard-Ruderlage statt
       
       //-Eigenschaften-
       public double laenge = 25d, breite = 8d, hoehe = 7d;
       
       //Erreichbare Gecshwindigkeit (Zeile 2) abhängig von Fahrtstufe(Zeile 1)
       List<double> FAHRSTUFE_vmax = new List<double>() {-5d,      -4d,    -3d,  -2d,     -1d, 0d,      1d,      2d,      3d,      4d,      5d,      6d,      7d,      8d,      9d,     10d,      11d,      12d,      13d,      14d,      15d,      16d,      17d,   18d};
       List<double> fahrstufe_VMAX = new List<double>() {-8*kn,-6.5*kn,-5.4*kn,-4*kn,-2.0d*kn, 0d, 0.7d*kn, 1.0d*kn, 1.7d*kn, 2.2d*kn, 3.7d*kn, 4.9d*kn, 6.1d*kn, 7.5d*kn, 8.6d*kn, 9.7d*kn, 10.8d*kn, 11.8d*kn, 12.7d*kn, 13.6d*kn, 14.5d*kn, 15.6d*kn, 16.6d*kn, 17.5f};
      
       List<double> mj_FAHRSTUFE_vmax = new List<double>() {-5d,      -4d,    -3d,  -2d,     -1d, 0d,      1d,      2d,      3d,      4d,      5d,      6d,      7d,      8d,      9d,     10d,      11d,      12d,      13d,      14d,      15d,      16d,      17d,   18d};
       List<double> mj_fahrstufe_VMAX = new List<double>() {-8*kn,-6.5*kn,-5.4*kn,-4*kn,-2.0d*kn, 0d, 0.7d*kn, 1.0d*kn, 1.7d*kn, 2.2d*kn, 3.7d*kn, 4.9d*kn, 6.1d*kn, 7.5d*kn, 8.6d*kn, 9.7d*kn, 10.8d*kn, 11.8d*kn, 12.7d*kn, 13.6d*kn, 14.5d*kn, 15.6d*kn, 16.6d*kn, 17.5f};
       
       //Beschleunigung gleichförmig unabhängig von ggw Speed, 1.Zeile:Fahrstufe, 2.Zeile Beschleunigung=delta_v/delta_t
       List<double> FAHRSTUFE_a = new List<double>() {      -5d,        -4d,        -3d,     -2d ,         -1d,0d   ,         1d,         2d,         3d,         4d,         5d,         6d,         7d,         8d,         9d,        10d,         11d,         12d,         13d,         14d,         15d,         16d,         17d,         18d};
       List<double> fahrstufe_A = new List<double>() {-8*kn/90d,-6.5*kn/90d,-5.4*kn/90d,-4*kn/90d,-2.0d*kn/90d,0.00d, 0.4*kn/277, 0.9*kn/199, 1.8*kn/154, 2.9*kn/131, 3.9*kn/122, 5.0*kn/119, 6.0*kn/118, 7.0*kn/118, 8.1*kn/116, 9.1*kn/112, 10.1*kn/108, 11.2*kn/104, 12.2*kn/101, 13.3*kn/102, 14.3*kn/105, 15.3*kn/111, 16.4*kn/117, 17.4*kn/122};
       
       List<double> mj_FAHRSTUFE_a = new List<double>() {      -5d,        -4d,        -3d,     -2d ,         -1d,0d   ,         1d,         2d,         3d,         4d,         5d,         6d,         7d,         8d,         9d,        10d,         11d,         12d,         13d,         14d,         15d,         16d,         17d,         18d};
       List<double> mj_fahrstufe_A = new List<double>() {-8*kn/90d,-6.5*kn/90d,-5.4*kn/90d,-4*kn/90d,-2.0d*kn/90d,0.00d, 0.4*kn/277, 0.9*kn/199, 1.8*kn/154, 2.9*kn/131, 3.9*kn/122, 5.0*kn/119, 6.0*kn/118, 7.0*kn/118, 8.1*kn/116, 9.1*kn/112, 10.1*kn/108, 11.2*kn/104, 12.2*kn/101, 13.3*kn/102, 14.3*kn/105, 15.3*kn/111, 16.4*kn/117, 17.4*kn/122};
       // (Max Negativgeschw wird nach jeweis 90 sek erreicht)
       
       double modifier_winkela=1.0;       //Wie träge er auf Ruderänderungen reagiert
       double modifier_winkelvmax=1.0;    //wie schn ell er maximal drehen kann ->Drehkreis
       double mj_modifier_winkela=1.0;       //Wie träge er auf Ruderänderungen reagiert
       double mj_modifier_winkelvmax=1.0;    //wie schn ell er maximal drehen kann ->Drehkreis
       
       //Negativbeschleunigung (Zeile 2-4) abhängig von Geschwindigkeit (Zeile 1)
       List<double> V_a = new List<double>()        {0d*kn, 7d*kn, 14d*kn, 17.5d*kn}; 
       List<double> v_A_delta1 = new List<double>() {0d,   -0.02d,-0.015d,   -0.02d}; //Fahrt um 1  reduzieren  (m/s*s);
       List<double> v_A_stop = new List<double>()   {0d,   -0.04d, -0.06d,   -0.08d}; //Fahrt auf 0 reduzieren  (m/s*s);
       List<double> v_A_back = new List<double>()   {-0.1d,  -0.1d, -0.15d,    -0.2d}; //Fahrt voll zurück (hier minus -10) (m/s*s);
       
       List<double> mj_V_a = new List<double>()        {0d*kn, 7d*kn, 14d*kn, 17.5d*kn}; 
       List<double> mj_v_A_delta1 = new List<double>() {0d,   -0.02d,-0.015d,   -0.02d}; //Fahrt um 1  reduzieren  (m/s*s);
       List<double> mj_v_A_stop = new List<double>()   {0d,   -0.04d, -0.06d,   -0.08d}; //Fahrt auf 0 reduzieren  (m/s*s);
       List<double> mj_v_A_back = new List<double>()   {-0.1d,  -0.1d, -0.15d,    -0.2d}; //Fahrt voll zurück (hier minus -10) (m/s*s);
       
       //die Geschwindigkeit rediziert sich bei einer Ruderlage, siehe fFahrstufe_MaxSpeed(double ifahrstufe, double iRuderlage)
       /*private static*/
       List<double> FAHRSTUFE_vminusruder = new List<double>()  {-5d     , 0d,      7d,     11d,    15d};
       List<double> fahrstufe_VMINUSRUDER0 = new List<double>() {-0.0d*kn, 0d, 0.0d*kn, 0.0d*kn,0.0d*kn};
       List<double> fahrstufe_VMINUSRUDER5 = new List<double>() {-0.4d*kn, 0d, 0.2d*kn, 0.7d*kn,0.8d*kn};
       List<double> fahrstufe_VMINUSRUDER10 = new List<double>(){-1.4d*kn, 0d, 1.0d*kn, 2.7d*kn,3.0d*kn};
       List<double> fahrstufe_VMINUSRUDER15 = new List<double>(){-2.0d*kn, 0d, 1.7d*kn, 4.0d*kn,5.0d*kn};
       List<double> fahrstufe_VMINUSRUDER20 = new List<double>(){-2.5d*kn, 0d, 2.8d*kn, 4.9d*kn,7.0d*kn};
       List<double> fahrstufe_VMINUSRUDER25 = new List<double>(){-2.9d*kn, 0d, 3.3d*kn, 5.7d*kn,7.8d*kn};
       List<double> fahrstufe_VMINUSRUDER45 = new List<double>(){-3.5d*kn, 0d, 4.3d*kn, 6.9d*kn,8.5d*kn};

       List<double> mj_FAHRSTUFE_vminusruder = new List<double>()  {-5d     , 0d,      7d,     11d,    15d};
       List<double> mj_fahrstufe_VMINUSRUDER0 = new List<double>() {-0.0d*kn, 0d, 0.0d*kn, 0.0d*kn,0.0d*kn};
       List<double> mj_fahrstufe_VMINUSRUDER5 = new List<double>() {-0.4d*kn, 0d, 0.2d*kn, 0.7d*kn,0.8d*kn};
       List<double> mj_fahrstufe_VMINUSRUDER10 = new List<double>(){-1.4d*kn, 0d, 1.0d*kn, 2.7d*kn,3.0d*kn};
       List<double> mj_fahrstufe_VMINUSRUDER15 = new List<double>(){-2.0d*kn, 0d, 1.7d*kn, 4.0d*kn,5.0d*kn};
       List<double> mj_fahrstufe_VMINUSRUDER20 = new List<double>(){-2.5d*kn, 0d, 2.8d*kn, 4.9d*kn,7.0d*kn};
       List<double> mj_fahrstufe_VMINUSRUDER25 = new List<double>(){-2.9d*kn, 0d, 3.3d*kn, 5.7d*kn,7.8d*kn};
       List<double> mj_fahrstufe_VMINUSRUDER45 = new List<double>(){-3.5d*kn, 0d, 4.3d*kn, 6.9d*kn,8.5d*kn};
       
       
       
       
       List<List<double>> Liste_FAHRSTUFE_vminusruder = new List<List<double>>();//{FAHRSTUFE_vminusruder,FAHRSTUFE_vminusruder,FAHRSTUFE_vminusruder,FAHRSTUFE_vminusruder,FAHRSTUFE_vminusruder,FAHRSTUFE_vminusruder,FAHRSTUFE_vminusruder};
       List<List<double>> Liste_fahrstufe_VMINUSRUDER = new List<List<double>>();//{fahrstufe_VMINUSRUDER0,fahrstufe_VMINUSRUDER5,fahrstufe_VMINUSRUDER10,fahrstufe_VMINUSRUDER15,fahrstufe_VMINUSRUDER20,fahrstufe_VMINUSRUDER25,fahrstufe_VMINUSRUDER45};
       
       public bool istManuell=false;      //nur für manuelle Spieler-Schiffe
       //Kategorien gem KVR
       public int schiffstyp = AIConst.cSchiffstyp_Minenjaeger;
       public bool istMaschinenfzg=true;
       public bool istSegler = false;
       public bool istFischer=false;
       public bool istManoevrierbeh = false;
       public bool istManoevrierunf=false;
       public bool istBagger = false;
       public bool istUnterwasserkabelleger=false; 
       
       public CFzg(string name, bool bistManuell, CShipPhysicalData SPD, int schiffstyp=AIConst.cSchiffstyp_Minenjaeger) //Konstruktor
       {
           this.name = name;
           this.istManuell = bistManuell;
           firstSPD = SPD;
           AIglobal.Fahrzeuge.Add(this);
           ich = index(); //setzt den eigenen Index
           this.schiffstyp = schiffstyp;
           
           switch (schiffstyp)
           {
               case AIConst.cSchiffstyp_Faehre:
                   init_PhysikParameter(1.3, 1.0, 0.7, 0.3, 0.6, 0.5, 0.5, 0.5,
                       1.0, 1.0, 1.0, 1.0, 1.0, 1.0);
                   break;
               case AIConst.cSchiffstyp_Containerschiff:
                   init_PhysikParameter(1.1, 1.1, 0.3, 0.15, 0.3, 0.3, 0.3, 0.3,
                       1.0, 1.0, 1.0, 1.0, 1.0, 1.0);
                   break;
               case AIConst.cSchiffstyp_Fischer:
                   init_PhysikParameter(0.6, 1.0, 0.7, 0.7, 0.6, 0.8, 0.8, 0.8,
                       1.0, 1.0, 1.0, 1.0, 1.0, 1.0);
                   break;
               case AIConst.cSchiffstyp_Lotse:
                   init_PhysikParameter(0.5, 1.4, 0.8, 1.5, 1, 1.4, 1.2, 1.4,
                       0.8, 0.8, 0.8, 0.8, 0.8, 0.8);
                   break;
               default: //zB Minenjäger
                   init_PhysikParameter(1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                       1.0, 1.0, 1.0, 1.0, 1.0, 1.0);
                   break;
           }
           
           
           Liste_FAHRSTUFE_vminusruder.Add(FAHRSTUFE_vminusruder);
           Liste_FAHRSTUFE_vminusruder.Add(FAHRSTUFE_vminusruder);
           Liste_FAHRSTUFE_vminusruder.Add(FAHRSTUFE_vminusruder);
           Liste_FAHRSTUFE_vminusruder.Add(FAHRSTUFE_vminusruder);
           Liste_FAHRSTUFE_vminusruder.Add(FAHRSTUFE_vminusruder);
           Liste_FAHRSTUFE_vminusruder.Add(FAHRSTUFE_vminusruder);
           Liste_FAHRSTUFE_vminusruder.Add(FAHRSTUFE_vminusruder);

           Liste_fahrstufe_VMINUSRUDER.Add(fahrstufe_VMINUSRUDER0);
           Liste_fahrstufe_VMINUSRUDER.Add(fahrstufe_VMINUSRUDER5);
           Liste_fahrstufe_VMINUSRUDER.Add(fahrstufe_VMINUSRUDER10);
           Liste_fahrstufe_VMINUSRUDER.Add(fahrstufe_VMINUSRUDER15);
           Liste_fahrstufe_VMINUSRUDER.Add(fahrstufe_VMINUSRUDER20);
           Liste_fahrstufe_VMINUSRUDER.Add(fahrstufe_VMINUSRUDER25);
           Liste_fahrstufe_VMINUSRUDER.Add(fahrstufe_VMINUSRUDER45);
           
           //List<List<double>> Liste_FAHRSTUFE_vminusruder=new List<List<double>>(){FAHRSTUFE_vminusruder,FAHRSTUFE_vminusruder,FAHRSTUFE_vminusruder,FAHRSTUFE_vminusruder,FAHRSTUFE_vminusruder,FAHRSTUFE_vminusruder,FAHRSTUFE_vminusruder};
           //List<List<double>> Liste_fahrstufe_VMINUSRUDER=new List<List<double>>(){fahrstufe_VMINUSRUDER0,fahrstufe_VMINUSRUDER5,fahrstufe_VMINUSRUDER10,fahrstufe_VMINUSRUDER15,fahrstufe_VMINUSRUDER20,fahrstufe_VMINUSRUDER25,fahrstufe_VMINUSRUDER45};
           DNCs_ermitteln();
           AUFSTOPPSTECKEn_ermitteln();
           
           //time fix von martin
           _scenarioInterface = ResourceManager.GetInterface<ScenarioInterface>();
       }

       public void init_PhysikParameter(double mod_v, double mod_minusv, double mod_a, double mod_winkela, double mod_winkelvmax, double mod_a_delta1, double mod_a_stop, double mod_a_back,
                                        double mod_vminusruder5,  double mod_vminusruder10,double mod_vminusruder15,double mod_vminusruder20,double mod_vminusruder25,double mod_vminusruder45)
       {
           //Beschleunigung Fahrstufe
           //Erreichbare Gecshwindigkeit (Zeile 2) abhängig von Fahrtstufe(Zeile 1)
           //List<double> mj_FAHRSTUFE_vmax = new List<double>() {-5d,      -4d,    -3d,  -2d,     -1d, 0d,      1d,      2d,      3d,      4d,      5d,      6d,      7d,      8d,      9d,     10d,      11d,      12d,      13d,      14d,      15d,      16d,      17d,   18d};
           //List<double> mj_fahrstufe_VMAX = new List<double>() {-8*kn,-6.5*kn,-5.4*kn,-4*kn,-2.0d*kn, 0d, 0.7d*kn, 1.0d*kn, 1.7d*kn, 2.2d*kn, 3.7d*kn, 4.9d*kn, 6.1d*kn, 7.5d*kn, 8.6d*kn, 9.7d*kn, 10.8d*kn, 11.8d*kn, 12.7d*kn, 13.6d*kn, 14.5d*kn, 15.6d*kn, 16.6d*kn, 17.5f};
           FAHRSTUFE_vmax.Clear();
           foreach(double v in mj_FAHRSTUFE_vmax) {FAHRSTUFE_vmax.Add(v);}
           fahrstufe_VMAX.Clear();
           foreach(double v in mj_fahrstufe_VMAX) {fahrstufe_VMAX.Add(v*(v>0?mod_v:mod_minusv));}
           
           //Beschleunigung gleichförmig unabhängig von ggw Speed, 1.Zeile:Fahrstufe, 2.Zeile Beschleunigung=delta_v/delta_t
           //List<double> mj_FAHRSTUFE_a = new List<double>() {      -5d,        -4d,        -3d,     -2d ,         -1d,0d   ,         1d,         2d,         3d,         4d,         5d,         6d,         7d,         8d,         9d,        10d,         11d,         12d,         13d,         14d,         15d,         16d,         17d,         18d};
           //List<double> mj_fahrstufe_A = new List<double>() {-8*kn/90d,-6.5*kn/90d,-5.4*kn/90d,-4*kn/90d,-2.0d*kn/90d,0.00d, 0.4*kn/277, 0.9*kn/199, 1.8*kn/154, 2.9*kn/131, 3.9*kn/122, 5.0*kn/119, 6.0*kn/118, 7.0*kn/118, 8.1*kn/116, 9.1*kn/112, 10.1*kn/108, 11.2*kn/104, 12.2*kn/101, 13.3*kn/102, 14.3*kn/105, 15.3*kn/111, 16.4*kn/117, 17.4*kn/122};
           // (Max Negativgeschw wird nach jeweis 90 sek erreicht)
           FAHRSTUFE_a.Clear();
           foreach(double v in mj_FAHRSTUFE_a) {FAHRSTUFE_a.Add(v);}
           mj_fahrstufe_A.Clear();
           foreach(double v in mj_fahrstufe_A) {fahrstufe_A.Add(mod_a*v);}
           
           modifier_winkela=mod_winkela;       //Wie träge er auf Ruderänderungen reagiert
           modifier_winkelvmax=mod_winkelvmax;    //wie schn ell er maximal drehen kann ->Drehkreis
           
           //Negativbeschleunigung (Zeile 2-4) abhängig von Geschwindigkeit (Zeile 1)
           //List<double> mj_V_a = new List<double>()        {0d*kn, 7d*kn, 14d*kn, 17.5d*kn}; 
           //List<double> mj_v_A_delta1 = new List<double>() {0d,   -0.02d,-0.015d,   -0.02d}; //Fahrt um 1  reduzieren  (m/s*s);
           //List<double> mj_v_A_stop = new List<double>()   {0d,   -0.04d, -0.06d,   -0.08d}; //Fahrt auf 0 reduzieren  (m/s*s);
           //List<double> mj_v_A_back = new List<double>()   {-0.1d,  -0.1d, -0.15d,    -0.2d}; //Fahrt voll zurück (hier minus -10) (m/s*s);
           V_a.Clear();
           foreach(double v in mj_V_a) {V_a.Add(v*(v>0?mod_v:mod_minusv));}
           v_A_delta1.Clear();
           foreach(double v in mj_v_A_delta1) {v_A_delta1.Add(v*mod_a_delta1);}
           v_A_stop.Clear();
           foreach(double v in mj_v_A_stop) {v_A_stop.Add(v*mod_a_stop);}
           v_A_back.Clear();
           foreach(double v in mj_v_A_back) {v_A_back.Add(v*mod_a_back);}
           
           //List<double> mj_FAHRSTUFE_vminusruder = new List<double>()  {-5d     , 0d,      7d,     11d,    15d};
           //List<double> mj_fahrstufe_VMINUSRUDER0 = new List<double>() {-0.0d*kn, 0d, 0.0d*kn, 0.0d*kn,0.0d*kn};
           //List<double> mj_fahrstufe_VMINUSRUDER5 = new List<double>() {-0.4d*kn, 0d, 0.2d*kn, 0.7d*kn,0.8d*kn};
           //List<double> mj_fahrstufe_VMINUSRUDER10 = new List<double>(){-1.4d*kn, 0d, 1.0d*kn, 2.7d*kn,3.0d*kn};
           //List<double> mj_fahrstufe_VMINUSRUDER15 = new List<double>(){-2.0d*kn, 0d, 1.7d*kn, 4.0d*kn,5.0d*kn};
           //List<double> mj_fahrstufe_VMINUSRUDER20 = new List<double>(){-2.5d*kn, 0d, 2.8d*kn, 4.9d*kn,7.0d*kn};
           //List<double> mj_fahrstufe_VMINUSRUDER25 = new List<double>(){-2.9d*kn, 0d, 3.3d*kn, 5.7d*kn,7.8d*kn};
           //List<double> mj_fahrstufe_VMINUSRUDER45 = new List<double>(){-3.5d*kn, 0d, 4.3d*kn, 6.9d*kn,8.5d*kn};
           FAHRSTUFE_vminusruder.Clear();
           foreach(double v in mj_FAHRSTUFE_vminusruder) {FAHRSTUFE_vminusruder.Add(v*(v>0?mod_v:mod_minusv));}
           fahrstufe_VMINUSRUDER0.Clear();
           foreach(double v in mj_fahrstufe_VMINUSRUDER0) {fahrstufe_VMINUSRUDER0.Add(v);}
           fahrstufe_VMINUSRUDER5.Clear();
           foreach(double v in mj_fahrstufe_VMINUSRUDER5) {fahrstufe_VMINUSRUDER5.Add(v*mod_vminusruder5);}
           fahrstufe_VMINUSRUDER10.Clear();
           foreach(double v in mj_fahrstufe_VMINUSRUDER10) {fahrstufe_VMINUSRUDER10.Add(v*mod_vminusruder10);}
           fahrstufe_VMINUSRUDER15.Clear();
           foreach(double v in mj_fahrstufe_VMINUSRUDER15) {fahrstufe_VMINUSRUDER15.Add(v*mod_vminusruder15);}
           fahrstufe_VMINUSRUDER20.Clear();
           foreach(double v in mj_fahrstufe_VMINUSRUDER20) {fahrstufe_VMINUSRUDER20.Add(v*mod_vminusruder20);}
           fahrstufe_VMINUSRUDER25.Clear();
           foreach(double v in mj_fahrstufe_VMINUSRUDER25) {fahrstufe_VMINUSRUDER25.Add(v*mod_vminusruder25);}
           fahrstufe_VMINUSRUDER45.Clear();
           foreach(double v in mj_fahrstufe_VMINUSRUDER45) {fahrstufe_VMINUSRUDER45.Add(v*mod_vminusruder45);}
           
           Liste_FAHRSTUFE_vminusruder.Clear();
           Liste_FAHRSTUFE_vminusruder.Add(FAHRSTUFE_vminusruder);
           Liste_FAHRSTUFE_vminusruder.Add(FAHRSTUFE_vminusruder);
           Liste_FAHRSTUFE_vminusruder.Add(FAHRSTUFE_vminusruder);
           Liste_FAHRSTUFE_vminusruder.Add(FAHRSTUFE_vminusruder);
           Liste_FAHRSTUFE_vminusruder.Add(FAHRSTUFE_vminusruder);
           Liste_FAHRSTUFE_vminusruder.Add(FAHRSTUFE_vminusruder);
           Liste_FAHRSTUFE_vminusruder.Add(FAHRSTUFE_vminusruder);

           Liste_fahrstufe_VMINUSRUDER.Clear();
           Liste_fahrstufe_VMINUSRUDER.Add(fahrstufe_VMINUSRUDER0);
           Liste_fahrstufe_VMINUSRUDER.Add(fahrstufe_VMINUSRUDER5);
           Liste_fahrstufe_VMINUSRUDER.Add(fahrstufe_VMINUSRUDER10);
           Liste_fahrstufe_VMINUSRUDER.Add(fahrstufe_VMINUSRUDER15);
           Liste_fahrstufe_VMINUSRUDER.Add(fahrstufe_VMINUSRUDER20);
           Liste_fahrstufe_VMINUSRUDER.Add(fahrstufe_VMINUSRUDER25);
           Liste_fahrstufe_VMINUSRUDER.Add(fahrstufe_VMINUSRUDER45);
       }
       
       
       

       public int index() //aus der Liste der AIglobals.AIglobal.Fahrzeuge, welcher index
       {
           for (int i = 0; i < AIglobal.Fahrzeuge.Count; i++)
           {
               if (AIglobal.Fahrzeuge[i] == this) return i;
           }
           AIglobal.Fehler("Indexfehler bei Fahrzeug");
           return -1;//Fehler
       }
      


     
       
       
       
       

       public double f_beschleunigung(double iFahrstufe,double iFdW,double iRuderlage) //Beschleunigung als Änderung der Geschwindigkeit, hängt ab von Fahrstufe(+Ruderlage?), Geschwindigkeit
       {
           double max_spd = fFahrstufe_MaxSpeed(iFahrstufe,iRuderlage);
           if ((iFahrstufe>0 && iFdW < max_spd) || (iFahrstufe<0 && Math.Abs(iFdW) < Math.Abs(max_spd) && iFdW<0)) // kann noch beschleunigen (hängt ab von Fahrstufe)
           {
               return AIMath.Wert_Ermitteln("i", iFahrstufe, FAHRSTUFE_a, fahrstufe_A);
           }

           if (iFahrstufe > 0 && iFdW > 0) //Verzögerung 1-n Fahrstufen >Dreisatz (hängt ab von Geschwindigkeit und Fahrstufe)
           {
               double a_1 = AIMath.Wert_Ermitteln("e", iFdW, V_a, v_A_delta1);     //entspricht Reduzierung um 1
               double a_stop = AIMath.Wert_Ermitteln("e", iFdW, V_a, v_A_stop);    //entspricht Reduzierung um imaginäre Fahrstufe (passend zu v)
               double imag_Fahrstufe_jetzt = AIMath.Wert_Ermitteln("e", iFdW, fahrstufe_VMAX, FAHRSTUFE_vmax); //rückwärts
               double imag_Fahrstufe_max = AIMath.Wert_Ermitteln("e", max_spd, fahrstufe_VMAX, FAHRSTUFE_vmax);
               double runter_um = imag_Fahrstufe_jetzt - imag_Fahrstufe_max;
               if (imag_Fahrstufe_jetzt == 1) return 0;
               return a_1 +(runter_um - 1)* (a_stop - a_1) / (imag_Fahrstufe_jetzt - 1);
           }

           if (iFahrstufe <= 0) //Verzögerung 1 Fahrstufe , Verzögerung n Fahrstufen >Dreisatz (hängt ab von Geschwindigkeit und Fahrstufe)
           {
               double a_stop = AIMath.Wert_Ermitteln("e", iFdW, V_a, v_A_stop);    //Fahrstufe auf 0 bei v=FdW
               double a_back = AIMath.Wert_Ermitteln("e", iFdW, V_a, v_A_back);    //Fahrstufe -10 0 bei v=FdW
               return a_stop + (iFahrstufe - 0)*(a_back - a_stop)  / (-10d - 0d);
           }

           AIglobal.Fehler();
           return -1;
       }


       
       //die ereichbare Geschwindigkeit verringert sich mit mit härterer Ruderlage
       public double fFahrstufe_MaxSpeed(double ifahrstufe, double iRuderlage) //Ruderlage
       {
           double fahrtreduktion = AIMath.Wert_Ermitteln2("i", ifahrstufe, Math.Abs(iRuderlage),
               new List<double>() {0, 5, 10, 15, 20, 25, 45}, Liste_FAHRSTUFE_vminusruder, Liste_fahrstufe_VMINUSRUDER);
           double fahrtmax = AIMath.Wert_Ermitteln("i", ifahrstufe, FAHRSTUFE_vmax, fahrstufe_VMAX);
           double result = fahrtreduktion > 0.5 * fahrtmax ? fahrtmax * 0.5d : fahrtmax - fahrtreduktion;
           return result;
       }
       
       //bei gegebener Geschwindigkeit, welche FAHRSTUFE liegt an
       public double f_FAHRSTUFE_speed(double iFdW) 
       {
           double imag_Fahrstufe_jetzt = AIMath.Wert_Ermitteln("e", iFdW, fahrstufe_VMAX, FAHRSTUFE_vmax); //rückwärts
           return imag_Fahrstufe_jetzt;
       }
       
       //maximale Winkelgeschwindigkeit, ermitteln durch Approximieren in EXCEL, mathematische Funktionen ohne physikalische Herleitung
       public double f_winkelgeschwindigkeit_max(double iRuderlage,double iFdW) //Winkelgeschwindigkeit, +=stb,,-=bb  als Fkt von Ruderlage, Geschwindigkeit
       {
           double absR = Math.Abs(iRuderlage);
           double w=Math.Sign(iRuderlage)*(Math.Pow(absR, 0.63)*0.089d*Math.Pow(Math.Abs((iFdW/kn)*(1-0.45*Math.Sin(absR*grad))),0.74));
           if (iFdW < 0) w = -w;
           return 1.1*w*modifier_winkelvmax;
       }

       public double f_winkelbeschleunigung(double iRuderlage,double iFdW) //Winkelgeschwindigkeit, +=stb,,-=bb  als Fkt von Ruderlage, Geschwindigkeit
       {
           //                                       ist positiv oder negativ abhängig von Vorzeichen von iRuderlage bzw iFdW
           double w_a = Math.Abs(iRuderlage)*f_winkelgeschwindigkeit_max(iRuderlage,iFdW)/(45d);
           return w_a*modifier_winkela;
       }


       // DNCs speichern mit Ruderlagen 0,5,10,...45 Grad, Fahrtstufe 0,1,2..,18, Kursänderung 0,5,10,...175                                              *                 *                               *
       double[,,] DNC_ruder_fahrtsstufe_kursaenderung=new double[10,19,37];
       
       public void DNCs_ermitteln()
       {
           for (int cntruder = 1; cntruder <= 9; cntruder++)
           {
               for (int ifahrstufe = 1; ifahrstufe <= 18; ifahrstufe++)
               {
                   andrehpunkt_ermitteln(cntruder, ifahrstufe, fFahrstufe_MaxSpeed(ifahrstufe, 0));
               }
           }
       }

       public double DNCs(double iRuderlage, double iFahrstufe, double Kursaenderung)   //-1 bedeutet nicht vernünftig möglich 
       {
           //Fahrtstufe muss>0 und Ruderlage>0
           if (iFahrstufe == 0 || Math.Abs(iRuderlage) < 5) return -1d;
           if (Math.Sign(iRuderlage)!=Math.Sign(Kursaenderung)) return -1d;
           int cntruderlage = Math.Abs((int) (iRuderlage / 5));
           int cntFahrstufe =Math.Abs((int) iFahrstufe);
           int cntKursaenderung = Math.Abs((int) (Kursaenderung / 5));
           double iDNC1 = DNC_ruder_fahrtsstufe_kursaenderung[cntruderlage,cntFahrstufe , cntKursaenderung];
           int cntKursaenderung2 = cntKursaenderung<35?cntKursaenderung + 1:cntKursaenderung;
           double iDNC2 = DNC_ruder_fahrtsstufe_kursaenderung[cntruderlage,cntFahrstufe , cntKursaenderung2];
           double iDNC = iDNC1 + ((iDNC2 - iDNC1) / 5) * (Math.Abs(Kursaenderung) - 5 * cntKursaenderung);
           return iDNC;
       }
       
       double[,] AUSFTOPPSTECKE_v_fahrtstufe=new double[19,11];
       public void AUFSTOPPSTECKEn_ermitteln()
       {
           for (int v = 1; v <= 18; v++)
           {
               for (int ifahrstufe = 0; ifahrstufe >= -10; ifahrstufe--)
               {
                   stoppstrecke_ermitteln(ifahrstufe, v);
               }
           }
       }
       public bool stoppstrecke_ermitteln(double iFahrstufe,double iFdW)
       {
           CShipPhysicalData tempSPD=new CShipPhysicalData(0d,0d,iFahrstufe,0d,iFdW,0d,0d);
           
           while (true)
           {
               sim_update(tempSPD);

               if (tempSPD.FdW <=0.1)
               {
                   AUSFTOPPSTECKE_v_fahrtstufe[(int) iFdW, (int) Math.Abs(iFahrstufe)] = tempSPD.z;
                   return true;
               }

               if (tempSPD.timestamp > 3600)
               {
                   AIglobal.Fehler("aufstoppen dauert übr 1h"+iFdW+","+iFahrstufe);
                   return false;  
               }
                   
           }
           return true;
       }
       
       public double Aufstoppstrecke(double iFahrstufe, double FdW)   //-1 bedeutet nicht vernünftig möglich 
       {
           //iFahrtstufe muss negativ
           double absFdW = Math.Abs(FdW);
           double absfahrstufe=Math.Abs(iFahrstufe);
           double strecke0,strecke1,FdW0, FdW1 ;

           if (absFdW >= AUSFTOPPSTECKE_v_fahrtstufe.GetUpperBound(0))
           {
               FdW0 = 17d; 
               FdW1 = 18d;      
           }
           else
           {
               FdW0 = (int) absFdW;     
               FdW1 = (int) absFdW + 1; 
           }
           strecke0=AUSFTOPPSTECKE_v_fahrtstufe[(int) FdW0, (int) absfahrstufe] ;
           strecke1=AUSFTOPPSTECKE_v_fahrtstufe[(int) FdW1, (int) absfahrstufe] ;
           double strecke = strecke0 + (strecke1 - strecke0) * (absFdW - FdW0) / (FdW1 - FdW0);
           
           return strecke;
       }
       
      
       
       
       
       
       
       
       
       
       
       
       
       
       
       //:::::::::::::::::::::::-Berechnet, was mit den byref zu übergebenen Schiffsparametern in einer Zeiteinheit passiert:::::::::::::::::::::::
       /*
        sim_update(CSPD SPD) //SPD als letztes Element einer anderen Berechnung
         aktualisiert SPD->SPD(t+delta) und speichert Kopie davon, also SPD(t+delta), in Track
         das SPD am Ende ist die gleiche Variable, aber mit Inhalt SPD(t+steps*delta)
         deshalb darf Eingangsvariable SPD kein Element eines Tracks sein

        fahre_Kurs_bis
         ruft konsekutiv sim_update auf
         das SPD am Ende ist die gleiche Variable, aber mit Inhalt SPD(t+steps*delta)

        Manoever
         ruft fahre_Kurs_bis... auf
         das SPD am Ende ist die gleiche Variable, aber mit Inhalt SPD(t+steps*delta)
        */
       
        public void sim_update(CShipPhysicalData SPD, CTrack iTrack=null)
       {
           bool bnachschwenken = false;
           
           SPD.timestamp  += AIConst.delta_t;
           SPD.x +=  SPD.FdW * Math.Sin(SPD.KdW * grad)*AIConst.delta_t; //lon wird durch setter gestzt
           SPD.z +=  SPD.FdW * Math.Cos(SPD.KdW * grad)*AIConst.delta_t; //lat wird durch setter gesetzt 
           
           SPD.FdW =SPD.FdW+ f_beschleunigung(SPD.Fahrstufe, SPD.FdW, SPD.Ruderlage)*AIConst.delta_t;
          
           if (SPD.FdW==double.NaN)
           {
           }
           if (SPD.Ruderlage != 0 || istManuell)
           {
               SPD.KdW = AIMath.norm(SPD.KdW + SPD.Winkelv * AIConst.delta_t,360);
               double winkela= f_winkelbeschleunigung(SPD.Ruderlage,  SPD.FdW);
               double w_max =f_winkelgeschwindigkeit_max(SPD.Ruderlage, SPD.FdW);
               
               if (istManuell)
               {
                   if ((SPD.Winkelv > 0 && SPD.Winkelv > w_max) ||  (SPD.Winkelv < 0 && SPD.Winkelv < w_max))
                   {
                       bnachschwenken = true;
                       if (Math.Abs(SPD.Winkelv - w_max) > 0.02)
                       {
                           double neu_Winkelv=0.8d*SPD.Winkelv+ 0.2* w_max;
                           if (Math.Sign(neu_Winkelv) == Math.Sign(SPD.Winkelv))
                           {
                               SPD.Winkelv = neu_Winkelv;
                           }
                           else
                           {
                               SPD.Winkelv = 0;
                           }
                       }
                       else
                       {
                             SPD.Winkelv = w_max;
                       }
                   }
                   
               }

               if (!bnachschwenken)
               {
                   SPD.Winkelv += winkela*AIConst.delta_t;
                   if (Math.Abs(SPD.Winkelv) > Math.Abs(w_max)) SPD.Winkelv = w_max;
               }
           }
           else
           {
               SPD.Winkelv = 0d;
           }
           
           iTrack?.ListeSPD.Add(SPD.Copy());

       }
       
       

       //i_aufTrack: index, ab dem Fzg ERSTMALS auf Track gebracht wurde
       public int verfolge_Plan(CPlan iPlan,CShipPhysicalData SPD,ref double t_aufTrack, double zulaufwinkel,CTrack iTrack=null)
       {
           int i_auftrack = -1;
           if (istManuell)
           {
               ES_simulate_track_update();
               return DarstellungTrack.ListeSPD.Count-1;
           }
           
           
           t_aufTrack = -1;
          
           double wartezeit = 0;
           bool mussaufstoppen = false;
           
           while (iPlan.counter < iPlan.ListeWegpunkt.Count)
           {
               //CWegpunkt ggwWP    = iPlan.ggw_WP_Start();
               //CWegpunkt folgWP   = iPlan.ggw_WP_Ziel();
               if (iPlan.ggw_WP_Ziel() == null) return i_auftrack;   //Ende der Wegstrecke
               
               double ggwKurs = AIMath.Absolut_Peilung_Punkt(iPlan.ggw_WP_Start().x,iPlan.ggw_WP_Start().z,iPlan.ggw_WP_Ziel().x,iPlan.ggw_WP_Ziel().z);
               int i = iPlan.next_wegpunkt_counter(iPlan.counter);
               
               double folgKurs = (i==-1)?double.NaN: iPlan.Kurs(i);
               if (double.IsNaN(folgKurs))
               {
               }

               wartezeit = iPlan.ListeWegpunkt[iPlan.counter].auftrag_wartezeit;
               if (wartezeit > 0)
               {
                   
               }
               if (iPlan.warte_bis == -1) 
                   iPlan.warte_bis = SPD.timestamp + wartezeit;
               if (wartezeit > 0 && iPlan.warte_bis>SPD.timestamp)
               {
                   Manoever_warte(wartezeit,SPD,iTrack);
               }
               
               
               SPD.Fahrstufe = f_FAHRSTUFE_speed((double) iPlan.ggw_WP_Start().auftrag_geschwindigkeit);
               //Abweichung von Kursvektor? Bringe auf Kursvektor
               if (Math.Abs(AIMath.Diff_Winkel(SPD.KdW, ggwKurs)) > 2 || AIMath.Abstand_Punkt_Gerade(SPD.x, SPD.z, iPlan.ggw_WP_Start().x, iPlan.ggw_WP_Start().z, ggwKurs) > 20d)
               {
                   
                   int i_auftrack0=bringe_auf_Kursvektor(iPlan,SPD, zulaufwinkel, iTrack);
                   if (i_auftrack < 0) i_auftrack = i_auftrack0; 
                   t_aufTrack = SPD.timestamp;
                   
                   if (i_auftrack == -1)
                   {
                       
                       //goto naechster_Wegpunkt;
                   }
                   else
                   {
                       if (i_auftrack == -1) t_aufTrack = SPD.timestamp; //t;
                   }
               }
               else
               {
                   SPD.KdW = ggwKurs;
                   t_aufTrack=SPD.timestamp;
               }
           
               
               switch (iPlan.ggw_WP_Start().auftrag)
               {
                   case AIConst.cAuftrag_Fahre_Geschw:
                        SPD.Fahrstufe = f_FAHRSTUFE_speed((double) iPlan.ggw_WP_Start().auftrag_geschwindigkeit);
                        break;
                   //case AIConst.cAuftrag_Warte_Zeit:
                   //    fahre_Kurs_bis(cBedingung_Warten, ggwWP.auftrag_wartezeit,-1, -1, -1, -1, -1, -1,null, SPD,ref t, delta_t,iTrack);
                   //    goto naechster_Wegpunkt;
                   //    break;
               }
               
               //Fahren bis DNC nächster Wegpunkt
               //Phase 3 geradeaus bis <=DNC
               SPD.Ruderlage = 0;
               SPD.Winkelv = 0;
               if (!iPlan.muss_am_Ende_aufstoppen(iPlan.counter))
               {
                   fahre_Kurs_bis(AIConst.cBedingung_Andrehpunkt, -1,-1, 20, folgKurs, iPlan.ggw_WP_Ziel().x, iPlan.ggw_WP_Ziel().z, -1,null, SPD,iTrack);
               }
               else
               {
                   fahre_Kurs_bis(AIConst.cBedingung_aufstoppen, -1,-1, -1, folgKurs, iPlan.ggw_WP_Ziel().x, iPlan.ggw_WP_Ziel().z, -1,null, SPD,iTrack);
               }
              
               
             
               //Phase 4 Gegenlenken
               //Andrehen durch bringe auf Kursvektor
               //-siehe Beginn while-Schleife
               
              naechster_Wegpunkt:
              iPlan.counter = iPlan.next_wegpunkt_counter(iPlan.counter);
              iPlan.warte_bis = -1; //muss neu gesetzt werden
              
              if (iPlan.counter<0 || iPlan.counter>=iPlan.ListeWegpunkt.Count) 
                   return i_auftrack;

           }
           return i_auftrack;
       }
       
       
       
       //return index: geht nicht, nächster Wegpunkt bitte
       //public int bringe_auf_Kursvektor(CPlan iPlan,ref double ilat,ref double ilon,ref double iFahrstufe,ref double cFdW,ref double iRuderlage,ref double kurs,ref double w,ref double t, double delta_t)
       public int bringe_auf_Kursvektor(CPlan iPlan, CShipPhysicalData SPD,double zulaufwinkel, CTrack iTrack=null)
       {
           
           neuer_anlauf:
           if (SPD.Fahrstufe == 0) SPD.Fahrstufe = 10;
           
           CWegpunkt ggwWP    = iPlan.ggw_WP_Start();
           CWegpunkt folgWP   = iPlan.ggw_WP_Ziel();
           double ggwKurs = AIMath.Absolut_Peilung_Punkt(ggwWP.x,ggwWP.z,folgWP.x,folgWP.z);                        
          
           double diff_kurs = 0;
           double peil = 0;
           double neuerkurs=0;
           double dist = 0;

           neuer_Kursvektor:
           diff_kurs = 0;peil = 0;neuerkurs=0;dist = 0;
           
           
           diff_kurs =AIMath.Diff_Winkel(SPD.KdW, ggwKurs); //<0 bb,>0 stb
           peil = AIMath.Relativpeilung_Peilung_Punkt(SPD.x,SPD.z,SPD.KdW,folgWP.x,folgWP.z);
           
           //Phase 1 Parallellkurs zu Kursvektor erreichen
           if (peil>0 && diff_kurs>0 || (peil<0 && diff_kurs<0)) //Ziel Stb und ich muß noch stb steuern oder Ziel Bb und ich muß noch Bb steuern
           {
               Manoever_Ruderlage_drehe_auf_Kurs(20,ggwKurs, SPD,  iTrack);
           }
           
           
           //Phase 2 Parallellkurs oder besser erreicht, auf neuen Kurs abhängig von Peilung und max 30 Grad zu Kurs gehen
           peil = AIMath.Relativpeilung_Peilung_Punkt(SPD.x,SPD.z,SPD.KdW,folgWP.x,folgWP.z);
           if (Math.Abs(peil) > zulaufwinkel*0.75d)             //An Wegpunkt vorbeigefahren?
           {
               //gehe direkt auf nächsten Wegpunkt
               //ggwKurs = AIMath.Absolut_Peilung_Punkt(SPD.x,SPD.z,folgWP.x,folgWP.z);
               //ggwWP = new CWegpunkt(SPD.lat,SPD.lon,0,SPD.FdW,0,0);
               //
               //goto neuer_Kursvektor;
               //eine ALternative
               //Manoever_drehe_auf_Punkt(folgWP.x, folgWP.z, 15, SPD, ref t, delta_t, iTrack);
               //andere Alternative
               int bestleg= Plan.bester_leg_von_punkt(SPD.x, SPD.z);
               if (bestleg == Plan.counter)
               {
                   Manoever_drehe_auf_Punkt(folgWP.x, folgWP.z, std_ruderlage, SPD, iTrack);
                   //dreht natürlich nicht wirklich auf SPD.KdW+175, geht nur nah ran ans Ziel, um das Ende des Manövers zu haben

                   if (!Plan.muss_am_Ende_aufstoppen(Plan.counter))
                   {
                       fahre_Kurs_bis(AIConst.cBedingung_Andrehpunkt, -1,-1, 20, /*AIMath.norm(SPD.KdW+175,360)*/ Plan.folgeKurs(), iPlan.ggw_WP_Ziel().x, iPlan.ggw_WP_Ziel().z, -1,null, SPD,iTrack);
                   }
                   else
                   {
                       fahre_Kurs_bis(AIConst.cBedingung_aufstoppen, -1,-1, -1, -1, iPlan.ggw_WP_Ziel().x, iPlan.ggw_WP_Ziel().z, -1,null, SPD,iTrack);
                   }
               }
               else
               {
                   Plan.counter = bestleg;
                   goto neuer_anlauf;
               }
               
               return iTrack.ListeSPD.Count-1;
           }
           
           neuerkurs = AIMath.norm(ggwKurs + (peil > 0 ? zulaufwinkel : -zulaufwinkel),360);
          
           SPD.Ruderlage = (peil > 0 ? std_ruderlage : -std_ruderlage);
           fahre_Kurs_bis(AIConst.cBedingung_Andrehpunkt+AIConst.cBedingung_Zielkurserreicht,-1, -1, SPD.Ruderlage, ggwKurs, folgWP.x, folgWP.z,neuerkurs, null, SPD,iTrack);

           peil = AIMath.Relativpeilung_Peilung_Punkt(SPD.x, SPD.z, SPD.KdW, folgWP.x, folgWP.z);
           if (Math.Sign(peil) == Math.Sign(SPD.Ruderlage))   //Nach dem eindrehen ist das Ende des anvisierten Legs mit 
           {                                                  //dem 30 GRad Kurs nicht erreichrbar
               Manoever_drehe_auf_Punkt(folgWP.x, folgWP.z, SPD.Ruderlage, SPD,  iTrack);
           }
           
           //Phase 3 geradeaus bis <=DNC
           SPD.Ruderlage = 0;
           SPD.Fahrstufe = 7;
           SPD.Winkelv = 0;
           
           fahre_Kurs_bis(AIConst.cBedingung_Andrehpunkt, -1,-1, std_ruderlage, ggwKurs, ggwWP.x, ggwWP.z,-1, null, SPD, iTrack);
           
           //Phase 4 Gegenlenken
           peil = AIMath.Relativpeilung_Peilung_Punkt(SPD.x,SPD.z,SPD.KdW,folgWP.x,folgWP.z);
           if (Math.Abs(peil) > 30)             //An Wegpunkt vorbeigefahren?
           {
               //An Wegpunkt vorbeigefahren? -> dann nächster Wegpunkt, gehe zu (1)
           }
           SPD.Fahrstufe = f_FAHRSTUFE_speed(SPD.FdW);
           
           Manoever_Ruderlage_drehe_auf_Kurs(std_ruderlage,ggwKurs, SPD, iTrack);
           
           return iTrack.ListeSPD.Count-1;
       }

       
       
       public bool Manoever_drehe_auf_Kurs(double neuerKurs,CShipPhysicalData SPD,CTrack iTrack=null)
       {
           double diff_kurs =AIMath.Diff_Winkel(SPD.KdW, neuerKurs); //<0 bb,>0 stb
           if (diff_kurs != 0d) 
            fahre_Kurs_bis(AIConst.cBedingung_Zielkurserreicht, -1,-1, -1, -1, -1, -1,neuerKurs, null, SPD,iTrack);
           
           SPD.Ruderlage = 0;
           SPD.Winkelv = 0;
           
           return true;
       }
       
       public bool Manoever_Ruderlage_drehe_auf_Kurs(double absruderlage,double neuerKurs,CShipPhysicalData SPD,CTrack iTrack=null)
       {
           
           double diff_kurs =AIMath.Diff_Winkel(SPD.KdW, neuerKurs); //<0 bb,>0 stb
           if (diff_kurs != 0d) 
           {
               SPD.Ruderlage=(diff_kurs>0)?absruderlage:-absruderlage; //Stb oder Bb
               fahre_Kurs_bis(AIConst.cBedingung_Zielkurserreicht, -1,-1, -1, -1, -1, -1,neuerKurs, null, SPD, iTrack);
           }
           
           SPD.Ruderlage = 0;
           SPD.Winkelv = 0;
           
           return true;
       }
       
       
       public bool Manoever_drehe_auf_Punkt(double x,double z,double absruderlage, CShipPhysicalData SPD,CTrack iTrack=null)
       {
           double neuerKursgeschaetzt= AIMath.Absolut_Peilung_Punkt(SPD.x, SPD.z, x, z);
           double diff_kurs =AIMath.Diff_Winkel(SPD.KdW, neuerKursgeschaetzt); //<0 bb,>0 stb
           SPD.Ruderlage=(diff_kurs>0)?absruderlage:-absruderlage; //Stb oder Bb
           
           fahre_Kurs_bis(AIConst.cBedingung_aufPunktzu, -1,-1, -1, -1, x, z,-1, null, SPD,iTrack);
           SPD.Ruderlage = 0;
           SPD.Winkelv = 0;
           return true;
       }
       
       public bool Manoever_peile_Gegner_min(double minwinkel,CFzg Gegner,int index_Gegnertrack, CShipPhysicalData SPD,CTrack iTrack=null)
       {
           fahre_Kurs_bis(AIConst.cBedingung_peile_Gegner_min, -1,-1, -1, -1, -1, -1,minwinkel, Gegner, SPD,iTrack,index_Gegnertrack);
           return true;
       }
       
       public bool Manoever_peile_Gegner_achtern(double minwinkel,CFzg Gegner,int index_Gegnertrack, CShipPhysicalData SPD,CTrack iTrack=null)
       {
           fahre_Kurs_bis(AIConst.cBedingung_peile_Gegner_achtern, -1,-1, -1, -1, -1, -1,minwinkel, Gegner, SPD,iTrack,index_Gegnertrack);
           return true;
       }
       
       public bool Manoever_peile_Gegner_voraus(double minwinkel,CFzg Gegner,int index_Gegnertrack, CShipPhysicalData SPD,CTrack iTrack=null)
       {
           fahre_Kurs_bis(AIConst.cBedingung_peile_Gegner_voraus, -1,-1, -1, -1, -1, -1,minwinkel, Gegner, SPD,iTrack,index_Gegnertrack);
           return true;
       }
       
       public bool Manoever_am_Gegner_vorbei(double mindestdistanz,CFzg Gegner,int index_Gegnertrack, CShipPhysicalData SPD,CTrack iTrack=null)
       {
           fahre_Kurs_bis(AIConst.cBedingung_amGegnervorbei, -1,mindestdistanz, -1, -1, -1, -1,-1, Gegner, SPD,iTrack,index_Gegnertrack);
           return true;
       }
       
       
       // fahre_Kurs_bis(cBedingung_Warten, ggwWP.auftrag_wartezeit,-1, -1, -1, -1, -1, -1,null, SPD,ref t, delta_t,iTrack);  
       public bool Manoever_warte(double wartezeit, CShipPhysicalData SPD,CTrack iTrack=null)
       {
           fahre_Kurs_bis(AIConst.cBedingung_Warten, wartezeit,-1, -1, -1, -1, -1,-1, null, SPD, iTrack);
           return true;
       }
       
       //fahre_Kurs_bis(cBedingung_Zeit,zeit,-1,-1,-1,-1,-1,-1,null,nSPD, ref nt, ndelta_t, nTrack);
       public bool Manoever_Fahre_weiter_bis_Ende(CShipPhysicalData SPD, CTrack iTrack=null)
       {
           fahre_Kurs_bis(AIConst.cBedingung_Zeit,-999,-1,-1,-1,-1,-1,-1,null,SPD,  iTrack);
           return true;
       }
       
       public bool Manoever_Fahre_weiter_Zeit(double zeit,CShipPhysicalData SPD,CTrack iTrack=null)
       {
           fahre_Kurs_bis(AIConst.cBedingung_Zeit,zeit,-1,-1,-1,-1,-1,-1,null,SPD, iTrack);
           return true;
       }
       
       public bool Manoever_Fahre_weiter_Distanz(double distanz,CShipPhysicalData SPD,CTrack iTrack=null)
       {
           fahre_Kurs_bis(AIConst.cBedingung_Distanzgefahren,-1,distanz,-1,-1,-1,-1,-1,null,SPD, iTrack);
           return true;
       }
       //Fahre Kurs bis:
       //cBedingung_Andrehpunkt=1;        //1.   nkruderlage,double nk,double nkx2, double nkz2
       //cBedingung_amGegnervorbei=2;     //2.   GEGNER achtern und Abstand>DISTANZ
       //cBedingung_Distanzgefahren=3;    //3.   DISTANZ zurückgelegt
       //cBedingung_AbstandGegnerkurs=4;  //4.   DISTANZ von GEGNER.Kurs
       //cBedingung_AbstandGegnerkurs=8   //5.
       //cBedingung_Zielkurserreicht=16   //6.   double zk
       //cBedingung_Warten=32             //
       
       //Das ist die komplizierte Routine
       // bedingung:cBedingung_Andrehpunkt,cBedingung_amGegnervorbei,cBedingung_Distanzgefahren,cBedingung_AbstandGegnerkurs,,cBedingung_Zielkurserreicht
       //
       // nk: nächster Kurs 
       // (nkx2, nkz2) Punkt auf nächstem Kursabschnitt
       //
       // zk: Zielkurs
       // Gegner: Falls Bedingung mit Gegner verknüpft
       // SPD: gegenwärtige Schiffsdaten
       
       public bool   fahre_Kurs_bis(int bedingung, double zeit, double distanz, 
                                  double nkruderlage,double nk,double nkx2, double nkz2,double zk,  CFzg Gegner,
                                  CShipPhysicalData SPD, CTrack iTrack=null,int index_Gegnertrack=0)
       {
           //Falls SPD noch nicht gespeichert, dann abspeichern
           if (iTrack != null)
           {
               if (iTrack.ListeSPD.Count==0 || (iTrack.ListeSPD.Count>0 && iTrack.gesamtzeit()<SPD.timestamp)) 
                   iTrack.ListeSPD.Add(SPD.Copy());
           }
          
           if (!istManuell) SPD.Winkelv = 0; //wird einmalig auf 0 gesetzt, muss aber je nach Ruderlage nicht null bleiben
           int index_G = iTrack.ListeSPD.Count - 1; //index_Gegnertrack;
           double dnc = 0;
           
           double olddistS = 999999999d;//vorheriger Abstand vom Schnittpunkt
           double oldDistG = 999999999d;//vorheriger Abstand vom gegner
           double distD = 0;
           double oldx = SPD.x;
           double oldz = SPD.z;
           double timestart = SPD.timestamp;
           double t= SPD.timestamp;
           bool bZielerreicht = false;
           double old_KdW = SPD.KdW;
           double old_peil=AIMath.Relativpeilung_Peilung_Punkt(SPD.x, SPD.z, SPD.KdW,nkx2, nkz2);
           
           double dist=0, dx=0, dz=0;
           double dmin_x=0, dmin_z=0, dmin=-1, dmin_richt= 0;
           double richtung = 0;
           
           //check, ob er in eine Area reindonnert
           if (SPD.Ruderlage==0d)
           {
               foreach (CArea Area in AIglobal.Gebiete)
               {
                   if (Area.typ == AIConst.cAreaTyp_NOGO)
                   {
                       if (Area.faehrt_rein(SPD.x, SPD.z, SPD.KdW, ref dist, ref dx, ref dz, ref richtung))
                       {
                           if (dist < dmin || dmin<0)
                           {
                               dmin = dist;
                               dmin_x=dx;dmin_z = dz;
                               dmin_richt = richtung;
                           }
                       }
                   }
               }
           } 
           
           
           
           
           
           do
           {
               old_KdW = SPD.KdW;
               old_peil=AIMath.Relativpeilung_Peilung_Punkt(SPD.x, SPD.z, SPD.KdW,nkx2, nkz2);
               
               sim_update(SPD,iTrack); //EIN ZEITSCHRITT
               
               if ((index_G % AIConst.check_steps) == 0 && dmin>0)
               {
                   dist= AIMath.Dist_XZ(SPD.x,SPD.z,dmin_x,dmin_z);
                   if (dist < 0.1 * sm) return false;
               }
               
               index_G++; //Gegner Track auch ein Punkt weiter, ??? besser wäre wie in simulate_track_update
               
               t=SPD.timestamp;
               
               if (t >= AIglobal.sim_duration) 
               {   
                   AIglobal.Fehler("Fehler: fahre_distanz_auf_Kurs");
                   return false;
               }

               
               
               //fährt bis Andrehpunkt DNC auf Kurs nk auf deren Linie (nkx2,nkz2) liegt
               if ((bedingung & AIConst.cBedingung_Andrehpunkt)==AIConst.cBedingung_Andrehpunkt)
               {
                   double sx = nkx2, sz = nkz2, distS, peil, diff_kurs = 0;
                   
                   if (!double.IsNaN(nk))
                   {
                       //muss sx, sz jeden zeitschritt neu ermittelt werden ???
                       AIMath.Schnitt_Gerade1_Gerade2(SPD.x, SPD.z, SPD.KdW, nkx2, nkz2, nk, ref sx, ref sz);
                       //false: kein Schnittpunkt, Winkel gleich
                       diff_kurs = AIMath.Diff_Winkel(SPD.KdW, nk);
                   }

                   distS = AIMath.Dist_XZ(SPD.x, SPD.z, sx, sz); //Abstand Position zum virtuellen Schnittpunkt Kurs-Kursvektor
                   peil = AIMath.Relativpeilung_Peilung_Punkt(SPD.x, SPD.z, SPD.KdW, sx, sz);
                   dnc = DNCs(Math.Abs(nkruderlage) , f_FAHRSTUFE_speed(SPD.FdW), Math.Abs(diff_kurs));
                   
                   if (dnc < 0)                     //über das Schnittpunkt hinausgeschossen
                   {
                       if (distS>olddistS) 
                           bZielerreicht = true;
                   }
                   
                   if (Math.Abs(peil) < 90 && dnc>0)  //zwischen Schnittpunkt und dnc
                   {
                       if (distS <= dnc)
                       {
                           //Linie(ilat, ilon, sz / lat_to_m, sx / lon_to_m, 1.3, Color.blue);
                           bZielerreicht = true;
                       }
                   }
                   else
                   {
                       if (distS <= dnc && dnc>0)   //DNCs(nkruderlage, f_FAHRSTUFE_speed(SPD.FdW), diff_kurs))
                       {
                           bZielerreicht = true;
                       }
                       else
                       {   //Entfernt sich wieder vom Punkt und fährt geradeaus
                           if (distS > olddistS && SPD.Ruderlage==0) //nk==double.NaN bedeutet kein Folgekurs, Fahrzeug soll abbremsen
                           {
                               bZielerreicht = true; //heisst über das Ziel hinausgeschossen
                           }
                       }
                   }

                   olddistS = distS;
               }
               
               //fährt bis Andrehpunkt DNC auf Kurs nk auf deren Linie (nkx2,nkz2) liegt
               if ((bedingung & AIConst.cBedingung_aufstoppen)==AIConst.cBedingung_aufstoppen)
               {
                   double sx = nkx2, sz = nkz2, distS, peil;
                   
                   distS = AIMath.Dist_XZ(SPD.x, SPD.z, nkx2, nkz2); //Abstand Position zum virtuellen Schnittpunkt Kurs-Kursvektor
                  
                   double stoppstrecke = Aufstoppstrecke(-5d, SPD.FdW);

                   if (distS <= stoppstrecke || olddistS < distS)
                   {
                       SPD.Fahrstufe = -5;
                       if (iTrack != null) iTrack.ListeSPD[^1].Fahrstufe = -5;
                   }

                   if (SPD.FdW < 0.1 && SPD.Fahrstufe == -5)
                   {
                       SPD.FdW = 0d;
                       if (iTrack != null) iTrack.ListeSPD[^1].FdW = 0d;
                       SPD.Fahrstufe = 0d;
                       if (iTrack != null) iTrack.ListeSPD[^1].Fahrstufe = 0d;
                       bZielerreicht = true;
                   }
                  
                   olddistS = distS;
               }
               
               
               
               
               
               
               if ((bedingung & AIConst.cBedingung_aufPunktzu) == AIConst.cBedingung_aufPunktzu)
               {
                   double peil = AIMath.Relativpeilung_Peilung_Punkt(SPD.x, SPD.z, SPD.KdW,nkx2, nkz2);
                   
                   if (AIMath.Winkel_durchlaufen(0, old_peil, peil))
                   {
                       zk=AIMath.Absolut_Peilung_Punkt(SPD.x, SPD.z, nkx2, nkz2);
                       SPD.KdW = zk;
                       SPD.Ruderlage = 0d;
                       SPD.Winkelv = 0d;
                       if (iTrack != null) iTrack.ListeSPD[^1].KdW = zk;
                       if (iTrack != null) iTrack.ListeSPD[^1].Ruderlage = 0;
                       if (iTrack != null) iTrack.ListeSPD[^1].Winkelv = 0;
                       bZielerreicht = true;
                   }
               }
               
               
               //die Distanz zum Gegner wächst wieder
               if ((bedingung & AIConst.cBedingung_amGegnervorbei) == AIConst.cBedingung_amGegnervorbei)
               {
                   CShipPhysicalData gSPD= Gegner.Track.SPD(index_G);
                   double distG=AIMath.Dist_XZ(SPD.x, SPD.z, gSPD.x, gSPD.z);
                   if (distG>distanz && distG>oldDistG) bZielerreicht=true;
                   oldDistG = distG;
               }
               
               //cBedingung_peile_Gegner_mindestens in - für backbord
               if ((bedingung & AIConst.cBedingung_peile_Gegner_min) == AIConst.cBedingung_peile_Gegner_min)
               {
                   CShipPhysicalData gSPD= Gegner.Track.SPD(index_G);
                  
                   double peile = AIMath.Relativpeilung_Peilung_Punkt(SPD.x, SPD.z, SPD.KdW, gSPD.x, gSPD.z);
                   
                   if (Math.Sign(peile) == Math.Sign(zk) && Math.Abs(peile) > Math.Abs(zk)) bZielerreicht = true;
               }
               
               
               //cBedingung_peile_Gegner_mindestens in - für backbord
               if ((bedingung & AIConst.cBedingung_peile_Gegner_achtern) == AIConst.cBedingung_peile_Gegner_achtern)
               {
                   CShipPhysicalData gSPD= Gegner.Track.SPD(index_G);
                  
                   double peile = AIMath.Relativpeilung_Peilung_Punkt(SPD.x, SPD.z, SPD.KdW, gSPD.x, gSPD.z);
                   
                   if (Math.Abs(peile) > Math.Abs(zk)) bZielerreicht = true;
               }
               
               
               //cBedingung_peile_Gegnerhöchstemns in - für backbord
               if ((bedingung & AIConst.cBedingung_peile_Gegner_voraus) == AIConst.cBedingung_peile_Gegner_voraus)
               {
                   CShipPhysicalData gSPD= Gegner.Track.SPD(index_G);
                  
                   double peile = AIMath.Relativpeilung_Peilung_Punkt(SPD.x, SPD.z, SPD.KdW, gSPD.x, gSPD.z);
                   
                   if (Math.Abs(peile) < Math.Abs(zk)) bZielerreicht = true;
               }
               
               
               
               
               //muss eine Distanz zurücklegen 
               if ((bedingung & AIConst.cBedingung_Distanzgefahren) == AIConst.cBedingung_Distanzgefahren)
               {
                   distD=distD+AIMath.Dist_XZ(SPD.x, SPD.z, oldx, oldz);
                   oldx = SPD.x; oldz = SPD.z;
                   if (distD>=distanz) bZielerreicht=true;
               }
               
               //cBedingung_Warten
               //muss eine Zeit fahren/warten
               if ((bedingung & AIConst.cBedingung_Warten) == AIConst.cBedingung_Warten)
               {
                   if (SPD.FdW > 0) SPD.Fahrstufe = -5; else SPD.Fahrstufe = 0;
                   if (t - timestart > zeit) bZielerreicht=true;
               }
               
               if ((bedingung & AIConst.cBedingung_Zeit) == AIConst.cBedingung_Zeit)
               {
                   if (t - timestart > zeit && zeit>=0) bZielerreicht=true;
               }
               
               //dreht bis Zielkurs erreicht    Toleranz wird über distanz mitgegeben, default -1 bedeutet Math.Abs(dist)=1
               if ((bedingung & AIConst.cBedingung_Zielkurserreicht) == AIConst.cBedingung_Zielkurserreicht)
               {
                   if (AIMath.Winkel_durchlaufen(zk, old_KdW, SPD.KdW) || Math.Abs(AIMath.Diff_Winkel(SPD.KdW, zk)) <=Math.Abs(dist))
                   {
                       SPD.KdW = zk; //fixiere Zielkurs, weil sonst ungenau
                       if (iTrack != null) iTrack.ListeSPD[^1].KdW = zk;
                       bZielerreicht = true;
                   }
                   
               }
               
               
               //cBedingung_aufstoppen_bis_Gegner_Distanz
               if ((bedingung & AIConst.cBedingung_aufstoppen_bis_Gegner_Distanz) == AIConst.cBedingung_aufstoppen_bis_Gegner_Distanz)
               {
                   if (SPD.FdW <= 0)
                   {
                       SPD.FdW = 0;
                       SPD.Fahrstufe = 0;
                   }
                   else
                   {
                       SPD.Fahrstufe = -5;
                   }

                   bool bsicher = false;
                   //foreach (CFzg aGegner in AIglobal.Fahrzeuge)
                   //{
                       CShipPhysicalData gSPD= Gegner.Track.SPD(index_G);
                       if (AIMath.Dist_XZ(SPD.x, SPD.z, gSPD.x, gSPD.z) > distanz) bZielerreicht =true;
                   //}
               }
               
           } while (bZielerreicht==false);
          
           return true;
       }
       
       public double andrehpunkt_ermitteln2(double iRuderlage, double iFahrstufe,double iFdW, double Kursaenderung)
       {
           if (Kursaenderung == 0) return 0d;
           if (Kursaenderung > 175) return -1;
           double t = 0;
           double delta_t = 0.1;
           
           CShipPhysicalData SPD = new CShipPhysicalData(0, 0, iFahrstufe, iRuderlage, iFdW, 0, 0);
           
           Manoever_drehe_auf_Kurs(Kursaenderung,SPD);

           double z1 = SPD.z - SPD.x / Math.Tan(Kursaenderung * grad);
           return z1;
       }
       
       // ermittelt Andrehpunkte für Kursänderungen 1,5,10,15,...175 Grad
       public bool andrehpunkt_ermitteln(int cntruder, double iFahrstufe,double iFdW)
       {
           double iRuderlage = cntruder * 5d;
           double t = 0;
           
           double z1 = 0;
           int cntkursaenderung = 0;
           double neuerkurs = 0;
           CShipPhysicalData tempSPD=new CShipPhysicalData(0d,0d,iFahrstufe,cntruder * 5d,iFdW,0d,0d);
           
           while (cntkursaenderung <= 35)
           {
               sim_update(tempSPD);

               if (tempSPD.KdW >= neuerkurs)
               {
                   z1 =(neuerkurs==0)?0: tempSPD.z - tempSPD.x / Math.Tan(neuerkurs * grad);
                   
                   DNC_ruder_fahrtsstufe_kursaenderung[cntruder,(int) iFahrstufe, cntkursaenderung] = z1;
                   cntkursaenderung++;
                   neuerkurs = cntkursaenderung * 5d;
               }

               if (t > 3600)
               {
                   AIglobal.Fehler("dauert übr 1h"+iRuderlage+","+iFahrstufe+","+neuerkurs);
                   return false;  
               }
                   
           }
           return true;
       }
       
       public void Setze_auf_Karte()
       {
           if (istManuell)
           {
               ObjSC = AIglobal.m_ObjSpawnerSO.SpawnObjectLatLon(NauticType.MineSweeper, new double2(firstSPD.lat,firstSPD.lon), Vector3.zero);
               ResourceManager.GetInterface<ObjectsInterface>().SelectNauticObject(ObjSC);
           }
           else
           {
               ObjSC = AIglobal.m_ObjSpawnerSO.SpawnObjectLatLon(NauticType.Cargo, new double2(firstSPD.lat,firstSPD.lon), Vector3.zero);
           }
           //ObjSC = AIglobal.m_ObjSpawnerSO.SpawnObjectLatLon(EcdisType.npc, new double2(firstSPD.lat,firstSPD.lon), Vector3.zero);
           ObjSC.SetAI(this);
           ObjSC.name = name;
           ObjSC.Data.ObjectName = name;
           ObjSC.Data.m_Direction = (float)firstSPD.KdW;
           //ObjSC.Data.m_Size
           /*ObjSC.SetDirVelPos(
               new Vector3((float) Math.Sin(firstSPD.KdW * grad), 0f, (float) Math.Cos(firstSPD.KdW * grad)),
               new Vector3(0f, 0f, 0f),
               ObjSC.));
           */
       }

       public CArea in_Area(int areatyp, double lat, double lon)
       {
           foreach (CArea A in AIglobal.Gebiete)
           {
               if (A.typ == areatyp && A.Pos_enthalten(lat, lon))
               {
                   return A;
               }
           }
           return null;
       }
       
       public double kritische_Entf(CShipPhysicalData mSPD, CFzg aFzg, CShipPhysicalData aSPD, int typOrtung, ref string msg)
       {

           double tmp = AIMath.Relativpeilung_Peilung_Punkt(mSPD.x, mSPD.z, mSPD.KdW, aSPD.x, aSPD.z);
           CArea VTG = in_Area(AIConst.cAreaTyp_VTG, mSPD.lon, mSPD.lon);
               
          
           double peilMA=AIMath.Relativpeilung_Peilung_Punkt(mSPD.x,mSPD.z,mSPD.KdW,aSPD.x,aSPD.z);
           double peilAM=AIMath.Relativpeilung_Peilung_Punkt(aSPD.x,aSPD.z,aSPD.KdW,mSPD.x,mSPD.z);
           double kursdiff = AIMath.Diff_Winkel(mSPD.KdW, aSPD.KdW);
           string regel = msgregel[aFzg.ich];
           
           //selber oder Gegner gestoppt
           if (Math.Abs(mSPD.FdW)<1.0*kn  || Math.Abs(aSPD.FdW)<1.0*kn) {return (VTG==null)?krit_Entf_ortsfest:krit_Entf_ortsfest_VTG; }
           
           //(Regel 13) Überholen
           if (regel=="13+")  {return (VTG==null)?krit_Entf_ueberholen:krit_Entf_ueberholen_VTG; }
           if (regel=="13-")  {return (VTG==null)?(krit_Entf_ueberholen*0.7d):(krit_Entf_ueberholen_VTG*0.7d); }
           //Regel 14 Entgegengesetzte Kurse
           if (regel=="14")   {return (VTG==null)?krit_Entf_entgegen:krit_Entf_entgegen_VTG; }
           
           //otherwise ...
           return (VTG==null)?krit_Entf_kreuz:krit_Entf_kreuz_VTG; 
       }
       
     

      
       //Bewertung des Tracks nach Kriterien:
       //        -möglichst wenig von Area-Regeln abweichen
       
       //bewertet für Abschnitte (oder tManoeverende für den Rest) die Strafpunkte für Arearelevante Verstösse, zB Schiff auf Land
       //und sorgt für vorzeitigen Abbruch bei heftigem Verstoss (Branch&Bound)
       public double Bewertung_Track_Areas(CTrack iTrack, int gegnerindex,int indexManoeverbeginn, int indexManoeverende)
       {
           CShipPhysicalData iSPD;
           double penalty = 0;
           
           for (int i = indexManoeverbeginn; i < indexManoeverende; i+=AIConst.check_steps)
           {
               iSPD = iTrack.ListeSPD[i];
               //if (iSPD.timestamp > tManoeverende) break;  //wenn wieder auf Orignaltrack, dann keine Bewertung

               if (in_Area(AIConst.cAreaTyp_NOGO, iSPD.lat, iSPD.lon) != null) 
               {
                   return -AIConst.penalty_NOGO;
               }

               CArea Area = in_Area(AIConst.cAreaTyp_VTG, iSPD.lat, iSPD.lon);
               if (Area != null)
               {
                   double deltakurs = Math.Abs(AIMath.Diff_Winkel(iSPD.KdW , Area.richtung));
                          
                   penalty += AIMath.Wert_Ermitteln("i", deltakurs, new List<double> {5, 180}, 
                       new List<double> { 0, AIConst.penalty_VTG_Abweichung180_min })*AIConst.check_steps*AIConst.delta_t/60d;
               }
           }
           return -penalty;
       }
       
       
       //macht eine Gesamtbewertung vom Track inklsuive Arebewertung für den letzten Abschnitt
       //Bewertung des Tracks nach Kriterien:
       //      -möglichst grosser CPA mit Gegner
       //      -möglichst wenig KritischeSituationen mit anderen (ausser manuelles Schiff)
       //      -möglichst wenig Zeitverzug bis Ankunft
       //      -möglichst wenig vom Plan abweichen
       //      -möglichst wenig von Area-Regeln abweichen
       public double Bewertung_Track_Ende(CTrack iTrack, int gegnerindex,int indexManoeverbeginn, double indexManoeverende)
       {
           string msg="";
           double penalty = 0d;
           double tManoeverende = indexManoeverende * AIConst.delta_t;
           
           //if (tManoeverende <= AIglobal.sim_duration)
           {
               double delta_richtung = 0d;
               
               double dist = AIMath.Dist_XZ(Track.ListeSPD[^1].x, Track.ListeSPD[^1].z, iTrack.ListeSPD[^1].x, iTrack.ListeSPD[^1].z);
               if (dist < 0.2 * sm)
               {
                   double delta_t =iTrack.gesamtzeit()- Track.gesamtzeit();
                   penalty += (delta_t / 60d) * AIConst.penalty_verlorene_Minute;
                   msg += "\nMin verloren"+(delta_t / 60d).ToString("0.0");
                         
                   double delta_wegstrecke = iTrack.wegstrecke(0,AIConst.check_steps)-Track.wegstrecke(0,AIConst.check_steps);
                   penalty += (delta_wegstrecke / sm) * AIConst.penalty_zusaetzliche_sm; 
                   msg += "\nsm mehr"+(delta_wegstrecke / sm).ToString("0.0");
               }
               else
               {
                   penalty += penalty + (dist / sm) * AIConst.penalty_zusaetzliche_sm;
                   msg += "\nsm mehr"+(dist / sm).ToString("0.0");
                   
                   if (iTrack.ListeSPD[^1].FdW > 0)
                   {
                       penalty +=  ((dist/sm) / (iTrack.ListeSPD[^1].FdW/60d)) * AIConst.penalty_verlorene_Minute;
                       msg += "\nMin verloren"+((dist/sm) / (iTrack.ListeSPD[^1].FdW/60d)).ToString("0.0");
                   }
               }

               if (tManoeverende > AIglobal.sim_duration)
               {
                   penalty += penalty;
               }
               
               if (indexManoeverbeginn >= 1 && indexManoeverbeginn+1<iTrack.ListeSPD.Count)
               {
                   if (iTrack.ListeSPD[indexManoeverbeginn+1].Fahrstufe!=iTrack.ListeSPD[indexManoeverbeginn - 1].Fahrstufe) penalty+=AIConst.penalty_fahrtaenderung;
                   //penalty +=Math.Abs((iTrack.ListeSPD[indexManoeverbeginn].Fahrstufe - iTrack.ListeSPD[indexManoeverbeginn - 2].Fahrstufe)) * AIConst.penalty_fahrtaenderung;
               }
               
           }

           listKritischeSituationen = null;
           Berechne_listKritischeSituationen2(iTrack, current_Track_index, ref listKritischeSituationen);//alle
           
           double timenow = iTrack.ListeSPD[current_Track_index].timestamp;
           //Auswertung KritischeSituationen
           int g = 0;
           double mindist = 2 * sm;

           msg += "\nCPAs";
           
           for (g = 0; g < AIglobal.Fahrzeuge.Count; g++)
           {
               CFzg Gegner = AIglobal.Fahrzeuge[g];
               if (Gegner.name == name) continue;

               foreach (CCPA KS in listKritischeSituationen[g])
               {
                   if (KS.cpa_zeit() >= timenow) //keine vergangenen kritischen SItuationen betrachteb
                   {
                       if (g == gegnerindex || KS.cpa_zeit() < tManoeverende)
                       {
                           if (KS._cpa_dist < mindist) mindist = KS._cpa_dist;
                           double tmp = KS._krit_dist;
                           if (KS.cpa_gegner_peilt_mich() > 113) 
                               tmp = tmp * 0.6d;
                           
                           double tmp2= AIMath.Wert_Ermitteln("i", KS._cpa_dist, new List<double> { tmp, 0 }, new List<double> { 0, 1 });
                           tmp2 *= tmp2;
                           penalty +=AIConst.penalty_Gegner_krit_Entf+ tmp2*(AIConst.penalty_Gegner_Nahbereich-AIConst.penalty_Gegner_krit_Entf);
                           
                           msg += "\n!"+Gegner.name+(KS._cpa_dist).ToString("N0")+"m@"+KS._cpa_zeit.ToString("N0")+"s";
                       }
                       else
                       {
                           penalty += AIConst.penalty_Andere_krit_Entf;
                           msg += "\n"+Gegner.name+(KS._cpa_dist).ToString("N0")+"m@"+KS._cpa_zeit.ToString("N0")+"s";
                       }
                   }

               }

           }

           //iTrack.name += "||" + msg;//+"\n\n\n\n\n\n"; //mindist.ToString("0.0");
           iTrack.beschreibung += msg;
           return -penalty;

       }
       
       


       //Spieler
       //1.Controls -> SPD
       //2.Änderung->neuer Track wird berechnet aufgrund von ggw Geschwindigkeit, Ruderlage 0
       //  ggf mit Beendigung Ausweichmanöver der anderen, Neubewertung
       
       
       //TODO
       //Was ist mit Fzg2->ausprobieren!
       //Was ist bei vielen Fahrzeugen, ein Ausweichmanöver soll nicht alle zukünftigen Ausweichsituationen lösen:
       //                Lösung?:   andere Einheiten nur für begrenzte Horizont (Zeit/Entfernung bewerten)? Z.B. bis bringe auf Track?  
       
       //gehört zu  simulate_track_update
       
       bool[] bam_ausweichen;            //ist ein Ausweichmanöver gerade im gang um zu verhindern, daß ständig neu bewertet wird. 
       private double[] ausweichmanoever_ingang; //0: keines ingang, 1 auf Ausweichentfernung, 0.5 auf halbe Ausweichentfernung, 0.25 auf letztes Manöver
       private double[] tausweichen_bis; //wann ist das Ausweichmanöver nach Berechnung zu Ende
       
       ///CCPA[] iCPAs;            //CPA-Objekte zu den jeweiligen Gegnern
       public List<CCPA>[] listKritischeSituationen;
       public List<CCPA> sortedlistKritischeSituationen=new List<CCPA>();
       
       double[] idist_old;      //Distanz zum Gegner aus der vorherigen Berechnung um festzustellen, ob die Distanz zunimmt
       string[] msgregel ;      //Regel, aufgrund derer vom Gegner g ausgewichen wird oder kursgehalten
       string[] msgmanoever;    //Regel, nach der das Manoever auszuwaehlen ist
       public bool[] bkurshaltepflichtig;  //nur relevant dem manuellen Spieler gegenüber
       public bool[] bausweichpflichtig;   //Ende des Manövers: ausweichpflicht besteht weiter, bis
                                           //(1) CPA sich vergrössert && Abstand>Nahbereich
                                           //(2) Gegner Ausweichmanöver fährt, dann Neubewertung Ausweichpflicht etc
                                           //(3) Spieler Kurs/Fahrt ändert
                                           //und bedeutet, daß keine Neuberwertung der Situation mit dem Gegner stattfindet
       public bool[] bletzterAugenblick;
                                           
                                           //public double[] t_aufTrack; //Zeit, wann die Unit nach Ausweichmanoever wieder auf dem Track des Planes ist

       public void simulate_track_initialization(CTrack iTrack)
       {
           for(int g=0;g<AIglobal.Fahrzeuge.Count;g++)
           {
               CFzg Fzg = AIglobal.Fahrzeuge[g];
               
               //Debug.Log(name+"-"+current_Track_index+" Init");
              
               Fzg.idist_old= new double[AIglobal.Fahrzeuge.Count];
               Fzg.bam_ausweichen= new bool[AIglobal.Fahrzeuge.Count];
               Fzg.ausweichmanoever_ingang = new double[AIglobal.Fahrzeuge.Count];
               Fzg.tausweichen_bis= new double[AIglobal.Fahrzeuge.Count];
               Fzg.msgregel= new string[AIglobal.Fahrzeuge.Count];
               Fzg.msgmanoever= new string[AIglobal.Fahrzeuge.Count];
               Fzg.bkurshaltepflichtig= new bool[AIglobal.Fahrzeuge.Count];
               Fzg.bausweichpflichtig=  new bool[AIglobal.Fahrzeuge.Count];
               Fzg.bletzterAugenblick=new bool[AIglobal.Fahrzeuge.Count];
           }
          
           foreach(CFzg Fzg in AIglobal.Fahrzeuge)
               if (Fzg.istManuell == false && Fzg.listKritischeSituationen!=null)
                   Fzg.listKritischeSituationen[ich] = null; //Fzg.update_meine_listKritischeSituationen();//alle null setzen
       }

       
       //msg_regel =Nach welcher Regel offiziell wird ausgewichen, msg_manoever==nach welcher Regel wird dias Ausweichmanoever gewählt
      public bool ausweichpflichtig(CShipPhysicalData mSPD,CFzg aFzg, CShipPhysicalData aSPD, int typOrtung,ref string msg_regel,ref string msg_manoever,bool bletztesManoever=false)
       {
           double peilMA=AIMath.Relativpeilung_Peilung_Punkt(mSPD.x,mSPD.z,mSPD.KdW,aSPD.x,aSPD.z);
           double peilAM=AIMath.Relativpeilung_Peilung_Punkt(aSPD.x,aSPD.z,aSPD.KdW,mSPD.x,mSPD.z);
           double kursdiff = AIMath.Diff_Winkel(mSPD.KdW, aSPD.KdW);
           CArea VTG = null;
           CArea Fahrrinne = null;
           //foreach(CArea A in AIglobal.Gebiete) {if (A.typ==AIConst.cAreaTyp_VTG && A.Pos_enthalten(aSPD.lat,aSPD.lon) ) {VTG =A;break;}}
           //foreach(CArea A in AIglobal.Gebiete) {if (A.typ==AIConst.cAreaTyp_Fahrrinne && A.Pos_enthalten(aSPD.lat,aSPD.lon) ) {Fahrrinne =A;break;}}

           msg_regel = "";
           msg_manoever = "";
           // NICHT ausweichpflichtig
           if (!bletztesManoever)
           {
              if (aFzg.istMaschinenfzg && (istFischer || istManoevrierbeh || istManoevrierunf || istSegler)) 
              {msg_regel="18a"; return false; }
              if (aFzg.istSegler && (istFischer || istManoevrierbeh || istManoevrierunf)) 
              {msg_regel="18b"; return false; }
              if (aFzg.istFischer && (istManoevrierbeh || istManoevrierunf)) 
              {msg_regel="18c"; return false; } 
           }
           
           
           
           //auf jeden Fall ausweichpflichtig
           if (istMaschinenfzg && (aFzg.istFischer || aFzg.istManoevrierbeh || aFzg.istManoevrierunf || aFzg.istSegler)) 
           {msg_regel="18a";}
           if (istSegler && (aFzg.istFischer || aFzg.istManoevrierbeh || aFzg.istManoevrierunf)) 
           {msg_regel="18b";}
           if (istFischer && (aFzg.istManoevrierbeh || aFzg.istManoevrierunf)) 
           {msg_regel="18c";}
           if (bletztesManoever)   
           {msg_regel="Letztes Manoever";}    
           
           //Regel Weyer:"In der Praxis gilt immer, dass ein aufgestopptes Fahrzeug aufgestoppt liegen bleibt und der umgebende Verkehr erforderlichenfalls ausweicht
           if (Math.Abs(mSPD.FdW)<1.0*kn) 
             {msg_regel="Eigenschiff ohne Fahrt (Weyer)"; return false; }
           if (Math.Abs(aSPD.FdW)<1.0*kn ) 
             {msg_regel="Gegner ohne Fahrt (Weyer)"; return true; }
           
           /*Regel 10
             a) Diese Regel gilt in Verkehrstrennungsgebieten, die von der Organisation festgelegt worden sind; sie befreit ein Fahrzeug nicht von seiner Verpflichtung auf Grund einer anderen Regel.
             b) Ein Fahrzeug, das ein Verkehrstrennungsgebiet benutzt, muß
              i) auf dem entsprechenden Einbahnweg in der allgemeinen Verkehrsrichtung dieses Weges fahren;
                 -->> Bewertungfunktion: Vergleich mit Richtung, Prozentsatz Abweichung (klein, mittel, gross)
              ii) sich, soweit möglich, von der Trennlinie oder der Trennzone klar halten;
                 -->> Bewertungfunktion: Trennzone=NOGO
              iii) in der Regel an den Enden des Einbahnwegs ein- oder auslaufen; wenn es jedoch von der Seite ein- oder ausläuft, muß dies in einem möglichst kleinen Winkel zur allgemeinen Verkehrsrichtung erfolgen.
                -->> Bewertungfunktion: richtig eingelaufen? wenig Kursdifferenz zu VTG.Richtung bis Verlassen des VTG, ansonsten "in fahrtrichtung eingelaufen, aber nicht an VTG.Richtung gehalten"
             c) Ein Fahrzeug muß soweit wie möglich das Queren von Einbahnwegen vermeiden; ist es jedoch zum Queren gezwungen, so muß dies möglichst mit der Kielrichtung im rechten Winkel zur allgemeinen Verkehrsrichtung erfolgen.
                -->> Bewertungfunktion: quer eingelaufen? wenig Kursänderung bis verlassen des VTG ansonsten "quer einglaufen, aber VTG nicht gequert"
                
             d) 
              i) Ein Fahrzeug darf eine Küstenverkehrszone nicht benutzen, wenn es den entsprechenden Einbahnweg des angrenzenden Verkehrstrennungsgebiets sicher befahren kann. Fahrzeuge von weniger als 20 Meter Länge, Segelfahrzeuge und fischende Fahrzeuge dürfen die Küstenverkehrszone jedoch benutzen.
                -->> Bewertungfunktion: falls Maschfzg>20m, dann Negativpunkte
              ii) Ungeachtet der Ziffer i darf ein Fahrzeug eine Küstenverkehrszone benutzen, wenn es sich auf dem Weg zu oder von einem Hafen, einer Einrichtung oder einem Bauwerk vor der Küste, einer Lotsenstation oder einem sonstigen innerhalb der Küstenverkehrszone gelegenen Ort befindet, oder zur Abwendung einer unmittelbaren Gefahr.
             
             e) Außer beim Queren oder beim Einlaufen in einen Einbahnweg oder beim Verlassen eines Einbahnweges darf ein Fahrzeug in der Regel nicht in eine Trennzone einlaufen oder eine Trennlinie überfahren, ausgenommen
              i) in Notfällen zur Abwendung einer unmittelbaren Gefahr;
              ii) zum Fischen innerhalb einer Trennzone.
               -->>Bewerten Trennzone außer Fischer
             f) Im Bereich des Zu- und Abgangs der Verkehrstrennungsgebiete muß ein Fahrzeug mit besonderer Vorsicht fahren.
             g) Ein Fahrzeug muß das Ankern innerhalb eines Verkehrstrennungsgebiets oder im Bereich des Zu- und Abgangs soweit wie möglich vermeiden.
             h) Ein Fahrzeug, das ein Verkehrstrennungsgebiet nicht benutzt, muß von diesem einen möglichst großen Abstand halten.
             i) Ein fischendes Fahrzeug darf die Durchfahrt eines Fahrzeugs auf dem Einbahnweg nicht behindern.
             j) Ein Fahrzeug von weniger als 20 Meter Länge oder ein Segelfahrzeug darf die sichere Durchfahrt eines Maschinenfahrzeugs auf dem Einbahnweg nicht behindern.
               --->>> <20m oder Segler -->> ausweichpflichtig  vgl Regel 18 */
               if (laenge<20 || istSegler) {msg_regel="10j"; return true; }
           /*        
             k) Ein manövrierbehindertes Fahrzeug, das in einem Verkehrstrennungsgebiet Arbeiten zur Aufrechterhaltung der Sicherheit der Schiffahrt durchführt, ist von der Befolgung dieser Regel befreit, soweit dies zur Ausführung der Arbeiten erforderlich ist.
             l) Ein manövrierbehindertes Fahrzeug, das in einem Verkehrstrennungsgebiet Unterwasserkabel auslegt, versorgt oder aufnimmt, ist von der Befolgung dieser Regel befreit, soweit dies zur Ausführung der Arbeiten erforderlich ist.
           */
           
           
           //Regel 19: Bewertung Fahr/Ausweichmanöver
           if (typOrtung != AIConst.cOrtung_Sicht)
           {
              //a) Diese Regel gilt für Fahrzeuge, die einander nicht in Sicht haben, wenn sie innerhalb oder in der Nähe eines Gebiets mit verminderter Sicht fahren.
              //b) Jedes Fahrzeug muß mit sicherer Geschwindigkeit fahren, die den gegebenen Umständen und Bedingungen der verminderten Sicht angepaßt ist. Ein Maschinenfahrzeug muß seine Maschinen für ein sofortiges Manöver bereithalten.
              //c) Jedes Fahrzeug muß bei der Befolgung der Regeln des Abschnitts I die gegebenen Umstände und Bedingungen der verminderten Sicht gehörig berücksichtigen.
              //d) Ein Fahrzeug, das ein anderes Fahrzeug lediglich mit Radar ortet, muß ermitteln, ob sich eine Nahbereichslage entwickelt und/oder die Möglichkeit der Gefahr eines Zusammenstoßes besteht. Ist dies der Fall, so muß es frühzeitig Gegenmaßnahmen treffen; ändert es deshalb seinen Kurs, so muß es nach Möglichkeit folgendes vermeiden:
               
              //i)eine Kursänderung nach Backbord gegenüber einem Fahrzeug vorlicher als querab, außer beim Überholen;
                     // bei Bewertung: unabhängig von der Ausweichpflicht Nahbereichslagen vermeiden, außer beim Überholen
              
              //ii)eine Kursänderung auf ein Fahrzeug zu, das querab oder achterlicher als querab ist.
                    //bei Bewertung: unabhängig von der Ausweichpflicht Nahbereichslagen vermeiden
              
              //e) Außer nach einer Feststellung, daß keine Möglichkeit der Gefahr eines Zusammenstoßes besteht, muß jedes Fahrzeug, das anscheinend vorlicher als querab das Nebelsignal eines anderen Fahrzeugs hört oder das eine Nahbereichslage mit einem anderen Fahrzeug vorlicher als querab nicht vermeiden kann, seine Fahrt auf das für die Erhaltung der Steuerfähigkeit geringstmögliche Maß verringern. Erforderlichenfalls muß es jegliche Fahrt wegnehmen und in jedem Fall mit äußerster Vorsicht manövrieren, bis die Gefahr eines Zusammenstoßes vorüber ist.
                    //bei Bewertung: unabhängig von der Ausweichpflicht bei Nahbereichslagen mit vorne Fzgen Fahrtverminderung
               
               
                    msg_regel="12ii"; return true;
           }
           
           
           //Regel 16: klare Kurs/Geschwindigkeitsänderung: ergibt sich aus den zu testenden Ausweichmanövern
           //Regel 17 Kurshalter: Attribut, wer gegenüber wem Kurshalter ist bis Mindestabstand hergestellt
           
          
           //if (VTG != null)
           {
               
               //Regel 12 Segelfahrzeuge
               if (istSegler && aFzg.istSegler)
               {
                   double m_rel_windrichtung = AIMath.Diff_Winkel(mSPD.KdW, AIglobal.Windrichtung);
                   double a_rel_windrichtung = AIMath.Diff_Winkel(mSPD.KdW, AIglobal.Windrichtung);
                   //i)Wenn sie den Wind nicht von derselben Seite haben, muß das Fahrzeug, das den Wind von Backbord hat, dem anderen ausweichen;
                   if (Math.Sign(m_rel_windrichtung) != Math.Sign(a_rel_windrichtung))
                   {
                       if (m_rel_windrichtung<0) {msg_regel="12i"; return true; }
                   }
                   //ii)wenn sie den Wind von derselben Seite haben, muß das luvwärtige Fahrzeug dem leewärtigen ausweichen;
                   if (Math.Sign(m_rel_windrichtung) == Math.Sign(a_rel_windrichtung))
                   {
                       //berechne ich so: ein Fzg bekommt eine Minimalverschiebung in Richtung Wind. Entfernt es sich vom anderen Fzg, ist es die Luv-Seite
                       double dist1 = AIMath.Dist_XZ(mSPD.x, mSPD.z, aSPD.x, aSPD.lon);
                       double dist2 = AIMath.Dist_XZ(mSPD.x+Math.Sin(AIglobal.Windrichtung)*0.1d, mSPD.z+Math.Cos(AIglobal.Windrichtung)*0.1d, aSPD.x, aSPD.lon);
                       if (dist2>dist1) {msg_regel="12ii"; return true; }
                   }
                   //iii)wenn ein Fahrzeug mit Wind von Backbord ein Fahrzeug in Luv sichtet und nicht mit Sicherheit feststellen kann, ob das andere Fahrzeug den Wind von Backbord oder von Steuerbord hat, muß es dem anderen ausweichen.
                   if (Math.Sign(m_rel_windrichtung) == -1 && Math.Sign(peilMA)==-1)
                   {
                        {msg_regel="12iii"; return true; }
                   }

               }
               
               //Regel 13 Überholen
               //1.Überholender
               if ((istMaschinenfzg && aFzg.istMaschinenfzg) && Math.Abs(peilAM) > 90d + 22.5d && Math.Abs(kursdiff) < 45 && mSPD.FdW > aSPD.FdW)
               {
                   msg_manoever="13+";
                   if (msg_regel == "") msg_regel = msg_manoever;//wenn  nicht schon nach §18 ausweichpflichtig
                   return true;
               }
               //1.ÜberholTer
               if ((istMaschinenfzg && aFzg.istMaschinenfzg) && Math.Abs(peilMA) > 90d + 22.5d && Math.Abs(kursdiff) < 45 && mSPD.FdW < aSPD.FdW)
               {
                   msg_manoever="13-"; 
                   if (msg_regel == "") msg_regel = msg_manoever;//wenn  nicht schon nach §18 ausweichpflichtig
                   return true;
               }
               
               //Regel 14 Entgegengesetzte Kurse
               if ((istMaschinenfzg && aFzg.istMaschinenfzg) && Math.Abs(peilMA) < 22.5 && Math.Abs(kursdiff) > 160)
               {
                   msg_manoever="14"; 
                   if (msg_regel == "") msg_regel = msg_manoever;//wenn  nicht schon nach §18 ausweichpflichtig
                   return true;
               } //... muß jedes seinen Kurs nach Steuerbord so ändern, daß sie einander an Backbordseite passieren.
               
               
               
               //Regel 15 Kreuzende Kurse
               if (istMaschinenfzg && aFzg.istMaschinenfzg && peilMA > 0 && peilMA < 180) //Regel 14 trifft nicht zu
               {
                   if (!(peilAM > 0 && peilAM < 180))
                   {
                       msg_manoever="15"; 
                       if (msg_regel == "") msg_regel = msg_manoever;//wenn  nicht schon nach §18 ausweichpflichtig
                       return true;
                   } //der andere hat einen nicht auch steuerbord
                   
                   //Sonderfall: der andere hat einen AUCH steuerbord
                   double sx = 0, sz = 0;
                   bool bschnitt = AIMath.Schnitt_Gerade1_Gerade2(mSPD.x, mSPD.z, mSPD.KdW, aSPD.x, aSPD.z, aSPD.KdW, ref sx, ref sz);
                   if (bschnitt)
                   {
                       double peilSchnitt=AIMath.Relativpeilung_Peilung_Punkt(mSPD.x,mSPD.z,mSPD.KdW,sx,sz);
                       if (Math.Abs(peilSchnitt) < 90d)
                       {
                           msg_manoever="15"; 
                           if (msg_regel == "") msg_regel = msg_manoever;//wenn  nicht schon nach §18 ausweichpflichtig
                           return true;
                       }
                   }
               }
               
               //Regel 15 Kreuzende Kurse, anderes Fzg hat aber Vorfahrt
               if (msg_regel != "" && msg_manoever=="" && istMaschinenfzg && aFzg.istMaschinenfzg && peilMA < 0 && peilMA > -90) //Regel 14 trifft nicht zu
               {  
                   if ((peilAM > 0 && peilAM < 180)) {msg_manoever="15-"; return true;} //der andere hat einen nicht auch steuerbord
                   
                   //Sonderfall: der andere hat einen AUCH backbord
                   double sx = 0, sz = 0;
                   bool bschnitt = AIMath.Schnitt_Gerade1_Gerade2(mSPD.x, mSPD.z, mSPD.KdW, aSPD.x, aSPD.z, aSPD.KdW, ref sx, ref sz);
                   if (bschnitt)
                   {
                       double peilSchnitt=AIMath.Relativpeilung_Peilung_Punkt(mSPD.x,mSPD.z,mSPD.KdW,sx,sz);
                       if (Math.Abs(peilSchnitt) < 90d)
                       {
                           msg_manoever="15-"; 
                           return true;
                       }
                   }
               }
               
               
               
               
               
               //es fehlt noch 18d: Jedes Fahrzeug mit Ausnahme eines manövrierunfähigen oder manövrierbehinderten muß, sofern die Umstände es zulassen, vermeiden, die sichere Durchfahrt eines tiefgangbehinderten Fahrzeugs zu behindern, das Signale nach Regel 28 zeigt.
               //es fehlt noch 18e-f: hier nicht sinnvoll?
           }
           if (msg_regel == "")
           {
               return false;
           }
           else
           {
               if (msg_manoever == "") msg_manoever = msg_regel;
               return true;
           }
           
       }
       
       //returniert die Zeit, zu dem das Manöver zu Ende ist.
       public bool busy_tAusweichmanoever = false;
       public string tmp_msg="";
       private List<CTrack> Opt_Tracks = new List<CTrack>();
       public double tAusweichmanoever(string manoeverregel,ref CTrack iTrack,int index_iTrack,CFzg Gegner, double cpazeit)
       {
           busy_tAusweichmanoever = true;
           DateTime ti = DateTime.Now;
           int zaehler= 0;
           //Opt_Tracks.Clear();
           int plan_counter = Plan.counter;                 //beide Plan-Variablen merken, weil die das Durchberechnen 
           double plan_warte_bis = Plan.warte_bis;//der Optionen verwendet und danach zurückgesetzt werden

           double curr_time = iTrack.SPD(index_iTrack).timestamp;
           CShipPhysicalData iSPD = iTrack.SPD(index_iTrack+AIglobal.reactionsteps);
           double kurs = iSPD.KdW;
           double gegnerkurs = Gegner.Track.SPD(index_iTrack+AIglobal.reactionsteps).KdW;
           double gegnerfahrstufe= Gegner.Track.SPD(index_iTrack+AIglobal.reactionsteps).Fahrstufe;
          
           double max_t = 0;
           string msgstart = "§" + manoeverregel + ":" + name + "-" + Gegner.name+"/";
           
           //hier bewerten "kein Ausweichmanöver"------------------------
           int indexManoeverstart =  index_iTrack + AIglobal.reactionsteps;
           int indexManoeverende = indexManoeverstart + Math.Abs(((int)(1.5 * (cpazeit / AIConst.delta_t - indexManoeverstart))));
           if (indexManoeverende >(int) (AIglobal.sim_duration / AIConst.delta_t) || indexManoeverende <= indexManoeverstart) 
               indexManoeverende = (int) (AIglobal.sim_duration / AIConst.delta_t);
           double max_bew = Bewertung_Track_Ende(Track, Gegner.ich, indexManoeverstart, indexManoeverende);
           CTrack max_Track=Track.copy();
           this.ObjSC.Data.Debug1 += "(*)=" + max_bew.ToString("0.0")+"\n";
           max_Track.expiry_time = curr_time + AIglobal.deletetrackoptionsafter;
           max_Track.name = "[*]"+msgstart +"#orig *"+max_bew.ToString("0.00")+"|";
           Opt_Tracks.Add(max_Track);
           //-------------------------------------------------------
           
           double t_aufTrack = 0;
           string msg = "";
           CTrack nTrack=null;
           CShipPhysicalData nSPD=null;
           
           // NOTFALLMANOEVER=aufstoppen
           if (ausweichmanoever_ingang[Gegner.ich] == faktor_LetztesManoever)   //aufstoppen bis Gegner sich entfernt hat
           {
               nTrack = new CTrack();
               nTrack.ListeSPD.AddRange(Track.ListeSPD.GetRange(0, index_iTrack + AIglobal.reactionsteps));
               int index_beginn_manoever = nTrack.ListeSPD.Count-1;
               nSPD = nTrack.ListeSPD[^1].Copy();nSPD.Ruderlage = 0; nSPD.Winkelv = 0;
              
               //aufstoppen bis Gegner sich entfernt hat
               fahre_Kurs_bis(AIConst.cBedingung_aufstoppen_bis_Gegner_Distanz, -1, 1*sm, 0, -1, -1, -1, -1, Gegner, nSPD, nTrack, index_iTrack);
               
               if (nTrack.ListeSPD[^1].timestamp<AIglobal.sim_duration) verfolge_Plan(Plan, nSPD, ref t_aufTrack, 15, nTrack);
              
               nTrack.name = "#aufstoppen: Manoever des letzten Augenblickes";
               iTrack = nTrack;
               
               int index_ende_rueckkehr =verfolge_Plan(Plan, nSPD, ref t_aufTrack, 30, nTrack); //Rest des GTracks
               
               Plan.counter = plan_counter;
               Plan.warte_bis = plan_warte_bis;
                                                      
               double bew = Bewertung_Track_Ende(nTrack, Gegner.ich, index_beginn_manoever, index_ende_rueckkehr-1);
                                                      
               nTrack.name += "*" + bew.ToString("0.00");
               nTrack.itmp = index_iTrack;
               nTrack.expiry_time = curr_time + AIglobal.deletetrackoptionsafter;
               
                   
                                                     
               Opt_Tracks.Add(nTrack.copy());
               
               if (bew > max_bew)
               {
                   max_Track = nTrack.copy();
                   max_bew = bew;
                   max_t = t_aufTrack;
               }
               
               //busy_tAusweichmanoever = false;
               //return t_aufTrack;
           }
           
           
           
           //"Gegner ohne Fahrt (Weyer)"
           if (manoeverregel == "Gegner ohne Fahrt (Weyer)")
           {
               Ausweichraute(new List<double> { 2, AIConst.opt_Fahrtstufe_halten,  17 },
                   AIConst.drehen_um, new List<double> { -60,-45,-30,-40,30, 45, 60, },
                   AIConst.entfernen_bis_distanz, new List<double> { 0.5*sm, 1*sm, 1.5*sm},
                   new List<double> { AIConst.parallel_eigener_kurs },
                   AIConst.parallel_bis_gegner_achtern, new List<double> { 100,135 },
                   new List<double> { 30 },0);
           } 
           
           
           
           if (manoeverregel == "10j") //Segler im Verkehrstrennungsgebiet
           {
               Ausweichraute(new List<double> {0.5*iSPD.Fahrstufe, AIConst.opt_Fahrtstufe_halten },
                   AIConst.drehen_um, new List<double> { -90,-45,30, 30, 45,90 },
                   AIConst.entfernen_bis_distanz, new List<double> { 0.25*sm, 0.5*sm, 0.75*sm,1*sm},
                   new List<double> { AIConst.parallel_gegner_kurs},
                   AIConst.parallel_bis_distanz, new List<double> { 0.5*sm,1*sm },
                   new List<double> {30 },0);
           }


           if (manoeverregel == "12i" || manoeverregel == "12ii" || manoeverregel == "12ii") //Regel 12 Segler untereinander
           {
               Ausweichraute(new List<double> { AIConst.opt_Fahrtstufe_halten },
                   AIConst.drehen_um, new List<double> { -45,30, 30, 45, },
                   AIConst.entfernen_bis_distanz, new List<double> { 0.25*sm, 0.5*sm, 0.75*sm,1*sm},
                   new List<double> { AIConst.parallel_eigener_kurs },
                   AIConst.parallel_bis_gegner_achtern, new List<double> { 100,135 },
                   new List<double> {30 },0);
           }
           
           
           
           
           if (manoeverregel == "13+") //Regel 13 Überholen
           {
               
               Ausweichraute(new List<double> {gegnerfahrstufe },
                       AIConst.drehen_um, new List<double> { 0 },
                       AIConst.entfernen_bis_distanz, new List<double> { 0.5*sm, 1*sm, 1.5*sm},
                       new List<double> { AIConst.keine_aktion },
                       AIConst.keine_aktion, new List<double> { AIConst.keine_aktion },
                       new List<double> { 6 },0);
               
               
               Ausweichraute(new List<double> { AIConst.opt_Fahrtstufe_halten,  17 },
                   AIConst.drehen_um, new List<double> { -45,30, 30, 45, },
                   AIConst.entfernen_bis_distanz, new List<double> { 0.25*krit_Entf_ueberholen, 0.5*krit_Entf_ueberholen,0.75*krit_Entf_ueberholen, 1.5*krit_Entf_ueberholen},
                   new List<double> { AIConst.parallel_gegner_kurs,AIConst.parallel_eigener_kurs },
                   AIConst.parallel_bis_gegner_achtern, new List<double> { 100,135 },
                   new List<double> { 6 },0);
           }
           if (manoeverregel == "13-") //Regel 13 Überholen
           {
               Ausweichraute(new List<double> { AIConst.opt_Fahrtstufe_halten,  4 },
                   AIConst.drehen_um, new List<double> { -45,30, 30, 45, },
                   AIConst.entfernen_bis_distanz, new List<double> { 0.25*krit_Entf_ueberholen, 0.5*krit_Entf_ueberholen,0.75*krit_Entf_ueberholen, 1.5*krit_Entf_ueberholen},
                   new List<double> { AIConst.parallel_gegner_kurs,AIConst.parallel_eigener_kurs },
                   AIConst.parallel_bis_gegner_voraus, new List<double> { 45,22 },
                   new List<double> { 6 },0);
           }
           
           

           if (manoeverregel == "14") //entgegengesetzte Kurse
           {
               
               Ausweichraute(new List<double> { AIConst.opt_Fahrtstufe_halten, 3, 6, 17 },
                   AIConst.drehen_um, new List<double> { 30, 40, 50, 60 },
                   AIConst.entfernen_bis_zeit, new List<double> { 120, 240, 360},
                   new List<double> { AIConst.parallel_gegner_kurs180,AIConst.parallel_eigener_kurs },
                   AIConst.parallel_bis_gegner_peilung, new List<double> { -100,-135 },
                   new List<double> { 6 },0);
               
           }
           
           if (manoeverregel == "15") //Kreuzende Kurse
                      {
                          //Hundelinie
                          
                          Ausweichraute(new List<double> { AIConst.opt_Fahrtstufe_halten,   17 },
                                           AIConst.drehen_stb_bis_gegner_peilung, new List<double> { -10, -20},
                                           AIConst.entfernen_bis_gegner_peilung, new List<double> { -45, -90 },
                                           new List<double> { AIConst.parallel_gegner_kurs90 },
                                           AIConst.parallel_bis_gegner_peilung, new List<double> { -100,-135 },
                                           new List<double> { 30 },5);
                          
                          Ausweichraute(new List<double> { AIConst.opt_Fahrtstufe_halten, 3, 6, 17 },
                              AIConst.drehen_um, new List<double> { -60, -30, 0, 30, 40, 50, 60 },
                              AIConst.entfernen_bis_gegner_Abstand, new List<double> { 0.25d*sm, 0.5d*sm, 0.75d*sm,sm, 1.5d*sm },
                              new List<double> { AIConst.keine_aktion },
                              AIConst.parallel_bis_zeit, new List<double> { 0 },
                              new List<double> { 30 });
                      }

           if (manoeverregel == "15-" || manoeverregel == "18a" || manoeverregel == "18b" || manoeverregel == "18c") //Kreuzende Kurse
           {
               //Hundelinie
               
               Ausweichraute(new List<double> { AIConst.opt_Fahrtstufe_halten,   17 },
                                AIConst.drehen_bb_bis_gegner_peilung, new List<double> { 10, 20},
                                AIConst.entfernen_bis_gegner_peilung, new List<double> { 45, 90 },
                                new List<double> { AIConst.parallel_gegner_kurs270 },
                                AIConst.parallel_bis_gegner_peilung, new List<double> { 100,135 },
                                new List<double> { 30 },11);
               
               Ausweichraute(new List<double> { AIConst.opt_Fahrtstufe_halten, 3, 6, 17 },
                   AIConst.drehen_um, new List<double> { 60, 30, 0, -30, -40, -50, -60 },
                   AIConst.entfernen_bis_gegner_Abstand, new List<double> { 0.25d*sm, 0.5d*sm, 0.75d*sm,sm, 1.5d*sm },
                   new List<double> { AIConst.keine_aktion },
                   AIConst.parallel_bis_zeit, new List<double> { 0 },
                   new List<double> { 30 });
           }

           
           //Rueckkehr
           Track_setzen:
            iTrack = max_Track;
            max_Track.name = "!" + max_Track.name;
            max_Track.expiry_time=3600;
            Opt_Tracks.Add(max_Track);
            string zeitangabe = "zeit("+zaehler+"):=" + (DateTime.Now - ti).Milliseconds.ToString();
            //Debug.Log("Zeit für AUsweichmanöver in ms  "+zeitangabe);
            busy_tAusweichmanoever = false;
            return max_t;
           
           
           
           
           
           
           //Dreiecksmanöver , Zeit=0
           void Ausweichraute(List<double> lFahrtstufen,
                              double drehen_bis,   List<double> ldrehen_parameter, 
                              double entfernen_bis, List<double> lentfernen_parameter,
                              List<double> lparallel_kurse,
                              double parallel_bis,  List<double> lparallel_parameter,
                              List<double> lwinkel_zurueck, double bewertungbonus=0f)

           {
               int index_beginn_manoever, index_ende_rueckkehr;
               nTrack = new CTrack();
               nTrack.ListeSPD.AddRange(Track.ListeSPD.GetRange(0, index_iTrack + AIglobal.reactionsteps));
               
               
               
               
               string msgfahrtstufe = "", msgdrehen="",msgentfernen="",msgparallelkurs="",msgparallel="";   
               
               
               foreach (double fahrtstufe in lFahrtstufen) //.............................................................
               {
                   double bew = 0d;
                   if (nTrack.ListeSPD.Count>indexManoeverstart) nTrack.ListeSPD.RemoveRange(indexManoeverstart, nTrack.ListeSPD.Count-indexManoeverstart);
                   
                   nSPD = nTrack.ListeSPD[^1].Copy(); //neue SPD Variable
                   index_beginn_manoever = nTrack.ListeSPD.Count-1;
                   nSPD.Ruderlage = 0; nSPD.Winkelv = 0;
                   
                   kurs = nSPD.KdW;
                   
                   int ifahrtstufe = nTrack.ListeSPD.Count;
                   
                   foreach (double drehen_parameter in ldrehen_parameter) //.............................................................
                   {
                       if (nTrack.ListeSPD.Count>ifahrtstufe) nTrack.ListeSPD.RemoveRange(ifahrtstufe, nTrack.ListeSPD.Count-ifahrtstufe);
                       nSPD = nTrack.ListeSPD[^1].Copy(); nSPD.Ruderlage = 0; nSPD.Winkelv = 0;

                       
                       if (fahrtstufe != AIConst.opt_Fahrtstufe_halten)
                       {
                           nSPD.Fahrstufe = fahrtstufe;
                           msgfahrtstufe= " F" + nSPD.Fahrstufe;
                       }
                       else
                       {
                           msgfahrtstufe= " F*";
                       }
                       
                       
                       switch(drehen_bis) 
                       {
                           case AIConst.drehen_um:
                               double neuerkurs = AIMath.norm(kurs + drehen_parameter, 360);
                               Manoever_Ruderlage_drehe_auf_Kurs(std_ruderlage, neuerkurs, nSPD, nTrack);
                               msgdrehen= "/D" + drehen_parameter + "°";
                               break;
                           case AIConst.drehen_stb_bis_gegner_peilung:
                               nSPD.Ruderlage = 10;
                               Manoever_peile_Gegner_min(drehen_parameter, Gegner, index_iTrack, nSPD, nTrack); 
                               msgdrehen= "/stb^gp" + drehen_parameter + "°";
                               break;
                           case AIConst.drehen_bb_bis_gegner_peilung:
                               nSPD.Ruderlage = -10;
                               Manoever_peile_Gegner_min(drehen_parameter, Gegner, index_iTrack, nSPD, nTrack); 
                               msgdrehen= "/bb^gp" + drehen_parameter + "°";
                               break;
                           
                       }

                       int idrehen = nTrack.ListeSPD.Count;//also 1 weiter
                       
                       foreach (double entfernen_parameter in lentfernen_parameter) //.............................................................
                       {
                           if (nTrack.ListeSPD.Count>idrehen) nTrack.ListeSPD.RemoveRange(idrehen, nTrack.ListeSPD.Count-idrehen);
                           nSPD = nTrack.ListeSPD[^1].Copy(); nSPD.Ruderlage = 0; nSPD.Winkelv = 0;

                           {
                               switch(entfernen_bis) 
                               {
                                   case AIConst.entfernen_bis_zeit:
                                       Manoever_Fahre_weiter_Zeit(entfernen_parameter, nSPD, nTrack);
                                       msgentfernen= "/E" + entfernen_parameter + "s";
                                       break;
                                   case AIConst.entfernen_bis_distanz:
                                       Manoever_Fahre_weiter_Distanz(entfernen_parameter, nSPD, nTrack);
                                       msgentfernen= "/E" + entfernen_parameter + "m";
                                       break;
                                   case AIConst.entfernen_bis_gegner_peilung:
                                       Manoever_peile_Gegner_min(entfernen_parameter, Gegner, index_iTrack, nSPD, nTrack);
                                       msgentfernen= "/E^gp" + entfernen_parameter + "°";
                                       break;
                                   case AIConst.entfernen_bis_gegner_Abstand:
                                       Manoever_am_Gegner_vorbei(entfernen_parameter, Gegner, index_iTrack, nSPD, nTrack);
                                       msgentfernen= "/E^gd" + entfernen_parameter + "m";
                                       break;
                                   case AIConst.entfernen_bis_track_Abstand:
                                       //todo
                                       break;
                               }
                           }
                           
                           
                           int ientfernen = nTrack.ListeSPD.Count;//also 1 weiter
                           double bew_entfernen = Bewertung_Track_Areas(nTrack, Gegner.ich, index_beginn_manoever, ientfernen-1);
                           if (bew_entfernen<= -AIConst.penalty_NOGO) 
                               continue;
                           
                           
                           foreach (double parallel_kurs in lparallel_kurse) //.............................................................
                           {
                               if (nTrack.ListeSPD.Count>ientfernen) nTrack.ListeSPD.RemoveRange(ientfernen, nTrack.ListeSPD.Count-ientfernen);
                               nSPD = nTrack.ListeSPD[^1].Copy(); nSPD.Ruderlage = 0; nSPD.Winkelv = 0;
                               
                               switch(parallel_kurs) 
                               {
                                   case AIConst.parallel_eigener_kurs:
                                       Manoever_Ruderlage_drehe_auf_Kurs(std_ruderlage, kurs, nSPD, nTrack);
                                       msgparallelkurs= "/Pe^";
                                       break;
                                   case AIConst.parallel_gegner_kurs:
                                       Manoever_Ruderlage_drehe_auf_Kurs(std_ruderlage, gegnerkurs, nSPD, nTrack);
                                       msgparallelkurs= "/Pg^";
                                       break;
                                   case AIConst.parallel_gegner_kurs180:
                                       Manoever_Ruderlage_drehe_auf_Kurs(std_ruderlage, AIMath.norm(gegnerkurs + 180, 360), nSPD, nTrack);
                                       msgparallelkurs= "/Pg180-^";
                                       break;
                                   case AIConst.parallel_gegner_kurs90:
                                       Manoever_Ruderlage_drehe_auf_Kurs(std_ruderlage, AIMath.norm(gegnerkurs + 90, 360), nSPD, nTrack);
                                       msgparallelkurs= "/Pg90-^";
                                       break;
                                   case AIConst.parallel_gegner_kurs270:
                                       Manoever_Ruderlage_drehe_auf_Kurs(std_ruderlage, AIMath.norm(gegnerkurs + 270, 360), nSPD, nTrack);
                                       msgparallelkurs= "/Pg270-^";
                                       break;
                               }
                               int iparallelkurs = nTrack.ListeSPD.Count;//also 1 weiter
                               
                               foreach (double parallel_parameter in lparallel_parameter) //.............................................................
                               {
                                   if (nTrack.ListeSPD.Count>iparallelkurs) nTrack.ListeSPD.RemoveRange(iparallelkurs, nTrack.ListeSPD.Count-iparallelkurs);
                                   nSPD = nTrack.ListeSPD[^1].Copy(); nSPD.Ruderlage = 0; nSPD.Winkelv = 0;
                                   
                                   switch(parallel_bis) 
                                   {
                                       case AIConst.parallel_bis_zeit:
                                           Manoever_Fahre_weiter_Zeit(parallel_parameter, nSPD, nTrack);
                                           msgparallel= parallel_parameter+"s";
                                           break;                                           
                                       case AIConst.parallel_bis_distanz:
                                           Manoever_Fahre_weiter_Distanz(parallel_parameter, nSPD, nTrack);
                                           msgparallel= parallel_parameter+"m";
                                           break;                                           
                                       case AIConst.parallel_bis_gegner_peilung:
                                           Manoever_peile_Gegner_min(parallel_parameter, Gegner, index_iTrack, nSPD, nTrack);
                                           msgparallel= "Gp"+parallel_parameter+"°";
                                           break;
                                       case AIConst.parallel_bis_gegner_achtern:
                                           Manoever_peile_Gegner_achtern(parallel_parameter, Gegner, index_iTrack, nSPD, nTrack);
                                           msgparallel= "ga"+parallel_parameter+"a°";
                                           break;
                                       case AIConst.parallel_bis_gegner_voraus:
                                           Manoever_peile_Gegner_voraus(parallel_parameter, Gegner, index_iTrack, nSPD, nTrack);
                                           msgparallel= "gv"+parallel_parameter+"a°";
                                           break;
                                       
                                       case AIConst.parallel_bis_gegner_Abstand:
                                           Manoever_am_Gegner_vorbei(parallel_parameter, Gegner, index_iTrack, nSPD, nTrack);
                                           msgparallel= "g"+parallel_parameter+"m";
                                           break;
                                   }
                                   
                                   int iparallel = nTrack.ListeSPD.Count;//also 1 weiter
                                   
                                   double bew_parallel= Bewertung_Track_Areas(nTrack, Gegner.ich, ientfernen, iparallel-1);
                                   if (bew_parallel<= -AIConst.penalty_NOGO) 
                                       continue;
                                   
                                   
                                   
                                   foreach (double winkel_zurueck in lwinkel_zurueck) //.............................................................
                                   {
                                       if (nTrack.ListeSPD.Count>iparallel) nTrack.ListeSPD.RemoveRange(iparallel, nTrack.ListeSPD.Count-iparallel);
                                       nSPD = nTrack.ListeSPD[^1].Copy(); nSPD.Ruderlage = 0; nSPD.Winkelv = 0;
                                      
                                       zaehler++;
                                       if (zaehler == 48 || zaehler==48)
                                       {
                                       }
                                       
                                       index_ende_rueckkehr =verfolge_Plan(Plan, nSPD, ref t_aufTrack, winkel_zurueck, nTrack); //Rest des GTracks
                                       msg +="/}"+ winkel_zurueck+"°";

                                       nTrack.name = "[" + zaehler + "]" + msgstart+"#" + msgfahrtstufe + msgdrehen + msgentfernen + msgparallelkurs + msgparallel + "/}" + winkel_zurueck + "°";
                                       nTrack.beschreibung = nTrack.name.Replace("/", "\n");
                                       Plan.counter = plan_counter;
                                       Plan.warte_bis = plan_warte_bis;
                                       
                                       bew =  Bewertung_Track_Areas(nTrack, Gegner.ich, iparallel,index_ende_rueckkehr-1);
                                       if (bew <= -AIConst.penalty_NOGO) continue;
                                       
                                       bew = bew_entfernen+ bew_parallel+bewertungbonus;
                                       bew += Bewertung_Track_Ende(nTrack, Gegner.ich, index_beginn_manoever, index_ende_rueckkehr-1);
                                       
                                       nTrack.name += "(*)=" + bew.ToString("0.00");
                                       nTrack.itmp = index_iTrack;
                                       nTrack.expiry_time = curr_time + AIglobal.deletetrackoptionsafter;
                                       
                                   
                                       Opt_Tracks.Add(nTrack.copy());
                                   

                                       if (bew > max_bew)
                                       {
                                           max_Track = nTrack.copy();
                                           max_bew = bew;
                                           max_t = t_aufTrack;
                                       }

                                   } //next winkel_zurueck 
                                   
                               } //next parallel_parameter
                               
                           } //next parallel_kurs
                           
                       }  //next entfernen_parameter
                           
                   } //next drehen_parameter
                   
               } //next_fahrtsufe
               
              
           } //end Ausweichraute
           
          
       }

       double tloeschen=-1d;
       private bool bschonangezeigt = false;
       
       public void display_OptionenAusweichmanoever()
       {
           
           if (busy_tAusweichmanoever) return;
           if (Opt_Tracks.Count == 0) return;
           CTrack bestTrack=null;
           //time fix von martin
           double tt = _scenarioInterface.ScenarioTime;
           List<CTrack> todelete = new List<CTrack>();
           bool schonangezeigt = false;
           
           foreach (CTrack nTrack in Opt_Tracks)
           {
               if (nTrack.expiry_time < tt)
               {
                   todelete.Add(nTrack);
                   if (nTrack.displayed) nTrack.Map_Entferne();
               }
               else
               {
                   if (nTrack.displayed == false)
                   {
                       nTrack.Map_Anzeige(30, nTrack.name.Substring(0, 1) == "!"?Color.yellow:Color.grey, nTrack.itmp, true);
                       nTrack.displayed=true;
                       if (sortedlistKritischeSituationen.Count>0 && schonangezeigt==false)
                       {
                           schonangezeigt = true;
                           CCPA nextCPA = sortedlistKritischeSituationen[0];
                           string msgCPA = "CPA " + name + " vs " + nextCPA.gegnername +
                                           "\ndist:" + (nextCPA._cpa_dist / sm).ToString("0.00") + "sm" +
                                           "\nkrit Entf:" + (nextCPA._krit_dist / sm).ToString("0.00") + "sm" +
                                           "\nZeit:" + (nextCPA._cpa_zeit).ToString("0") + "s";
                           AIMap.Punkt(nextCPA.cpa_SPD1.lat, nextCPA.cpa_SPD1.lon, 4d, Color.green,msgCPA);
                           AIMap.Punkt(nextCPA.cpa_SPD2.lat, nextCPA.cpa_SPD2.lon, 4d, Color.green,msgCPA);
                       }
                   }
               }
           }
           foreach (CTrack nTrack in todelete)  Opt_Tracks.Remove(nTrack);
                  
          
           
       }
       
       //wird verwendet, um auch Gegnerfahrzeuge zum Update ihrer listKritischeSituationen zu triggern
       public void update_meine_listKritischeSituationen(int gegnerindex=-1)
       {
           try
           {
               Berechne_listKritischeSituationen(Track, current_Track_index, ref listKritischeSituationen, gegnerindex);
           }
           catch
           {
               
           }
           
       }
       
       
       //berechnet die CPAs ggü allen Fzg : List<CCPA>[Gegner]
       // nur wenn CPAs < kritische_Entf
       //
       //updated eine Liste der Gegner sortiert nach Zeit des Auftretens kritischer Situationen
       public void Berechne_listKritischeSituationen(CTrack iTrack, int ab_index, ref List<CCPA>[] ilistKritischeSituationen, int gegnerindex=-1)
       {
           
           double dist=-1;
           double dist_min = 999999.9;
           double krit_Entf = -1;
           bool bnahbereichslage = false;
           string msg = "";
           double itime = iTrack.SPD(ab_index).timestamp;
           double old_peilung = -999;
           CCPA KritischeSituation=null;
           
           CShipPhysicalData iSPD, gSPD;
           bool bNogo_zw_Kontakten = false;
           
           if (ilistKritischeSituationen==null) ilistKritischeSituationen = new List<CCPA>[AIglobal.Fahrzeuge.Count];
           
           for(int g=0;g<AIglobal.Fahrzeuge.Count;g++)
           {
               if (gegnerindex >= 0) g = gegnerindex; //nur der eine Index
               
               CFzg Gegner = AIglobal.Fahrzeuge[g];if (Gegner.name == name) continue;
               bnahbereichslage = false;
               ilistKritischeSituationen[g] = new List<CCPA>();
               List<double> distances = new List<double>();
               
               
               for (int i = ab_index; i < iTrack.ListeSPD.Count-1; i++)
               {
                   
                   iSPD = iTrack.SPD(i);
                   if (Gegner.Track.ListeSPD.Count <= 1) continue;
                   gSPD = Gegner.Track.SPD(i);
                   krit_Entf = Entf_Ausweichen;//kritische_Entf(iSPD,Gegner,gSPD,AIConst.cOrtung_Sicht, ref msg);
                   dist = AIMath.Dist_XZ(iSPD.x, iSPD.z, gSPD.x, gSPD.z);

                   //liegt eine NOGO-are zwischen den beiden Schiffen?->falls ja, dann ist die Distanz irrelevant
                   bNogo_zw_Kontakten = false;
                   if (dist <= krit_Entf && (bnahbereichslage == false || dist<dist_min)) bNogo_zw_Kontakten = NOGO_Area_zwischen_Punkten(iSPD.x, iSPD.z, gSPD.x, gSPD.z);
                   
                   
                   distances.Add(dist);
                   if (dist <= krit_Entf && bnahbereichslage == false && bNogo_zw_Kontakten==false) //erstes Auftreten Nahbereichslage
                   {
                       bnahbereichslage = true;
                       KritischeSituation = new CCPA(iSPD,gSPD,null,null); //CPA noch unbekannt
                       ilistKritischeSituationen[g].Add(KritischeSituation); //zur Liste hinzufügen
                       KritischeSituation._entry_zeit = iSPD.timestamp;
                       KritischeSituation.gegnerindex = g;
                       KritischeSituation.gegnername = Gegner.name;
                       KritischeSituation._krit_dist = krit_Entf;
                       dist_min = 999999.9;
                   }

                   if (bnahbereichslage && dist > krit_Entf )  //Ende NNahbereichslage
                   {
                       KritischeSituation.exit_zeit = iSPD.timestamp;
                       bnahbereichslage = false;
                       //wenn die kritische Situation gleich zu Anfang auftritt und es dann nur besser wird, ist es keine kritische SItuation
                       if (KritischeSituation._entry_zeit == KritischeSituation._cpa_zeit)
                       {
                           ilistKritischeSituationen[g].RemoveAt(ilistKritischeSituationen[g].Count-1);
                       }
                           
                   }
                   
                   if (bnahbereichslage)  //noch weitermachen, um CPA zu berechneen
                   {
                           if (dist < dist_min && bNogo_zw_Kontakten==false)
                           {
                               dist_min = dist;
                               KritischeSituation.cpa_SPD1 = iSPD;
                               KritischeSituation.cpa_SPD2 = gSPD;
                               KritischeSituation._cpa_zeit = iSPD.timestamp;
                               KritischeSituation._cpa_dist = dist;
                           }
                   }
                   
               }
               if (gegnerindex >= 0) break; //nur der eine Index
           }
           
           sortedlistKritischeSituationen.Clear();
           for(int g=0;g<AIglobal.Fahrzeuge.Count;g++)
           {
               CFzg Gegner = AIglobal.Fahrzeuge[g];if (Gegner == this) continue;
                          
               if (Gegner.name == name) continue;
               foreach (CCPA KS in listKritischeSituationen[g])
               {
                  if (KS.cpa_zeit() >= itime) 
                  {
                      sortedlistKritischeSituationen.Add(KS);
                      break; //hat CPA noch vor sich
                  }                 
               }
           }

           
           if (sortedlistKritischeSituationen.Count > 1)
           {
               //sortedlistKritischeSituationen.Sort((CCPA slKS1, CCPA slKS2) => slKS1._entry_zeit.CompareTo(slKS2._entry_zeit));
               sortedlistKritischeSituationen.Sort(new CPAComparer());
           }

           
          

       }

       public void Berechne_listKritischeSituationen2(CTrack iTrack, int ab_index, ref List<CCPA>[] ilistKritischeSituationen, int gegnerindex=-1)
       {
           
           double dist=-1;
           double dist_min = 999999.9;
           double krit_Entf = -1;
           bool bnahbereichslage = false;
           string msg = "";
           double itime = iTrack.SPD(ab_index).timestamp;
           
           CCPA KritischeSituation=null;
           int anzahl_updated = 0;
           CShipPhysicalData iSPD, gSPD;
           bool bNogo_zw_Kontakten = false;
           
           if (ilistKritischeSituationen==null) ilistKritischeSituationen = new List<CCPA>[AIglobal.Fahrzeuge.Count];//für ersten AUfruf, initialisieren
           
           for(int g=0;g<AIglobal.Fahrzeuge.Count;g++)
           {
               if (gegnerindex >= 0) g = gegnerindex; //nur der eine Index
               
               CFzg Gegner = AIglobal.Fahrzeuge[g];
               if (Gegner.name == name) continue;
               if (ilistKritischeSituationen[g] != null) continue;   //Null setzen um update zu erzwingen
               bnahbereichslage = false;
               
               ilistKritischeSituationen[g] = new List<CCPA>();
               anzahl_updated++;
               List<double> distances = new List<double>();//zum debuggen
               
               
               for (int i = ab_index; i < iTrack.ListeSPD.Count-1; i++)
               {
                   
                   iSPD = iTrack.SPD(i);
                   if (Gegner.Track.ListeSPD.Count <= 1) continue;
                   gSPD = Gegner.Track.SPD(i);
                   krit_Entf = Entf_Ausweichen;//kritische_Entf(iSPD,Gegner,gSPD,AIConst.cOrtung_Sicht, ref msg);
                   dist = AIMath.Dist_XZ(iSPD.x, iSPD.z, gSPD.x, gSPD.z);

                   //liegt eine NOGO-are zwischen den beiden Schiffen?->falls ja, dann ist die Distanz irrelevant
                   bNogo_zw_Kontakten = false;
                   if (dist <= krit_Entf && (bnahbereichslage == false || dist<dist_min)) bNogo_zw_Kontakten = NOGO_Area_zwischen_Punkten(iSPD.x, iSPD.z, gSPD.x, gSPD.z);
                   
                   
                   distances.Add(dist);
                   if (dist <= krit_Entf && bnahbereichslage == false && bNogo_zw_Kontakten==false) //erstes Auftreten Nahbereichslage
                   {
                       bnahbereichslage = true;
                       KritischeSituation = new CCPA(iSPD,gSPD,null,null); //CPA noch unbekannt
                       ilistKritischeSituationen[g].Add(KritischeSituation); //zur Liste hinzufügen
                       KritischeSituation._entry_zeit = iSPD.timestamp;
                       KritischeSituation.gegnerindex = g;
                       KritischeSituation.gegnername = Gegner.name;
                       KritischeSituation._krit_dist = krit_Entf;
                       dist_min = 999999.9;
                   }

                   if (bnahbereichslage && dist > krit_Entf )  //Ende NNahbereichslage
                   {
                       KritischeSituation.exit_zeit = iSPD.timestamp;
                       bnahbereichslage = false;
                       //wenn die kritische Situation gleich zu Anfang auftritt und es dann nur besser wird, ist es keine kritische SItuation
                       if (KritischeSituation._entry_zeit == KritischeSituation._cpa_zeit)
                       {
                           ilistKritischeSituationen[g].RemoveAt(ilistKritischeSituationen[g].Count-1);
                       }
                           
                   }
                   
                   if (bnahbereichslage)  //noch weitermachen, um CPA zu berechneen
                   {
                           if (dist < dist_min && bNogo_zw_Kontakten==false)
                           {
                               dist_min = dist;
                               KritischeSituation.cpa_SPD1 = iSPD;
                               KritischeSituation.cpa_SPD2 = gSPD;
                               KritischeSituation._cpa_zeit = iSPD.timestamp;
                               KritischeSituation._cpa_dist = dist;
                           }
                   }
                   
               }
               if (gegnerindex >= 0) break; //nur der eine Index
           }
          
           if (anzahl_updated==0) return;   //wenn  nichts geupdated wurde, muß auch nichts sortiert werden;
           
           sortedlistKritischeSituationen.Clear();
           for(int g=0;g<AIglobal.Fahrzeuge.Count;g++)
           {
               CFzg Gegner = AIglobal.Fahrzeuge[g];if (Gegner == this) continue;
                          
               if (Gegner.name == name) continue;
               foreach (CCPA KS in listKritischeSituationen[g])
               {
                  if (KS.cpa_zeit() >= itime) 
                  {
                      sortedlistKritischeSituationen.Add(KS);
                      break; //hat CPA noch vor sich
                  }                 
               }
           }

           
           if (sortedlistKritischeSituationen.Count > 1)
           {
               //sortedlistKritischeSituationen.Sort((CCPA slKS1, CCPA slKS2) => slKS1._entry_zeit.CompareTo(slKS2._entry_zeit));
               sortedlistKritischeSituationen.Sort(new CPAComparer());
           }

           
          

       }
       public bool NOGO_Area_zwischen_Punkten(double x1,double y1, double x2,double y2)
       {
           foreach (CArea Area in AIglobal.Gebiete)
           {
               if (Area.typ == AIConst.cAreaTyp_NOGO)
               {
                   if (Area.Gerade_schneidet(x1,y1,x2,y2))
                   {
                       return true;
                   }
               }
           }
           return false;
       }


       private NauticObject tmpNO = null;
       public void display_Fahrzeug(double itime)
       {
           if (AIglobal.bsuppressmapoutput) return;
           
           int index = get_current_trackindex(itime,PresentationTrack(0));
           CShipPhysicalData iSPD = Track.ListeSPD[index].Copy();
           if (name=="Fzg2" && iSPD.KdW>270 )
           {}

           bool bmuss_ausweichen = false; 
           bool bmuss_kurshalten = false;
           bool bamausweichen = false;
           int Fzgcount = AIglobal.Fahrzeuge.Count;
           //for (int g = 0; g < Fzgcount; g++) if (this.bausweichpflichtig[g]) {bmuss_ausweichen = true;break;}
           //for (int g = 0; g < Fzgcount; g++) if (this.bkurshaltepflichtig[g]) {bmuss_kurshalten = true;break;}
           //for (int g = 0; g < Fzgcount; g++) if (this.bam_ausweichen[g]) {bamausweichen = true;break;}
              //auswweichmanoever_ingang
           for (int g = 0; g < Fzgcount; g++) if (this.ausweichmanoever_ingang[g]>0) {bamausweichen = true;break;}
           
           
           //this.ObjSC.Data.m_Color = bmuss_ausweichen ? Color.red : (bmuss_kurshalten ? Color.blue : Color.black);//ausweichpflichtig,Kurshaltepflichtig/weder noch
           //this.ObjSC.Data.m_Color = Color.red;
           this.ObjSC.Data.ObjectName = name;
           //if (bamausweichen)  this.ObjSC.Data.m_ObjectName += " weicht_aus";
           //if (bmuss_kurshalten)  this.ObjSC.Data.m_ObjectName += " kurshaltepflichtig";
          
           
           //this.ObjSC.Data.m_Position=new Position(iSPD.lat,iSPD.lon);                //Position
          /// this.ObjSC.Data.m_Direction = (float)90;          // (float) iSPD.KdW; //bearing
           
           /*
            * if (istManuell)
           
           {
               if (tmpNO) AIglobal.m_ObjSpawnerSO.DeleteNauticObject(tmpNO);
               tmpNO=AIMap.Punkt(iSPD.lat, iSPD.lon, 5, Color.green);
           }
            */
           ObjSC.Data.Debug2 = "R:" + iSPD.Ruderlage + "/F:" + iSPD.Fahrstufe.ToString("0.0")+ "/KdW:" + iSPD.KdW.ToString("0.0")+ "/FdW:" + (iSPD.FdW/kn).ToString("0.0")+"kn";
           ObjSC.Data.Debug3 = "i:"+index.ToString("N0")+"/t:" + iSPD.timestamp.ToString("0.0");
          
           ObjSC.Data.EcdisColor = busy?Color.black:farbe;
           //this.ObjSC.Data.m_Velocity.x =(float) iSPD.Ruderlage;
           //this.ObjSC.Data.m_Velocity.y =(float) iSPD.Fahrstufe;
           //this.ObjSC.Data.m_Velocity.z =(float) iSPD.FdW;
       }
       
       int get_current_trackindex_lastindex=0;
       double get_current_trackindex_lasttime = 0d;
       public int get_current_trackindex(double itime, CTrack iTrack)
       {
           if (itime > get_current_trackindex_lasttime)
           {
               for (int i = get_current_trackindex_lastindex; i < iTrack.ListeSPD.Count; i++) //rücke vor zum nächsten zur Zeit passenden Index
               {
                   if (iTrack.ListeSPD[i].timestamp >= itime)
                   {
                       get_current_trackindex_lastindex = i;
                       get_current_trackindex_lasttime = itime;
                       return get_current_trackindex_lastindex;
                   }
               }
               return iTrack.ListeSPD.Count-1;
           }
           else
           {
               if (itime == get_current_trackindex_lasttime)
                   return get_current_trackindex_lastindex;
               
               for (int i = get_current_trackindex_lastindex; i > 1; i--) //rücke vor zum nächsten zur Zeit passenden Index
               {
                   if (iTrack.ListeSPD[i-1].timestamp < itime && iTrack.ListeSPD[i].timestamp >= itime)
                   {
                       get_current_trackindex_lastindex = i;
                       get_current_trackindex_lasttime = itime;
                       return get_current_trackindex_lastindex;
                   }
               }
               return -1;
           }
       }

       public CTrack PresentationTrack(double itime)
       {
           return Track;
       }
       
       //Wird aus AI_VTController aufgerufen bei Update()
       //ein einziger Zeitstep vorrücken im Track
         // Fahrzeug neu darstellen  
         // Ausweichnotwendigkeit eruieren (-> ausweichpflichtig(...))
         // --falls notwendig Track mit bestmöglichem Ausweichmanöver (-> tAusweichmanoever(...) ) neu berechnen 
         //das sind quasi static variablen wie VB6
         public int current_Track_index=0; //zeigt an, welcher Index im eigenen Track in simulate_track_update gerade aktuell ist
         string ausgabe="";
         private int cc = 0;
         private double maxzeit = 0;
         public int kritSitES_updaten=-1;  //Fahrzeugindex, zu dem kritische Situation geupdated werden muss
         //ende static Variablen
       
       //  public void simulate_track_update(CTrack iTrack, double itime)
       
       
       public async Task simulate_track_update(CTrack iTrack, double itime)
       {
           
               //StartMagic();
               AIglobal.FzgUpdate.Add(ich+"#"+name+"#"+itime);
               //Debug.Log(ich + "#" + name + "#" + itime);
               AIglobal.busy = true;
               this.busy = true;
               CMsgBox MsgBox;
               string msg = "";
               int g = -1; //gegnerindex, wenn immernoch -1, dann kein Gegner
               bool btrack_updated = false;
               bool bkritischesituation_updaten = false;

               Berechne_listKritischeSituationen2(iTrack, current_Track_index, ref listKritischeSituationen);//alle
               
               //Track erstellen, nur beim ersten Mal
               if (/*bam_ausweichen*/ ausweichmanoever_ingang == null)
               {
                   simulate_track_initialization(iTrack); //nur 1x beim 1.Fzg für alle Fzge
                   //MsgBox = new CMsgBox("AI erste Trackberechnung");
               }

               //zum Index im Track vorrücken, dr dem aktuellen Zeit itime entspricht
               int i; // = iTrack.index(itime);     //index, der dem Zeitstep=itime
               for (i = current_Track_index; i < iTrack.ListeSPD.Count; i++) //rücke vor zum nächsten zur Zeit passenden Index
               {
                   if (iTrack.ListeSPD[i].timestamp >= itime)
                   {
                       current_Track_index = i;
                       break;
                   }
               }


               if (i == 0) goto ende;

               /*
               if (kritSitES_updaten >= 0)   //wenn das ES seinen Track geändert hat, updated es aus Zeitgründen nicht die kritischen Situationen der anderen
               {                             //sondern setzt in kritSitES_updaten seinen index.
                   update_meine_listKritischeSituationen(kritSitES_updaten); //kann wegfallen, wenn ES dbeim Gegner auf null setzt
                   kritSitES_updaten=-1;
               }
               */
               
               
               CShipPhysicalData iSPD = iTrack.SPD(i);
               if (iSPD == null) AIglobal.Fehler("iSPD == null");

          
               
               //List<int, double> gefaehrliche_Gegner = new List<int, double>;

               // -> Notwendigkeit eines Ausweichmanövers mit allen anderen Verkehrsteilnehmern eruieren
               //
               if (name == "FzgBlau")
               {
               }
                                                    //zu prüfende Verkehrteilnehmer sortiert nach Zeitpunkt Auftreten kritische Situation
               foreach (CCPA KritischeSituation in sortedlistKritischeSituationen)
               {
                   g = KritischeSituation.gegnerindex;
                   CFzg Gegner = AIglobal.Fahrzeuge[g];
                   CShipPhysicalData gSPD = Gegner.Track.SPD(i);

                   double dist = AIMath.Dist_XZ(iSPD.x, iSPD.z, gSPD.x, gSPD.z);
                   double cpa;
                   bool breagieren = true;
                   double ausweichmanoever_ingang_jetzt = ausweichmanoever_ingang[g];
                    
                   //CPA schon in der Vergangenheit,  (bis reagiert werden kann), nicht mehr gefährlich
                   if (KritischeSituation.cpa_zeit() < (itime+AIglobal.reactionsteps*AIConst.delta_t))
                   {
                       listKritischeSituationen[g] = null;
                       bkritischesituation_updaten = true;
                       continue; //  ----^^
                   }
                               
                   if (dist > Entf_Ausweichen) //Gegner zu weit weg, hat Zeit bis später
                   {
                       breagieren = false;
                       ausweichmanoever_ingang[g] = 0;
                       continue;//  ----^^
                   }

                   if (ausweichmanoever_ingang[g] == 0)  //Neubewerten Ausweichpfloicht nur wenn kein Manoever in Gang
                   {
                       if (name == "FzgGruen" && Gegner.name == "FzgRot")
                       {
                       }
                       
                       
                       bausweichpflichtig[g] = ausweichpflichtig(iSPD, Gegner, gSPD, AIConst.cOrtung_Sicht,
                                                                ref msgregel[g], ref msgmanoever[g]); //HIER WIRD DIE AUSWEICHPFLICHT BESTIMMT 
                              
                              
                   }
                           
                           
                   double krit_Entf = kritische_Entf(iSPD, Gegner, gSPD, AIConst.cOrtung_Sicht, ref msg); //benäötige Ausweichregel!

                   
                   //if (KritischeSituation._krit_dist != krit_Entf)  //kritische Entfernung hat sich geändert, zB weil Gegner Fahrt aufgenommen hat
                   //{
                   //    bkritischesituation_updaten = true;
                   //    continue;
                   //}

                   if (KritischeSituation._cpa_dist > krit_Entf) //Gegner kommt nicht gefährlich nahe
                   {                                             //bleibt drin, weil ausweichen muss man nicht, wenn Gegner zB ohne Fahrt
                       breagieren = false;
                       continue;
                   }

                   if (bkurshaltepflichtig[g]) //Ausweichpflicht geht vor Kurshaltepflicht
                   {
                       breagieren = false;
                   } //muss ich diesem Gegner gegenüber den Kurs halten

                           
                   switch (true)
                   {
                       case  true when  (dist<krit_Entf * faktor_LetztesManoever):     //Notfallmanöver        
                           breagieren = (ausweichmanoever_ingang_jetzt!=faktor_LetztesManoever); //in jedem Fall ausser  schon am ausweichen
                           if (breagieren)
                           {
                               ausweichmanoever_ingang[g] = faktor_LetztesManoever;
                               msgregel[g] = "8iii";
                               bausweichpflichtig[g] = ausweichpflichtig(iSPD, Gegner, gSPD, AIConst.cOrtung_Sicht,
                                   ref msgregel[g], ref msgmanoever[g],true);
                           }
                               ausweichmanoever_ingang[g] = faktor_LetztesManoever;
                           break;
                       case true when  (dist<krit_Entf * faktor_Korrektur):     // korrigierendes Ausweichmanöver
                           breagieren = bausweichpflichtig[g] && (ausweichmanoever_ingang_jetzt==0d || ausweichmanoever_ingang_jetzt==1); //nur wenn ausweichpflichtig  und nicht schon ausweicht
                           if (breagieren) 
                               ausweichmanoever_ingang[g] = faktor_Korrektur;
                           break;    
                       case true when  (dist<(Entf_Ausweichen*.9+krit_Entf*0.1d) * 1.0d):    // normales Ausweichmanöver
                           breagieren = bausweichpflichtig[g] && (ausweichmanoever_ingang_jetzt==0d);//nur wenn ausweichpflichtig und nicht schon ausweicht
                           if (breagieren) 
                               ausweichmanoever_ingang[g] = 1.0d;
                           break;    
                   }
                           
                            
                   if (ausweichmanoever_ingang_jetzt == ausweichmanoever_ingang[g])
                   {
                       breagieren = false;
                       continue;
                   }   
                           
                           
                   //hier fehlt noch: der Gegner ist manuell und dreht ...

                   if (breagieren) //Ausweichmanöver fahren 
                   {
                       //if (Time.timeScale > 4f) Time.timeScale = 1f;
                       tmp_calc_time = AIglobal.m_channel_map.ScenarioTime;
                       
                       if (name == "FzgBlau")
                       {
                       }
                       if (ausweichmanoever_ingang[g] == faktor_LetztesManoever) msg="Letztes Man.:";
                       if (ausweichmanoever_ingang[g] == faktor_Korrektur) msg="Korrekturma.:";
                       if (ausweichmanoever_ingang[g] == 1.0d) msg = "Ausweichman.:";

                       msg += Gegner.name + " , §" + (bausweichpflichtig[g] ? msgregel[g] : "--") + "\n";
                       msg +="ggw dist=" + dist.ToString("N0")+"m, ";

                       
                       //if (bausweichpflichtig[g]) msg += " ,ausweichpfl.,"; else msg+=" ,nicht ausweichpfl.,";
                       ObjSC.Data.Debug1 += "-------------------------\n";
                       ObjSC.Data.Debug1 += msg+"\n(Man. gem. "+msgmanoever[g]+")"; ///+"\n"kommt bei Bewertung orig Track
                       //NEUBERCHNEN DES TRACKS
                       //der TRack ist eine Lösung für alle Gegner, andere Lösungen müssen erstmal nicht mehr berechnet werden in diesem Schritt
                       AIglobal.reset_timescale = true;
                       Opt_Tracks.Remove(iTrack);
                       //iTrack.Map_Entferne();
                       tausweichen_bis[g] = tAusweichmanoever(msgmanoever[g], ref iTrack, i, Gegner,KritischeSituation.cpa_zeit()); //!!! UPDATE iTrack , liefert den Zeitpunkt, wenn zurück auf Planroute !!!
                       //t_aufTrack[g]= Zeit wann ich beim ausweichen gegenüber Gegner g wieder auf Plan-Track bin
                       
                       //iTrack.Map_Anzeige(30, Color.yellow, 0, true);
                       try
                       {
                           ObjSC.Data.Debug1 +="Lsg:"+iTrack.name.Split('#')[1]+"\n";
                       }
                       catch (Exception e)
                       {
                           
                       }
                       
                           
                       try
                       {
                           if (Track.ListeSPD[get_current_trackindex_lastindex].lat != iTrack.ListeSPD[get_current_trackindex_lastindex].lat)
                           {
                               AIglobal.Fehler("Fehler: neuer Track zu spät ermittelt");
                               AIglobal.Fehler("Entscheidung calles:" + itime + "--- Display called:" + get_current_trackindex_lasttime);
                               //MsgBox = new CMsgBox("Fehler: neuer Track zu spät ermittelt");
                           }

                       }
                       catch
                       {
                       }

                       Track = iTrack;
                           
                       //MsgBox=new CMsgBox("Entscheidung calles:" + itime + "\n --- Display called:" +
                       //                   get_current_trackindex_lasttime);
                            
                       btrack_updated = true;

                       ausweichmanoever_ingang[g] = ausweichmanoever_ingang[g] ; //Manöver im Gang, keine ständige Neubewertung
                       if (msgregel[g]!="14") Gegner.bkurshaltepflichtig[ich] = true; //wenn ich ausweichen muss, ist der Gegner mir gegenüber kurshaltepflichtig

                       //hier noch: alle, die mir bereits ausweichen, bam_ausweichen[ich] beenden, damit neu bewertet wird
                       tmp_calc_time = AIglobal.m_channel_map.ScenarioTime-tmp_calc_time;
                       ObjSC.Data.Debug4 = tmp_calc_time.ToString("0.000")+" RZ";
                   }

                   if (ausweichmanoever_ingang[g] >0) //ist noch ausweichpflichtig, gilt die Pflicht noch?
                   {
                       //Ende Pflicht 
                       //(1) Abstand>Nahbereich && AbleitungCPA>0
                       if (itime > tausweichen_bis[g] && dist > Entf_Ausweichen*faktor_Korrektur && (dist - idist_old[g]) > 0 && idist_old[g] != 0d && KritischeSituation.krit_zeit() < itime)
                       {
                           msg += name + "Ausweichmanöver mit " + Gegner.name + "beendet" + " Step=" + i + "\n";
                           ausweichmanoever_ingang[g] = 0;
                           bletzterAugenblick[g] = false;
                           bkurshaltepflichtig[g] = false;
                           Gegner.bausweichpflichtig[ich] = false;
                           bausweichpflichtig[g] = false;
                       }
                   }

                   idist_old[g] = dist;
                   if (btrack_updated)
                       break; //der neue Track ist eine Lösung für alle Gegner, andere Lösungen nicht mehr nötig in diesem Schritt
               } //next foreach (CCPA KritischeSituation in sortedlistKritischeSituationen)

               if (bkritischesituation_updaten)
               {
                   //update_meine_listKritischeSituationen();//alle updaten, weil CPA schon in Vergangenheit, oben Null setzen reicht
                   bkritischesituation_updaten = false;
               }
                   
                       
                       
               if (btrack_updated)
               {
                   //wenn ich ausweiche, verändere ih meinen Track, daher NEUBRECHNUNGEN
                   //update_meine_listKritischeSituationen(); //                           (neu) ... alle auf null setzeb
                   listKritischeSituationen = null;
                   
                   for (int ag = 0; ag < AIglobal.Fahrzeuge.Count; ag++) //ausweichpflichtige Gegner sollen ihren Track neu bewerten
                   {
                       if (ag == g) continue;
                       CFzg aGegner = AIglobal.Fahrzeuge[ag];
                       if (aGegner == this) continue;
                       if (aGegner.istManuell) continue;
                       aGegner.bam_ausweichen[ich] = false; //damit müssen andere neu ihre Ausweichpflicht mir gegenüber prüfen 
                               
                       aGegner.tausweichen_bis[ich] = -1;
                       //aGegner.update_meine_listKritischeSituationen(ich); // andere müssen kritische Situationen mir gegenüber neu berechnen
                                                                           //null setzen reicht    //alle auf null setzen
                       aGegner.listKritischeSituationen[ich] = null;
                   }
               }
               

           ende:
           
           
           this.busy = false;
           AIglobal.busy = false;
           return;
               ;
       }//end function testsimulate_track

       private double inputruderlage_alt = -999; //damit beim ersten Durchlauf ein Track erstellt wird
       private double inputfahrtstufe_alt = -999;
       private double inputkursvorgabe_alt = -999;
       private double last_ES_Track_update;
    
       //Eigenschiff Trackupdate
       public void ES_simulate_track_update()
       {
           if (AIglobal.m_ObjSpawnerSO.SelectedObject == null) AIglobal.m_ObjSpawnerSO.SelectNauticObject(ObjSC); //ist noch nicht so weit
           if (Track.ListeSPD.Count == 0)
           {
               ObjSC.Data.RuderValue   =(float) firstSPD.Ruderlage;
               ObjSC.Data.ThrustValue  =(float) firstSPD.Fahrstufe;
               ObjSC.Data.ActualCourse   =(float) firstSPD.KdW;
               ObjSC.Data.ActualVelocity =(float) firstSPD.FdW;
               ObjSC.Data.WantedCourse   =(float) firstSPD.KdW;
           } 
           
           double inputruderlage = (double) AIglobal.m_ObjSpawnerSO.SelectedObject.Data.RuderValue;
           double inputfahrtstufe = (double) AIglobal.m_ObjSpawnerSO.SelectedObject.Data.ThrustValue;
           double inputkursvorgabe = (double) AIglobal.m_ObjSpawnerSO.SelectedObject.Data.WantedCourse;
           double ruderlage=0d;
           
           //hat sich was an den Inputs vom Martin geändert ...
           if (inputfahrtstufe == inputfahrtstufe_alt && inputruderlage == inputruderlage_alt && inputkursvorgabe == inputkursvorgabe_alt) return; //TRack noch UptoDate

           Track?.Map_Entferne();
           //time fix von martin
           double itime = _scenarioInterface.ScenarioTime;
           
           int curr_index = get_current_trackindex(itime, Track);

           if (Track.ListeSPD.Count == 0)
           {
               Track.ListeSPD.Add(firstSPD);  //Track muss in jedem Fall berechnet werden
               curr_index = 0;
               
           }
           else
           {   //....                und wenn ja, ist es in Übereinstimmung mit dem Werten aus dem Track
               if (curr_index < Track.ListeSPD.Count && inputruderlage !=inputruderlage_alt)
               {
                   if (inputruderlage == Track.ListeSPD[curr_index].Ruderlage && inputfahrtstufe == inputfahrtstufe_alt)
                       return;

               }
           }
           
           
           
           
           CTrack nTrack = new CTrack();
           if (curr_index + AIglobal.ES_reactionsteps < Track.ListeSPD.Count)
           {
               nTrack.ListeSPD.AddRange(Track.ListeSPD.GetRange(0, curr_index  + AIglobal.ES_reactionsteps+1));
           }
           else
           {
               nTrack = Track;
           }
            
           CShipPhysicalData nSPD = nTrack.ListeSPD[^1].Copy();
           nSPD.Fahrstufe = inputfahrtstufe;  nSPD.Ruderlage = inputruderlage;
           if (inputruderlage != inputruderlage_alt && inputruderlage == 0)
           {
               inputkursvorgabe = nSPD.KdW;
               
           }
                      
           
           
           
           double kurs = nSPD.KdW;

           if (inputruderlage != inputruderlage_alt)
           {
               ruderlage = inputruderlage;
           }
           else
           {
               ruderlage = nSPD.Ruderlage;
           }
           
           
           if (inputkursvorgabe != inputkursvorgabe_alt && nSPD.Ruderlage == 0)
           {
               double diff_kurs =AIMath.Diff_Winkel(kurs, inputkursvorgabe); //<0 bb,>0 stb
               if (inputruderlage==0 && diff_kurs!=0) 
               {
                   ruderlage=(diff_kurs>0)?std_ruderlage:-std_ruderlage; //Stb oder Bb
               }
               
           }

           if (ruderlage != 0)
           {
               nSPD.KdW +=Math.Sign(nSPD.FdW) * (ruderlage / 1000d);//damit das Manöver nicht abgebrochen wird, weil der vorgegebene Kurs bereits anliegt in Manoever_Ruderlage_drehe_auf_Kurs
               //Manoever_Ruderlage_drehe_auf_Kurs(ruderlage, inputkursvorgabe, nSPD, nTrack);  geht nicht, weil 
               nSPD.Ruderlage = ruderlage;
               fahre_Kurs_bis(AIConst.cBedingung_Zielkurserreicht, -1,0, -1, -1, -1, -1,inputkursvorgabe, null, nSPD, nTrack);
               nSPD = nTrack.ListeSPD[^1].Copy(); nSPD.Ruderlage = 0; nSPD.Winkelv = 0;
           }
           
           Manoever_Fahre_weiter_bis_Ende(nSPD, nTrack); //hier vielleicht noch: wenn er in NOGO fährt, dann Richtung ändern 
           
           Track = nTrack;
           Track.Map_Anzeige(30, Color.green, nTrack.itmp, true);
           
           //Werte speichern als alt
           inputruderlage_alt = inputruderlage;
           inputfahrtstufe_alt = inputfahrtstufe;
           inputkursvorgabe_alt = inputkursvorgabe;

           //Änderung von Kurs oder Fahrt-neuer Track-neue kritische SItuationen
           foreach (CFzg aFzg in AIglobal.Fahrzeuge)
           {
               aFzg.kritSitES_updaten = ich;
           }
           
           //aGegner.update_meine_listKritischeSituationen(ich);
           //ich bin kurshaltepflichtig und habe ewtas verändert? Meldung! Flag setzen!
           // kurshaltepflichtig: einer der anderen Fahrzeuge hält mich für kurshaltepflichtig:
           // wenn Gegner.bamausweichen[ich] = false; UND 
           
       }
       
       int runnr=0;
       public async Task PerformMagicShow()
       {
               Debug.Log("Preparing for the magic show..."+runnr);
       
               Task trick1Task = PerformTrick("Card Trick");
               Task trick2Task = PerformTrick("Coin Trick");
       
               // While the tricks are being prepared, you can engage the audience
               Debug.Log("Engaging the audience with some jokes...");
       
               // Await the completion of each trick
               await trick1Task;
               Debug.Log("Card Trick performed!"+runnr);
       
               await trick2Task;
               Debug.Log("Coin Trick performed!"+runnr);
       
               Debug.Log("Magic show completed!"+runnr);
           }
           private async Task PerformTrick(string trickName)
               {
                   Debug.Log($"Preparing for {trickName}..."+runnr);
           
                   // Simulate the time it takes to set up the trick
                   await Task.Delay(9000); // 3 seconds
           
                   Debug.Log($"Performing {trickName}!"+runnr);
               }
           
               public async Task StartMagic( )
               {
                   runnr++;
                   Debug.Log("Task StartMagic" + runnr);
                   PerformMagicShow();
                   
                   
               }
       
       
   }//============================================C F z g======================================

