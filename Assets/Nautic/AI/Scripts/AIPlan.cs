using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;using Unity.Mathematics;
//
// Das sind Wegpunkte und dazugehörige Anweisungen, die vorgeben, wie das Fahrzeug sich eigentlich bewegen soll
//

public class CPlan      //-----------------------------------------Klasse--------CPlan-----------------------              
   {
       public int counter = 0;
       public double warte_bis=-1;
       public List<CWegpunkt> ListeWegpunkt = new List<CWegpunkt>();
       public string name="Plan";
       
       
       private PolyLine PL;
       private List<NauticObject> ListeNO=new List<NauticObject>();
       private List<Symbol> ListeSymbols = new List<Symbol>();
       //private List<PolyLine> ListeEMP=new List<PolyLine>();
       
       public int next_wegpunkt_counter(int icount)
       {
           if (icount>= ListeWegpunkt.Count) return -1; //Ende
           
           if (ListeWegpunkt[icount].auftrag == AIConst.cAuftrag_Fahre_Wegpunkt)
           {
               int i = (int) ListeWegpunkt[icount].auftrag_folgewegpunkt;
               if (i < ListeWegpunkt.Count && i >= 0)
               {
                   return i;
               }
               else
               {
                   return -1;
                   AIglobal.Fehler(name+":Folgewegpunkt "+i.ToString()+" nicht enthalten");
               }
           }
           if (icount + 1 < ListeWegpunkt.Count) return icount + 1;
           return -1;
       }

       public bool muss_am_Ende_aufstoppen(int cnt)
       {
           int nxt = next_wegpunkt_counter(cnt);
           if (nxt<0) return true;
           if (ListeWegpunkt[nxt].auftrag_wartezeit>0) return true;
           if (next_wegpunkt_counter(nxt) < 0) return true;
           return false;
       }
       
       
       
       public int bester_leg_von_punkt(double p1x, double p1z)
       {
           int i = counter;
           int j;
           int z = 0;
           int bestleg = counter;
           CWegpunkt WP1 = ggw_WP_Start();
           CWegpunkt WP2;
           double kursv = 0d;
           double abstand = 999999;
           double minabstand = 999999;
           double px=0, py=0;
           double relativ_zu_abschnitt = 0;
           while(true) 
           {
               j=next_wegpunkt_counter(i);
               if (j == -1) break;
               WP2= ListeWegpunkt[j];

               abstand = AIMath.Abstand_P1_AbschnittP2P3(ref relativ_zu_abschnitt,p1x, p1z, WP1.x, WP1.z, WP2.x, WP2.z,ref px,ref py);
               if (relativ_zu_abschnitt<=1 || next_wegpunkt_counter(j)==-1) 
               {
                   if (abstand < minabstand)
                   {
                       minabstand = abstand;
                       bestleg = i;
                   }
               }
               i = next_wegpunkt_counter(i);
               z++;
               if (z > 5) break;
               WP1 = WP2;
           }

           return bestleg;
       }
       
       public CWegpunkt ggw_WP_Start() { if (counter < ListeWegpunkt.Count) return ListeWegpunkt[counter];else return null; }
       public CWegpunkt ggw_WP_Ziel()
       {
           if (ListeWegpunkt.Count == 0) return null;
           int i = next_wegpunkt_counter(counter);
           if (i >= 0) return ListeWegpunkt[i];
           return null;
       }
       public double Kurs(int wegpunktcount)
       {
           if (wegpunktcount >= ListeWegpunkt.Count) return double.NaN;
           CWegpunkt WP0 = ListeWegpunkt[wegpunktcount];
           int i = next_wegpunkt_counter(wegpunktcount);
           if (i == -1) return double.NaN;
           CWegpunkt WP1 = ListeWegpunkt[i]; 
           return AIMath.Absolut_Peilung_Punkt(WP0.x ,WP0.z,WP1.x,WP1.z);
       }
       public double ggwKurs()
       {
           if (ListeWegpunkt.Count==0) return double.NaN;
           CWegpunkt WP0 = ggw_WP_Start();
           CWegpunkt WP1 = ggw_WP_Ziel(); 
           return AIMath.Absolut_Peilung_Punkt(WP0.x ,WP0.z,WP1.x,WP1.z);
       }
       
       public double folgeKurs()
       {
           int nxt = next_wegpunkt_counter(counter);
           if (nxt<0) return double.NaN;
           if (ListeWegpunkt.Count==0) return double.NaN;
           return Kurs(nxt);
       }
       
       public void Map_Anzeige(Color? col=null, bool bnurlinien=false)
       {
           CWegpunkt WP0,WP1;
           List<double2> ld2 = new List<double2>();
           for (int i = 0; i < ListeWegpunkt.Count; i++)
           {
               ld2.Add(new double2(ListeWegpunkt[i].lat, ListeWegpunkt[i].lon));
               // WP0 = ListeWegpunkt[i];WP1 = ListeWegpunkt[i+1];
               // ListeEMP.Add(AIMap.Linie(WP0.lat, WP0.lon, WP1.lat, WP1.lon, 4, col??Color.gray));
               if (bnurlinien == false)
               {
                   ListeNO.Add(AIMap.Punkt(ListeWegpunkt[i].lat, ListeWegpunkt[i].lon, 6, col ?? Color.gray));
               }
           }
           if (ld2.Count>1) PL=AIMap.Polylinie(ld2, 4, col??Color.gray,name,ListeSymbols );
       }

       public void Map_Entferne()
       {
           foreach (NauticObject NO in ListeNO) if (NO) AIglobal.m_ObjSpawnerSO.DeleteNauticObject(NO);
           if (PL) AIglobal.m_channel_ui.DeletePolyLine(PL.Data.UID);
       }
   }
