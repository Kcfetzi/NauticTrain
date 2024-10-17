using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Groupup;
using Unity.Mathematics;
using UnityEngine;

/* TO DO
 ... 1561 Zeilen ->1436->1298->1275->1216->1192->1154->1144
 * Manöver in Track füttern (Track Zeit->WP, sim_update->result in Track)
 * Routinen (Verfolge Track (Andrehpunkte) Return to Track,  Ausweichen, 30Grad bb, Aufstoppen etc) -1 Framework   -2.Implementation //Eintrag in vorgegebene Liste  mit Punkten+Informationen
 * Klasse: Wegpunkte: Punkt-Geschwindigkeit-(Wartezeit)->Start oder Ende   //dynamisch: Kurs
 * public Fzg HatVorfahrt(fzg:fzg1, fzg: fzg2){...}
 */

public class AI_VTController : MonoBehaviour 
{ 
   private AIInterface m_AI_VTChannelSO;
   [SerializeField] private Transform ShipSpawn;
   private static ObjectsInterface m_ObjSpawnerSO;
   public static ScenarioInterface m_channel_map;
   private static UI_RootInterface m_channel_ui;
   
   public static double lat_to_m;
   public static double lon_to_m;

   public static double Windrichtung = 271;
   
   public const double grad=Math.PI/180d; //RAD->GRAD
   public const double kn=0.514d;//0.514 kn->m/s
   public const double sm = 1852; //m
   
   
   /*
   public static List<CArea> Gebiete = new List<CArea>();
   public static List<CFzg> Fahrzeuge = new List<CFzg>();
   */
   private void Awake()
   {
       // Get own channel and subscribe
       m_AI_VTChannelSO = ResourceManager.GetInterface<AIInterface>();
       if (m_AI_VTChannelSO) { m_AI_VTChannelSO.IsActive = true; } else { Debug.Log("Could not subscribe to channel in m_AI_VTChannelSO"); }
       
       m_ObjSpawnerSO=ResourceManager.GetInterface<ObjectsInterface>();
       AIglobal.m_ObjSpawnerSO= m_ObjSpawnerSO;
       
       m_channel_map=ResourceManager.GetInterface<ScenarioInterface>();
       AIglobal.m_channel_map = m_channel_map;
      
       m_channel_ui=ResourceManager.GetInterface<UI_RootInterface>();
       AIglobal.m_channel_ui = m_channel_ui;
       
       AIMap.m_channel_ui = m_channel_ui;

       m_AI_VTChannelSO.OnInit_Szenario += init_szenario;

       //m_AI_VTChannelSO.OnUserInteractionStopped += Update_Track_Eigenschiff();
       //if (SceneLoader.Instance.PresetFullyLoaded)
       //    All_Services_Loaded();
       //else
       //    SceneLoader.Instance.OnActivePresetFullyLoaded += All_Services_Loaded;
   }

   

   public void init_szenario()   //her gehts los, wenn alles gestartet st   //war mal All_Services_Loaded
   {
       if (!m_channel_map || !m_ObjSpawnerSO || !m_channel_ui)  return;
       if (!m_channel_map.IsActive || !m_channel_ui.IsActive)  return;
       lon_to_m =m_channel_map.LonInMeters; lat_to_m =m_channel_map.LatInMeters;

       AIglobal.lon_to_m =m_channel_map.LonInMeters; 
       AIglobal.lat_to_m =m_channel_map.LatInMeters;
       
       //string test;
       //ServiceManager.Instance.GetChannel<PopupChannelSO>().InputTextPopup("eingabe", dummy);
       double t = 0, t_aufTrack=0;
       DateTime ti = DateTime.Now;
       TimeSpan tastru = (DateTime.Now-ti);
       Prepare_Szenario();
       string answer = "";
       
       void InputCallback()
       {
           answer = m_AI_VTChannelSO.ScenarioChoice.ToString();
           switch (answer)
           {
               case "1":
                   Szenario_Entgegenkommer();
                   break;
               case "2":
                   Szenario_Kreuzend2();
                   break;
               case "3":
                   Szenario_Ueberholer();
                   break;
               case "4":
                   Szenario_Fischer();
                   break;
               case "5":
                   Szenario_Behindert();
                   break;
               case "6":
                   Szenario_Faehren();
                   break;
               case "7":
                   Szenario_Nacht();
                   break;
               case "8":
                   Szenario_Custom();
                   break; 
           }
       }

       InputCallback();
       
       /**PopupManager.Instance.ShowInputPopup("1.Entgegenkommer\n"+ 
                                                "2.Kreuzen\n"+
                                                "3.Ueberholer\n"+
                                                "4.Fischer\n"+
                                                "5.Manoevrierbehindert\n"+
                                                "6.Faehren\n"+
                                                "7.Nahbereich Nacht\n"+
                                                "8.Custom", 
                                             InputCallback);
                                             */
       
       /*
       CFzg f4 = prepare_Fzg4();
       f4.Plan.Map_Anzeige();
       t = 0;
       f4.verfolge_Plan(f4.Plan, f4.firstSPD.Copy(), ref t_aufTrack,30,f4.Track);
       f4.Track.Map_Anzeige(30d);*/
       //Debug.Log("Prep Zeit "+(DateTime.Now-ti));
       //Debug.Log("benötigte Zeit "+(DateTime.Now-ti));
       //f0.testsimulate_track(f0.Track);
       StartCoroutine(UpdateFahrzeugeAnzeige());
       //StartCoroutine(UpdateFahrzeugeEntscheidungen());
       //Debug.Log("Sim Ende "+(DateTime.Now-ti));
       
       //m_ObjSpawnerSO.PlayerObject.Data.m_Position.Lat
   }



   private void Szenario_Entgegenkommer()
   {
       CFzg prepare_FzgRot()
       {
           CFzg ff=new CFzg("FzgRot",false,new CShipPhysicalData(38.2934943451473, 15.682301291195,12,0,12*kn,315,0));
           
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2934943451473, 15.682301291195,0,12*kn,-1,0));
           //ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.3179287917682, 15.6287592336986,0,12*kn,-1,0));
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.349031474601, 15.5487362656778,0,12*kn,-1,0));
           ff.Plan.name = ff.name+"(Plan)";
           ff.Entf_Ausweichen = 2*AIConst.sm;
           ff.krit_Entf_entgegen = 1 * sm;
           return ff;
       }
       
       CFzg prepare_FzgBlau()
       {
           CFzg ff=new CFzg("FzgBlau",false,new CShipPhysicalData(38.313091936936, 15.6441473198119,10,0,10*kn,135,0));
           
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.313091936936, 15.6441473198119,0,10*kn,-1,0));
           //ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2892412480872, 15.6918924843962,0,10*kn,-1,0));
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2695355236315, 15.7483326750912,0,10*kn,-1,0));
           ff.Plan.name = ff.name+"(Plan)";
           ff.Entf_Ausweichen = 2*AIConst.sm;
           ff.krit_Entf_entgegen = 1 * sm;
           return ff;
       }
      
       double t = 0, t_aufTrack=0;   
       
       CFzg frot = prepare_FzgRot();frot.Setze_auf_Karte();    
       frot.Plan.Map_Anzeige(Color.magenta,true);
       frot.verfolge_Plan(frot.Plan, frot.firstSPD.Copy(), ref t_aufTrack,30,frot.Track);
       frot.Plan.counter = 0;
       
       CFzg fblau = prepare_FzgBlau();fblau.Setze_auf_Karte();    
       fblau.Plan.Map_Anzeige(Color.magenta,true);
       fblau.verfolge_Plan(fblau.Plan, fblau.firstSPD.Copy(), ref t_aufTrack,30,fblau.Track);
       fblau.Plan.counter = 0;
       
       CFzg fEigenschiff = prepare_Eigenschiff();fEigenschiff.Setze_auf_Karte();
       t = 0;
       fEigenschiff.verfolge_Plan(fEigenschiff.Plan, fEigenschiff.firstSPD.Copy(), ref t_aufTrack,30,fEigenschiff.Track);
       fEigenschiff.Plan.Map_Anzeige(Color.magenta,true);
       fEigenschiff.Plan.counter = 0;       
       fEigenschiff.ObjSC.Data.OnUserInteractionStopped +=  fEigenschiff.ES_simulate_track_update;
       fEigenschiff.ES_simulate_track_update();
       
   }
   
   private void Szenario_Kreuzend()
   {
       CFzg prepare_FzgRot()
       {
           CFzg ff=new CFzg("FzgRot",false,new CShipPhysicalData(38.2952676283522, 15.6673167642925,12,0,12*kn,290,0));//38.288126873536, 15.6816810413211
           
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2952676283522, 15.6673167642925,0,12*kn,-1,0));//(38.288126873536, 15.6816810413211
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.3511992173149, 15.5612916267611,0,12*kn,-1,0));
          
           ff.Plan.name = ff.name+"(Plan)";
           ff.Entf_Ausweichen = 2*AIConst.sm;
           ff.krit_Entf_entgegen = 1 * sm;
           return ff;
       }
       
       CFzg prepare_FzgBlau()
       {
           CFzg ff=new CFzg("FzgBlau",false,new CShipPhysicalData(38.3220587524233, 15.6482559635724,10,0,10*kn,185,0));
           
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.3220587524233, 15.6482559635724,0,10*kn,-1,0));
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2806089102047, 15.6433168950313,0,10*kn,-1,0));
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2750231903865, 15.6705555128258,0,10*kn,-1,0));
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2647728282446, 15.6613251087432,0,10*kn,-1,0));
           
           ff.Plan.name = ff.name+"(Plan)";
           ff.Entf_Ausweichen = 2*AIConst.sm;
           ff.krit_Entf_entgegen = 1 * sm;
           return ff;
       }
      
       double t = 0, t_aufTrack=0;   
       
       CFzg frot = prepare_FzgRot();frot.Setze_auf_Karte();    
       frot.Plan.Map_Anzeige(Color.magenta,true);
       frot.verfolge_Plan(frot.Plan, frot.firstSPD.Copy(), ref t_aufTrack,30,frot.Track);
       frot.Plan.counter = 0;
       
       CFzg fblau = prepare_FzgBlau();fblau.Setze_auf_Karte();    
       fblau.Plan.Map_Anzeige(Color.magenta,true);
       fblau.verfolge_Plan(fblau.Plan, fblau.firstSPD.Copy(), ref t_aufTrack,30,fblau.Track);
       fblau.Plan.counter = 0;
       
       CFzg fEigenschiff = prepare_Eigenschiff();fEigenschiff.Setze_auf_Karte();
       t = 0;
       fEigenschiff.verfolge_Plan(fEigenschiff.Plan, fEigenschiff.firstSPD.Copy(), ref t_aufTrack,30,fEigenschiff.Track);
       fEigenschiff.Plan.Map_Anzeige(Color.magenta,true);
       fEigenschiff.Plan.counter = 0;       
       fEigenschiff.ObjSC.Data.OnUserInteractionStopped +=  fEigenschiff.ES_simulate_track_update;
       fEigenschiff.ES_simulate_track_update();
       
   }
   
   private void Szenario_Kreuzend2()
   {
       CFzg prepare_FzgRot()
       {
           CFzg ff=new CFzg("FzgRot",false,new CShipPhysicalData(38.288126873536, 15.6816810413211,12,0,12*kn,290,0));//38.288126873536, 15.6816810413211
           
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.288126873536, 15.6816810413211,0,12*kn,-1,0));//38.288126873536, 15.6816810413211
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.3511992173149, 15.5612916267611,0,12*kn,-1,0));
          
           ff.Plan.name = ff.name+"(Plan)";
           ff.Entf_Ausweichen = 2*AIConst.sm;
           ff.krit_Entf_entgegen = 1 * sm;
           return ff;
       }
       
       CFzg prepare_FzgBlau()
       {
           CFzg ff=new CFzg("FzgBlau",false,new CShipPhysicalData(38.3220587524233, 15.6482559635724,10,0,10*kn,185,0));
           
           //ff.Setze_auf_Karte(new Position(38.236651,15.607450));
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.3220587524233, 15.6482559635724,0,10*kn,-1,0));
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2806089102047, 15.6433168950313,0,10*kn,-1,0));
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2750231903865, 15.6705555128258,0,10*kn,-1,0));
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2647728282446, 15.6613251087432,0,10*kn,-1,0));
           
           ff.Plan.name = ff.name+"(Plan)";
           ff.Entf_Ausweichen = 2*AIConst.sm;
           ff.krit_Entf_entgegen = 1 * sm;
           return ff;
       }
      
       double t = 0, t_aufTrack=0;   
       
       CFzg frot = prepare_FzgRot();frot.Setze_auf_Karte();    
       frot.Plan.Map_Anzeige(Color.magenta,true);
       frot.verfolge_Plan(frot.Plan, frot.firstSPD.Copy(), ref t_aufTrack,30,frot.Track);
       frot.Plan.counter = 0;
       
       CFzg fblau = prepare_FzgBlau();fblau.Setze_auf_Karte();    
       fblau.Plan.Map_Anzeige(Color.magenta,true);
       fblau.verfolge_Plan(fblau.Plan, fblau.firstSPD.Copy(), ref t_aufTrack,30,fblau.Track);
       fblau.Plan.counter = 0;
       
       CFzg fEigenschiff = prepare_Eigenschiff();fEigenschiff.Setze_auf_Karte();
       t = 0;
       fEigenschiff.verfolge_Plan(fEigenschiff.Plan, fEigenschiff.firstSPD.Copy(), ref t_aufTrack,30,fEigenschiff.Track);
       fEigenschiff.Plan.Map_Anzeige(Color.magenta,true);
       fEigenschiff.Plan.counter = 0;       
       fEigenschiff.ObjSC.Data.OnUserInteractionStopped +=  fEigenschiff.ES_simulate_track_update;
       fEigenschiff.ES_simulate_track_update();
       
   }
   
   
   private void Szenario_Ueberholer()
   {
       CFzg prepare_FzgRot()
       {
           CFzg ff=new CFzg("FzgRot",false,new CShipPhysicalData(38.316722114485, 15.3765795853817,6,0,5*kn,70,0));
           
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.316722114485, 15.3765795853817,0,5*kn,-1,0));
           //ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.3179287917682, 15.6287592336986,0,12*kn,-1,0));
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.3579965055027, 15.5777309633173,0,5*kn,-1,0));
           ff.Plan.name = ff.name+"(Plan)";
           ff.Entf_Ausweichen = 2*AIConst.sm;
           ff.krit_Entf_entgegen = 1 * sm;
           return ff;
       }
       
       CFzg prepare_FzgBlau()
       {
           CFzg ff=new CFzg("FzgBlau", false,new CShipPhysicalData(38.2915507662111, 15.2307042392491,18,0,20*kn,80,0));
          
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2915507662111, 15.2307042392491,0,20*kn,-1,0));
           //ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2892412480872, 15.6918924843962,0,10*kn,-1,0));
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.367393075636, 15.6388720633089,0,20*kn,-1,0));
           ff.Plan.name = ff.name+"(Plan)";
           ff.Entf_Ausweichen = 2*AIConst.sm;
           ff.krit_Entf_entgegen = 1 * sm;
           return ff;
       }
      
       double t = 0, t_aufTrack=0;   
       
       CFzg frot = prepare_FzgRot();frot.Setze_auf_Karte();    
       frot.Plan.Map_Anzeige(Color.magenta,true);
       frot.verfolge_Plan(frot.Plan, frot.firstSPD.Copy(), ref t_aufTrack,30,frot.Track);
       frot.Plan.counter = 0;
       
       CFzg fblau = prepare_FzgBlau();fblau.Setze_auf_Karte();    
       fblau.Plan.Map_Anzeige(Color.magenta,true);
       fblau.verfolge_Plan(fblau.Plan, fblau.firstSPD.Copy(), ref t_aufTrack,30,fblau.Track);
       fblau.Plan.counter = 0;
       
       
       CFzg fEigenschiff = prepare_Eigenschiff();fEigenschiff.Setze_auf_Karte();
       t = 0;
       fEigenschiff.verfolge_Plan(fEigenschiff.Plan, fEigenschiff.firstSPD.Copy(), ref t_aufTrack,30,fEigenschiff.Track);
       fEigenschiff.Plan.Map_Anzeige(Color.magenta,true);
       fEigenschiff.Plan.counter = 0;       
       fEigenschiff.ObjSC.Data.OnUserInteractionStopped +=  fEigenschiff.ES_simulate_track_update;
       fEigenschiff.ES_simulate_track_update();
   }
   
   private void Szenario_Fischer()
   {
       CFzg prepare_FzgRot()
       {
           CFzg ff=new CFzg("FzgRot",false,new CShipPhysicalData(38.3496968275915, 15.3210115244842,7,0,7*kn,170,0));
           
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.3496968275915, 15.3210115244842,0,7*kn,-1,0));
           //ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.3179287917682, 15.6287592336986,0,12*kn,-1,0));
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2375822544345, 15.3886520286845,0,7*kn,-1,0));
           ff.Plan.name = ff.name+"(Plan)";
           ff.Entf_Ausweichen = 2*AIConst.sm;
           ff.krit_Entf_entgegen = 1 * sm;
           ff.istFischer = true;
           return ff;
       }
       
       CFzg prepare_FzgBlau()
       {
           CFzg ff=new CFzg("FzgBlau",false,new CShipPhysicalData(38.293177250728, 15.249749700793,13,0,13*kn,80,0));
           
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.293177250728, 15.249749700793,0,13*kn,-1,0));
           //ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2892412480872, 15.6918924843962,0,10*kn,-1,0));
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.345601996834, 15.5139134329103,0,13*kn,-1,0));
           ff.Plan.name = ff.name+"(Plan)";
           ff.Entf_Ausweichen = 2*AIConst.sm;
           ff.krit_Entf_entgegen = 1 * sm;
           return ff;
       }
      
       double t = 0, t_aufTrack=0;   
       
       CFzg frot = prepare_FzgRot();frot.Setze_auf_Karte();    
       frot.Plan.Map_Anzeige(Color.magenta,true);
       frot.verfolge_Plan(frot.Plan, frot.firstSPD.Copy(), ref t_aufTrack,30,frot.Track);
       frot.Plan.counter = 0;
       
       CFzg fblau = prepare_FzgBlau();fblau.Setze_auf_Karte();    
       fblau.Plan.Map_Anzeige(Color.magenta,true);
       fblau.verfolge_Plan(fblau.Plan, fblau.firstSPD.Copy(), ref t_aufTrack,30,fblau.Track);
       fblau.Plan.counter = 0;
       
       CFzg fEigenschiff = prepare_Eigenschiff();fEigenschiff.Setze_auf_Karte();
       t = 0;
       fEigenschiff.verfolge_Plan(fEigenschiff.Plan, fEigenschiff.firstSPD.Copy(), ref t_aufTrack,30,fEigenschiff.Track);
       fEigenschiff.Plan.Map_Anzeige(Color.magenta,true);
       fEigenschiff.Plan.counter = 0;       
       fEigenschiff.ObjSC.Data.OnUserInteractionStopped +=  fEigenschiff.ES_simulate_track_update;
       fEigenschiff.ES_simulate_track_update();
       
   }
   
   
   private void Szenario_Behindert()
   {
       CFzg prepare_FzgRot()
       {
           CFzg ff=new CFzg("FzgRot",false,new CShipPhysicalData(38.3496968275915, 15.3210115244842,7,0,7*kn,170,0));
           
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.3496968275915, 15.3210115244842,0,7*kn,-1,0));
           //ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.3179287917682, 15.6287592336986,0,12*kn,-1,0));
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2375822544345, 15.3886520286845,0,7*kn,-1,0));
           ff.Plan.name = ff.name+"(Plan)";
           ff.Entf_Ausweichen = 2*AIConst.sm;
           ff.krit_Entf_entgegen = 1 * sm;
           ff.istManoevrierbeh = true;
           return ff;
       }
       
       CFzg prepare_FzgBlau()
       {
           CFzg ff=new CFzg("FzgBlau",false,new CShipPhysicalData(38.293177250728, 15.249749700793,13,0,13*kn,80,0));
           
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.293177250728, 15.249749700793,0,13*kn,-1,0));
           //ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2892412480872, 15.6918924843962,0,10*kn,-1,0));
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.345601996834, 15.5139134329103,0,13*kn,-1,0));
           ff.Plan.name = ff.name+"(Plan)";
           ff.Entf_Ausweichen = 2*AIConst.sm;
           ff.krit_Entf_entgegen = 1 * sm;
           return ff;
       }
      
       double t = 0, t_aufTrack=0;   
       
       CFzg frot = prepare_FzgRot();frot.Setze_auf_Karte();    
       frot.Plan.Map_Anzeige(Color.magenta,true);
       frot.verfolge_Plan(frot.Plan, frot.firstSPD.Copy(), ref t_aufTrack,30,frot.Track);
       frot.Plan.counter = 0;
       
       CFzg fblau = prepare_FzgBlau();fblau.Setze_auf_Karte();    
       fblau.Plan.Map_Anzeige(Color.magenta,true);
       fblau.verfolge_Plan(fblau.Plan, fblau.firstSPD.Copy(), ref t_aufTrack,30,fblau.Track);
       fblau.Plan.counter = 0;
       
       CFzg fEigenschiff = prepare_Eigenschiff();fEigenschiff.Setze_auf_Karte();
       t = 0;
       fEigenschiff.verfolge_Plan(fEigenschiff.Plan, fEigenschiff.firstSPD.Copy(), ref t_aufTrack,30,fEigenschiff.Track);
       fEigenschiff.Plan.Map_Anzeige(Color.magenta,true);
       fEigenschiff.Plan.counter = 0;       
       fEigenschiff.ObjSC.Data.OnUserInteractionStopped +=  fEigenschiff.ES_simulate_track_update;
       fEigenschiff.ES_simulate_track_update();
   }
   
   private void Szenario_Faehren()
   {
       CFzg prepare_FzgRot()
       {
           CFzg ff=new CFzg("FzgRot",false,new CShipPhysicalData(38.2221219929785, 15.6293774369567,15,0,15*kn,255,0));
          
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2221219929785, 15.6293774369567,0,7*kn,-1,0));
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2131187917741, 15.5829748523015,0,7*kn,-1,0));
           ff.Plan.name = ff.name+"(Plan)";
           ff.Entf_Ausweichen = 0.25*sm;
           ff.krit_Entf_entgegen =  0.1*sm;;
           ff.krit_Entf_kreuz = 0.2 * sm;
           ff.krit_Entf_ueberholen = 0.1 * sm;
           return ff;
       }
       
       CFzg prepare_FzgGelb()
       {
           CFzg ff=new CFzg("FzgGelb",false,new CShipPhysicalData(38.2166626152097, 15.5923360727212,13,0,12*kn,070,0));
           
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2166626152097, 15.5923360727212,0,7*kn,-1,0));
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2270067011425, 15.6330493162101,0,7*kn,-1,0));
           ff.Plan.name = ff.name+"(Plan)";
           ff.Entf_Ausweichen = 0.25*sm;;
           ff.krit_Entf_entgegen =  0.15*sm;;
           ff.krit_Entf_kreuz = 0.2 * sm;
           ff.krit_Entf_ueberholen = 0.1 * sm;
           return ff;
       }
       
       CFzg prepare_FzgGruen()
       {
           CFzg ff=new CFzg("FzgGruen",false,new CShipPhysicalData(38.2110755073805, 15.6171110246025,15,0,15*kn,010,0),
                       0     /*AIConst.cSchiffstyp_Containerschiff*/);
          
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2110755073805, 15.6171110246025,0,15*kn,-1,0));
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2372230900626, 15.6268354148979,0,15*kn,-1,0));
           ff.Plan.name = ff.name+"(Plan)";
           ff.Entf_Ausweichen = 0.5*sm;
           ff.krit_Entf_kreuz = 0.25 * sm;
           
           return ff;
       }
       
       CFzg prepare_FzgBlau()
       {
           CFzg ff=new CFzg("Eigenschiff", true,new CShipPhysicalData(38.2240375803594, 15.6058936907724,7,0,7*kn,120,0));
           
           ff.Plan.name = ff.name+"(Plan)";
           ff.istManuell = true;
          
           return ff;
       }
      
       double t = 0, t_aufTrack=0;   
       
       CFzg frot = prepare_FzgRot();frot.Setze_auf_Karte();    
       frot.Plan.Map_Anzeige(Color.magenta,true);
       frot.verfolge_Plan(frot.Plan, frot.firstSPD.Copy(), ref t_aufTrack,30,frot.Track);
       frot.Plan.counter = 0;
       
       CFzg fgelb = prepare_FzgGelb();fgelb.Setze_auf_Karte();    
       fgelb.Plan.Map_Anzeige(Color.magenta,true);
       fgelb.verfolge_Plan(fgelb.Plan, fgelb.firstSPD.Copy(), ref t_aufTrack,30,fgelb.Track);
       fgelb.Plan.counter = 0;
       
       CFzg fgruen = prepare_FzgGruen();fgruen.Setze_auf_Karte();    
       fgruen.Plan.Map_Anzeige(Color.magenta,true);
       fgruen.verfolge_Plan(fgruen.Plan, fgruen.firstSPD.Copy(), ref t_aufTrack,30,fgruen.Track);
       fgruen.Plan.counter = 0;
       
      
       
       CFzg fEigenschiff = prepare_FzgBlau();fEigenschiff.Setze_auf_Karte();
       t = 0;
       fEigenschiff.verfolge_Plan(fEigenschiff.Plan, fEigenschiff.firstSPD.Copy(), ref t_aufTrack,30,fEigenschiff.Track);
       fEigenschiff.Plan.Map_Anzeige(Color.magenta,true);
       fEigenschiff.Plan.counter = 0;       
       fEigenschiff.ObjSC.Data.OnUserInteractionStopped +=  fEigenschiff.ES_simulate_track_update;
       fEigenschiff.ES_simulate_track_update();
   }


   private void Szenario_Nacht()
   {
       CFzg prepare_FzgRot()
       {
           CFzg ff=new CFzg("FzgRot",false,new CShipPhysicalData(38.2193148302931, 15.6198973498782,2,0,1.5*kn,260,0));
          
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2193148302931, 15.6198973498782,0,1.5*kn,-1,0));
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2131187917741, 15.5829748523015,0,1.5*kn,-1,0));
           ff.Plan.name = ff.name+"(Plan)";
           ff.Entf_Ausweichen = 0.25*sm;
           ff.krit_Entf_entgegen =  0.1*sm;;
           ff.krit_Entf_kreuz = 0.2 * sm;
           ff.krit_Entf_ueberholen = 0.1 * sm;
           return ff;
       }
       
       CFzg prepare_FzgGelb()
       {
           CFzg ff=new CFzg("FzgGelb",false,new CShipPhysicalData(38.2243974826134, 15.6067251754921,3,0,2*kn,090,0));
           
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2243974826134, 15.6067251754921,0,2*kn,-1,0));
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2236123720098, 15.6286026960802,0,2*kn,-1,0));
           ff.Plan.name = ff.name+"(Plan)";
           ff.istManoevrierbeh = true;
           ff.Entf_Ausweichen = 0.25*sm;;
           ff.krit_Entf_entgegen =  0.15*sm;;
           ff.krit_Entf_kreuz = 0.2 * sm;
           ff.krit_Entf_ueberholen = 0.1 * sm;
           return ff;
       }
       
       CFzg prepare_FzgGruen()
       {
           CFzg ff=new CFzg("FzgGruen",false,new CShipPhysicalData(38.2246219751248, 15.5984882398927,18,0,18*kn,135,0),
                       0     /*AIConst.cSchiffstyp_Containerschiff*/);
          
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2246219751248, 15.5984882398927,0,18*kn,-1,0));
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2098706991449, 15.6202937135065,0,18*kn,-1,0));
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.1800974833952, 15.6184743905567,0,18*kn,-1,0));
           ff.Plan.name = ff.name+"(Plan)";
           ff.Entf_Ausweichen = 0.5*sm;
           ff.krit_Entf_kreuz = 0.25 * sm;
          
           return ff;
       }
       
       CFzg prepare_FzgBlau()
       {
           CFzg ff=new CFzg("FzgBlau", false,new CShipPhysicalData(38.2117543414636, 15.6102241487948,7,0,7*kn,010,0));
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2117543414636, 15.6102241487948,0,8*kn,-1,0));
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2329295238562, 15.614991292011,0,8*kn,-1,0));
           ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2431664884421, 15.6402665580082,0,8*kn,-1,0));
           ff.Plan.name = ff.name+"(Plan)";
           ff.Entf_Ausweichen = 1*sm;
           ff.krit_Entf_kreuz = 0.5 * sm;
           return ff;
       }
      
       double t = 0, t_aufTrack=0;   
       
       CFzg frot = prepare_FzgRot();frot.Setze_auf_Karte();    
       frot.Plan.Map_Anzeige(Color.magenta,true);
       frot.verfolge_Plan(frot.Plan, frot.firstSPD.Copy(), ref t_aufTrack,30,frot.Track);
       frot.Plan.counter = 0;
       
       CFzg fgelb = prepare_FzgGelb();fgelb.Setze_auf_Karte();    
       fgelb.Plan.Map_Anzeige(Color.magenta,true);
       fgelb.verfolge_Plan(fgelb.Plan, fgelb.firstSPD.Copy(), ref t_aufTrack,30,fgelb.Track);
       fgelb.Plan.counter = 0;
       
       CFzg fgruen = prepare_FzgGruen();fgruen.Setze_auf_Karte();    
       fgruen.Plan.Map_Anzeige(Color.magenta,true);
       fgruen.verfolge_Plan(fgruen.Plan, fgruen.firstSPD.Copy(), ref t_aufTrack,30,fgruen.Track);
       fgruen.Plan.counter = 0;
       
      
       
       CFzg fblau = prepare_FzgBlau();fblau.Setze_auf_Karte();
       t = 0;
       fblau.verfolge_Plan(fblau.Plan, fblau.firstSPD.Copy(), ref t_aufTrack,30,fblau.Track);
       fblau.Plan.Map_Anzeige(Color.magenta,true);
       fblau.verfolge_Plan(fblau.Plan, fblau.firstSPD.Copy(), ref t_aufTrack,30,fblau.Track);
       fblau.Plan.counter = 0;  
       
       CFzg fEigenschiff = prepare_Eigenschiff();fEigenschiff.Setze_auf_Karte();
       t = 0;
       fEigenschiff.verfolge_Plan(fEigenschiff.Plan, fEigenschiff.firstSPD.Copy(), ref t_aufTrack,30,fEigenschiff.Track);
       fEigenschiff.Plan.Map_Anzeige(Color.magenta,true);
       fEigenschiff.Plan.counter = 0;       
       fEigenschiff.ObjSC.Data.OnUserInteractionStopped +=  fEigenschiff.ES_simulate_track_update;
       fEigenschiff.ES_simulate_track_update();
   }



   private void Szenario_Custom()
   {
       double t=0, t_aufTrack=0;
       
          CFzg prepare_Fzg0()
          {
              CFzg ff=new CFzg("Fzg0",false,new CShipPhysicalData(38.2091062653603, 15.5955199793523,10,0,11*kn,180,0));
              
              ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2091062653603, 15.5955199793523,0,5*kn,-1,0));
              ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.1824280832354, 15.5896985990862,0,5*kn,-1,0));
              ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.1626916787974, 15.5781406870873,0,15*kn,-1,0));
              ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.1664255857421, 15.5631834578226,0,15*kn,-1,0));
              ff.Plan.name = ff.name+"(Plan)";
              return ff;
          }
          
          
          CFzg prepare_Fzg3()
          {
              CFzg ff=new CFzg("Fzg3",false,new CShipPhysicalData(38.1818737378546,15.5875479457377 ,16,0,15*kn,20,0));
              
              ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.1818737378546,15.5875479457377 ,0,15*kn,-1,0));
              ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2273498560443,15.5981887351096 ,0,15*kn,-1,0));
              
              ff.Plan.name = ff.name+"(Plan)";
              return ff;
          }
          
          CFzg prepare_Fzg1()
          {
              CFzg ff=new CFzg("Fzg1",false,new CShipPhysicalData(38.1954078830926, 15.579047191978,7,0,6*kn,089,0));
              
              ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.1954078830926, 15.579047191978,0,9*kn,-1,900));
              ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.1961191016931, 15.6279982532133,0,5*kn,-1,0));
              ff.Plan.name = ff.name+"(Plan)";
              return ff;
          }
          
          CFzg prepare_Fzg2()
          {
              CFzg ff=new CFzg("Fzg2",false,new CShipPhysicalData(38.1791566762488, 15.6330453641181,10,0,5*kn,270,0));
              
              ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.175919609069, 15.6353941172254, 0,12*kn,-1,0));
              ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.1745915610069, 15.6247736986878,0,12*kn,-1,0));
              ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.1775796358322, 15.5775944773356,0,5*kn,-1,0));
              ff.Plan.name = ff.name+"(Plan)";
              return ff;
          }
          
          CFzg prepare_Fzg4()
          {
              CFzg ff=new CFzg("Fzg4",false,new CShipPhysicalData(38.1758492786588, 15.6032961217312,10,0,5*kn,330,0));
              
              ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.1758492786588, 15.6032961217312,0,10*kn,-1,0));
              ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.1770939205016, 15.5747413191051,0,5*kn,-1,0));
       
              ff.Plan.name = ff.name+"(Plan)";
              ff.Plan.Map_Anzeige();
              
              return ff;
          }
          
          CFzg prepare_Fzg5()
          {
              CFzg ff=new CFzg("Fzg5",false,new CShipPhysicalData(38.2338643675424,15.6291354755169,10,0,6*kn,60,0));
              
              ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2338643675424,15.6291354755169,0,6*kn,-1,0));
              ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2648209066856,15.71027644373,0,6*kn,-1,0));
       
              ff.Plan.name = ff.name+"(Plan)";
              
              return ff;
          }
          
          CFzg prepare_Fzg6()
          {
              CFzg ff=new CFzg("Fzg6",false,new CShipPhysicalData(38.2171279436684,15.61192810464,15,0,16*kn,30,0));
              
              ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2171279436684,15.61192810464,0,15*kn,-1,0));
              ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2338643675424,15.6291354755169,0,15*kn,-1,0));
              ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2648209066856,15.71027644373,0,15*kn,-1,1400));
              ff.Plan.ListeWegpunkt.Add(new CWegpunkt(38.2765241803181,15.7394700217637,0,15*kn,-1,0));
              ff.Plan.name = ff.name+"(Plan)";
             
              
              return ff;
          }
       
       
       CFzg f0 = prepare_Fzg0();f0.Setze_auf_Karte();    //Gegner ohne Fahrt
       f0.Plan.Map_Anzeige(Color.magenta,true);
       t = 0;
       f0.verfolge_Plan(f0.Plan, f0.firstSPD.Copy(), ref t_aufTrack,30,f0.Track);
       f0.Plan.counter = 0;
       
       
       CFzg f1 = prepare_Fzg1();f1.Setze_auf_Karte();
       f1.Plan.Map_Anzeige(Color.magenta,true);
       t = 0;
       f1.verfolge_Plan(f1.Plan, f1.firstSPD.Copy(), ref t_aufTrack,30,f1.Track);
       f1.Plan.counter = 0;
       
       
       CFzg f2 = prepare_Fzg2();f2.Setze_auf_Karte();
       f2.Plan.Map_Anzeige(Color.magenta,true);
       t = 0;
       f2.verfolge_Plan(f2.Plan, f2.firstSPD.Copy(), ref t_aufTrack,30,f2.Track);
       f2.Plan.counter = 0;
       
       
       CFzg f3 = prepare_Fzg3();f3.Setze_auf_Karte();
       f3.Plan.Map_Anzeige(Color.magenta,true);
       t = 0;
       f3.verfolge_Plan(f3.Plan, f3.firstSPD.Copy(), ref t_aufTrack,30,f3.Track);
       f3.Plan.counter = 0;
       
       
        CFzg f5 = prepare_Fzg5();f5.Setze_auf_Karte();  //ÜberholTer
        f5.Plan.Map_Anzeige(Color.magenta,true);
        t = 0;
        f5.verfolge_Plan(f5.Plan, f5.firstSPD.Copy(), ref t_aufTrack,30,f5.Track);
        f5.Plan.counter = 0;


        CFzg f6 = prepare_Fzg6();f6.Setze_auf_Karte();     //Überholer
        f6.Plan.Map_Anzeige(Color.magenta,true);
        t = 0;
        f6.verfolge_Plan(f6.Plan, f6.firstSPD.Copy(), ref t_aufTrack,30,f6.Track);
        f6.Plan.counter = 0;
   }

   
   private void Prepare_Szenario()
   {
       double ex = -0.025/60;
       double ey = -0.001; //-0.000/60;
       ;
       //38.2648208752946,15.710277008473
       //38.2647794822174,15.71013555316
       List<double2> tt2 = new List<double2>();
       tt2.Add(new double2(38.2648208752946+ey,15.710277008473+ex)); //S
       tt2.Add(new double2(38.2647794822174+ey+ey,15.71013555316+ex)); //
       CArea tt = new CArea("tt",AIConst.cAreaTyp_VTG,tt2,Color.green,14.5f,205);
       tt.Map_Anzeige();
       
       /*
        Bbd Fahrwasser	Messina 12	38° 11.754‘ N	015° 37,450‘ E
        Bbd Fahrwasser beleuchtet	Messina 14	38° 10.840‘ N	015° 37,450’ E
        Bbd Fahrwasser	Messina 16	38° 09.890‘ N	015° 37,900’ E
        Bbd Fahrwasser beleuchtet	Messina 18	38° 08.940‘ N	015° 38,350’ E         
       */

       List<double2> ld2 = new List<double2>();
       ld2.Add(new double2(38d+11.842/60+ey,015d+ 35.099/60+ex)); //Stbd Fahrwasser beleuchtet	Messina 11	38° 11.842‘ N	015° 35,099’ E
       ld2.Add(new double2(38d+10.840/60+ey,015d+ 34.796/60+ex)); //Stbd Fahrwasser beleuchtet	Messina 13	38° 10.840‘ N	015° 34,796’ E
       ld2.Add(new double2(38d+09.890/60+ey,015d+ 34.067/60+ex)); //Stbd Fahrwasser 	Messina 15	38° 09.890‘ N	015° 34,067’ E
       ld2.Add(new double2(38d+08.940/60+ey,015d+ 33.337/60+ex)); //Stbd Fahrwasser beleuchtet	Messina 17	38° 08.940‘ N	015° 33,337’ E
       
       ld2.Add(new double2(38d+08.942/60+ey,015d+ 35.803/60+ex)); //Fahrwassermitte beleuchtet	VTC Messina South	38° 08.942‘ N	015° 35,803’ E
       ld2.Add(new double2(38d+10.457/60+ey,015d+ 36.040/60+ex)); //Fahrwassermitte beleuchtet		38° 10.457‘ N	015° 36,040’ E
       ld2.Add(new double2(38d+11.835/60+ey,015d+ 36.241/60+ex)); //Fahrwassermitte beleuchtet		38° 11.835‘ N	015° 36,241’ E
       
       
       
       CArea VTCMessinaSouth = new CArea("VTCMessinaSouth",AIConst.cAreaTyp_VTG,ld2,Color.gray,1.5f,205);
       VTCMessinaSouth.Map_Anzeige();
       
       
       List<double2> tg3 = new List<double2>();//Fahrwassermitte Nogo
       tg3.Add(new double2(38.2284997050024+ey,15.623413115493+ex));
       tg3.Add(new double2(38.223517159442+ey,15.6105811703142+ex)); //X
       tg3.Add(new double2(38.2335824271765+ey,15.6118771160727+ex)); //X //, 
       CArea VTCMessinaEastNorth0 = new CArea("VTCMessinaEastNorth0",AIConst.cAreaTyp_VTG,tg3,Color.gray,1.5f,15);
       VTCMessinaEastNorth0.Map_Anzeige();

       List<double2> tg4 = new List<double2>();//Fahrwassermitte Nogo
       tg4.Add(new double2(38.2335824271765+ey,15.6118771160727+ex)); //X //, 
       tg4.Add(new double2(38.2596765860551+ey,15.6801692112304+ex));
       tg4.Add(new double2(38.2572358346698+ey,15.6972315270109+ex)); //X
       tg4.Add(new double2(38.2284997050024+ey,15.623413115493+ex)); //X
       //15.6801692112304, 38.2596765860551
       ///15.6972315270109, 38.2572358346698
       //15.623413115493, 38.2284997050024

       CArea VTCMessinaEastNorth1 = new CArea("VTCMessinaEastNorth1",AIConst.cAreaTyp_VTG,tg4,Color.gray,1.5f,67);
       VTCMessinaEastNorth1.Map_Anzeige();
       
       
       
       
       //15.5948495577723, 38.1455322143443
       //15.5980894221685, 38.1457662818158
       //15.6054331694501, 38.194629615505
       //15.6020492746473, 38.1946881490302
       List<double2> tg1 = new List<double2>();//Fahrwassermitte Nogo
       tg1.Add(new double2(38.1455322143443+ey,15.5948495577723 +ex)); 
       tg1.Add(new double2(38.1457662818158+ey,15.5980894221685+ex )); 
       tg1.Add(new double2(38.194629615505+ey,15.6054331694501+ex)); 
       tg1.Add(new double2(38.1946881490302+ey,15.6020492746473+ex)); 
       CArea VTCMmitte = new CArea("VTCMitteS",AIConst.cAreaTyp_NOGO,tg1,Color.red,1.5f,0);
       VTCMmitte.Map_Anzeige();
       
       /*
        15.6071973164989, 38.2222297550341
        15.6105811703142, 38.223517159442
15.6118771160727, 38.2335824271765
15.6801692112304, 38.2596765860551
15.6872249386864, 38.2757107742001
15.6842010789125, 38.2764715434543
15.6772893408755, 38.2618417934528
15.6089593731805, 38.2354955106595
        */
       List<double2> tg2 = new List<double2>();//Fahrwassermitte Nogo
       tg2.Add(new double2(38.2251434946531+ey, 15.6075544150306+ex));//38.2251434946531, 15.6075544150306
       tg2.Add(new double2(38.226478662098+ey, 15.6111494417744+ex)); //38.226478662098, 15.6111494417744
       tg2.Add(new double2(38.2366955797842+ey, 15.6128368973403+ex)); //38.2366955797842, 15.6128368973403
       tg2.Add(new double2(38.2628350657354+ey, 15.6835044088796+ex)); //38.2628350657354, 15.6835044088796
       tg2.Add(new double2(38.2786248476141+ey, 15.6908411504181+ex)); //38.2786248476141, 15.6908411504181
       tg2.Add(new double2(38.2796697555192+ey, 15.6875396250793+ex)); //38.2796697555192, 15.6875396250793
       tg2.Add(new double2(38.2648087843393+ey, 15.68071645879+ex )); //38.2648087843393, 15.68071645879
       tg2.Add(new double2(38.2383957280282+ey, 15.609554835735+ex )); //38.2383957280282, 15.609554835735
       CArea VTCMmitte2 = new CArea("VTCMitteN",AIConst.cAreaTyp_NOGO,tg2,Color.red,1.5f,0);
       VTCMmitte2.Map_Anzeige();
       
       List<double2> tk = new List<double2>();//Fahrwassermitte Nogo
       tk.Add(new double2(38.2140724875273+ey, 15.6067668856453+ex));//38.2251434946531, 15.6075544150306
       tk.Add(new double2(38.2137241959082+ey, 15.6086744568231+ex)); //38.226478662098, 15.6111494417744
       tk.Add(new double2(38.2120407368442+ey, 15.6098482953723+ex)); //38.2366955797842, 15.6128368973403
       tk.Add(new double2(38.2103572447323+ey, 15.6088945306672+ex)); //38.2628350657354, 15.6835044088796
       tk.Add(new double2(38.2096853813093+ey, 15.6067697676144+ex)); //38.2786248476141, 15.6908411504181
       tk.Add(new double2(38.2104980948017+ey, 15.604568778567+ex)); //38.2796697555192, 15.6875396250793
       tk.Add(new double2(38.2120654566593+ey, 15.6042019331366+ex )); //38.2648087843393, 15.68071645879
       tk.Add(new double2(38.2135747864377+ey, 15.6048622382043 +ex)); //38.2383957280282, 15.609554835735
       CArea VTCMmitteKreisel = new CArea("VTCMitteKreisel",AIConst.cAreaTyp_NOGO,tk,Color.red,1.5f,0);
       VTCMmitteKreisel.Map_Anzeige();
       
       
       
       
       //38.2140724875273, 15.6067668856453
      //38.2137241959082, 15.6086744568231
      //38.2120407368442, 15.6098482953723
      //38.2103572447323, 15.6088945306672
      //38.2096853813093, 15.6067697676144
      //38.2104980948017, 15.604568778567
      //38.2120654566593, 15.6042019331366
      //38.2135747864377, 15.6048622382043
       
       
       /*
        38.1184216143057, 15.644445407688
        38.1398618249062, 15.6489390825957
        38.1681636338183, 15.6376344991331
        38.183450345183, 15.6315828857638
        38.2166335569224, 15.6285587390767
        38.2251195521708, 15.6250305679417
        38.230679337566, 15.6338870033807 x
        38.2574294583738, 15.7115538221083
        38.2580732272067, 15.7392752077277
        38.2887377316991, 15.7943113820306
        38.3382585930977, 15.8217188436648
        38.3647633528284, 15.8286312375033
        38.4260150056122, 15.8667931471087
        38.4570182516434, 15.8934345096167
        38.1171507505326, 15.8398750363635

        */
       List<double2> ng1 = new List<double2>();
       ng1.Add(new double2(38.1184216143057, 15.644445407688));
       ng1.Add(new double2(38.1367335387104, 15.65591765757 ));
       ng1.Add(new double2(38.1425966624427, 15.6508552908722 ));
       ng1.Add(new double2(38.1539165531611, 15.6528362060754 ));
       ng1.Add(new double2(38.1709834704055, 15.6386029124545 ));
       ng1.Add(new double2(38.185031728093, 15.6334671599635 ));
       ng1.Add(new double2(38.2282214421086, 15.6298721749874 ));
       ng1.Add(new double2(38.2574294583738, 15.7115538221083 ));
       ng1.Add(new double2(38.2580732272067, 15.7392752077277 ));
       ng1.Add(new double2(38.2887377316991, 15.7943113820306 ));
       ng1.Add(new double2(38.3382585930977, 15.8217188436648 ));
       ng1.Add(new double2(38.3647633528284, 15.8286312375033 ));
       ng1.Add(new double2(38.4260150056122, 15.8667931471087 ));
       ng1.Add(new double2(38.4570182516434, 15.8934345096167 ));
       ng1.Add(new double2(38.1171507505326, 15.8398750363635 ));
       CArea Coast_East = new CArea("Coast_East",AIConst.cAreaTyp_NOGO,ng1,Color.red,1.5f,-1);
       Coast_East.Map_Anzeige();
       
       
       List<double2> ng0 = new List<double2>();
       ng0.Add(new double2(38.1175943327107, 15.5228929257531 ));
       ng0.Add(new double2(38.1901372923817, 15.5764377664351));
       ng0.Add(new double2(38.196842962313, 15.5730089902165 ));
       ng0.Add(new double2(38.1970919879819, 15.5609588834 ));
       ng0.Add(new double2(38.2445627767936, 15.588106688624 ));
       ng0.Add(new double2(38.2657242618745, 15.6560023107815 ));
       ng0.Add(new double2(38.2748084184453, 15.6451974890569 ));
       ng0.Add(new double2(38.278771707773, 15.6091510721593 ));
       ng0.Add(new double2(38.3132176378974, 15.5690180691412 ));
       ng0.Add(new double2(38.3145834312549, 15.5420524127865 ));
       ng0.Add(new double2(38.2530570673452, 15.4442279976215 ));
       ng0.Add(new double2(38.215488005531, 15.3235717914124 ));
       ng0.Add(new double2(38.2168648259784, 15.2593114409739 ));
       ng0.Add(new double2(38.2740532793367, 15.2567584774247 ));
       ng0.Add(new double2(38.2704011804803, 15.2314620101097 ));
       ng0.Add(new double2(38.2438073711693, 15.2398410527903 ));
       ng0.Add(new double2(38.1806247117171, 15.2117295984823 ));
       ng0.Add(new double2(38.1395131889697, 15.1218598494919 ));
       ng0.Add(new double2(38.1176214673471, 15.1219619692427 ));
       CArea Coast_West = new CArea("Coast_West",AIConst.cAreaTyp_NOGO,ng0,Color.red,1.5f,-1);
       Coast_West.Map_Anzeige();
       
       
       
   }

   
   private CFzg prepare_Eigenschiff()
   {
       //38.2484830571396,15.640053013126, 
       //old 38.1530496912989, 15.613732870394938
       CFzg ff=new CFzg("Eigenschiff",true,new CShipPhysicalData(38.2484830571396,15.640053013126, 10,0,7*kn,290,0));
       // Debug.Log(ff.andrehpunkt_ermitteln2(10, 10, 10, 100));
       ff.DNCs_ermitteln();
       ff.AUFSTOPPSTECKEn_ermitteln();
       
       ff.istManuell = true;
       ff.Plan.name = ff.name+"(Plan)";
       
       return ff;
   }
   private void Start()
   {
       m_AI_VTChannelSO.SceneLoaded();// Tell all listeners that the service loaded.
   }

   private void OnDestroy()
   {   // Unsubscribe to channel
       if (m_AI_VTChannelSO)
       {
           m_AI_VTChannelSO.Reset();
           m_AI_VTChannelSO.IsActive = false;
       }
   }
   

   public double last_t_update = 0d;
   public double ti_max = 0d;
   private int fzgcounter=0;
   
   IEnumerator UpdateFahrzeugeAnzeige()
   {
       
       double tt=0;
       do
       {
           if (AIglobal.reset_timescale == true)
           {
                      Time.timeScale = 1f;
                      AIglobal.reset_timescale = false;
           }
       
       foreach (CFzg Fzg in AIglobal.Fahrzeuge)
       {
           try
           {
               tt = Time.timeSinceLevelLoad;
               
               //if (Fzg.istManuell) 
                //   Fzg.ES_simulate_track_update(tt); 
               
               Fzg.display_Fahrzeug(tt);
               Fzg.display_OptionenAusweichmanoever();
           }
           catch(Exception e)
           {
               AIglobal.Fehler("Fehler display_Fahrzeug"+Fzg.name+" "+e.ToString());
           }
           
       }
       yield return new WaitForSeconds(.25f);
       } while (true);
   }


   
   
   IEnumerator UpdateFahrzeugeEntscheidungen()
   {
       do
       {
           
       
           foreach (CFzg Fzg in AIglobal.Fahrzeuge)
           {
               
               double tt = Time.timeSinceLevelLoad;
               Fzg.simulate_track_update(Fzg.Track,tt);
           
           }
          
           yield return new WaitForSeconds(3f);
       } while (true);
   }
   
   
   private async void Update()
   {
       double dt=Time.deltaTime;
       double tt = Time.timeSinceLevelLoad;
       double ts = Time.timeScale;

       if (tt - last_t_update < 0.5d) return;    //erst wenn neuer timestamp erreicht
       //Debug.Log(tt+"/"+last_t_update);
       
       DateTime ti = DateTime.Now;

       bool bES_erzeugt = false;
       foreach (CFzg mFzg in AIglobal.Fahrzeuge)
       {
           if (mFzg.istManuell)
               if (mFzg.Track.ListeSPD.Count>1) bES_erzeugt = true;
       }

       if (!bES_erzeugt) return;
       
       last_t_update = tt;
       /*foreach (CFzg Fzg in AIglobal.Fahrzeuge)
       {
           if (Fzg.istManuell) continue;
           string s = AIglobal.FzgUpdate[^1].Split('#')[0];
           int i = Int32.Parse(s);
           i++;
           if (i>=AIglobal.Fahrzeuge.Count) i=0
           if (AIglobal.busy==false) await Task.Run(()=>Fzg.simulate_track_update(Fzg.Track,tt));
           //Fzg.simulate_track_update(Fzg.Track,tt);
           Fzg.ZeigeOptionenAusweichmanoever();
           
       }*/
       if (AIglobal.Fahrzeuge.Count == 0) return;
       if (AIglobal.FzgUpdate.Count==0) AIglobal.FzgUpdate.Add("0#");
       string s = AIglobal.FzgUpdate[^1].Split('#')[0];
       int i = Int32.Parse(s);
       i++;
       if (i >= AIglobal.Fahrzeuge.Count) i = 0;
       CFzg Fzg=AIglobal.Fahrzeuge[i];
       if (Fzg.istManuell)
       {
           AIglobal.FzgUpdate.Add(i+"#"+name+"skip #"+ti);
           return;
       }
       if (AIglobal.busy==false && bES_erzeugt) await Task.Run(()=>Fzg.simulate_track_update(Fzg.Track,tt));
       
       //Fzg.ZeigeOptionenAusweichmanoever();
       
       
      /* foreach (CFzg Fzg in AIglobal.Fahrzeuge)
       {
           //await Task.Run(()=>Fzg.ZeigeOptionenAusweichmanoever());
          // Fzg.ZeigeOptionenAusweichmanoever();
       }*/
       string sdelta_ti = (DateTime.Now - ti).Milliseconds.ToString();
       double delta_ti = Convert.ToDouble(sdelta_ti);
       if (delta_ti > ti_max) ti_max =delta_ti;
       //Debug.Log("zeit="+(DateTime.Now-ti)+" < "+ti_max.ToString());
      
   }
   
}