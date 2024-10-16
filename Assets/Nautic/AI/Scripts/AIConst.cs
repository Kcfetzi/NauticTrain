using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class AIConst
{
    public const double grad=Math.PI/180d; //RAD->GRAD
    public const double kn=0.514d;//0.514 kn->m/s
    public const double sm = 1852; //m

    public const int check_steps = 10;
    public const double delta_t = 0.5d;
    
    
    public const int cAuftrag_Fahre_Geschw = 0;
    public const int cAuftrag_Warte_Zeit = 1;
    public const int cAuftrag_Fahre_Wegpunkt = 2;

    public const int cAreaTyp_See = 0;
    public const int cAreaTyp_VTG = 1;
    public const int cAreaTyp_Fahrrinne = 2;
    public const int cAreaTyp_NOGO = 4;

    public const int cSchiffstyp_Minenjaeger = 0;
    public const int cSchiffstyp_Containerschiff = 1;
    public const int cSchiffstyp_Faehre = 2;
    public const int cSchiffstyp_Fischer = 3;
    public const int cSchiffstyp_Lotse = 4;
    
    public const int cOrtung_Sicht=1;
    public const int cOrtung_Radar=2;
    
    public const double penalty_Gegner_krit_Entf = 10d;
    public const double penalty_Gegner_Nahbereich = 100d;
    public const double penalty_Andere_krit_Entf = 5d;
    public const double penalty_verlorene_Minute = 0.1d;
    public const double penalty_zusaetzliche_sm = 0.2d;
    public const double penalty_fahrtaenderung = 5d;
    public const double penalty_VTG_Abweichung180_min = 3d;
    public const double penalty_NOGO = 999d;
    
    
    public const int cBedingung_Andrehpunkt=1;        //1.   Abstand von DNC, RUDERLAGE, NEUERKURS=X2,Y2,Kurs minimal
    public const int cBedingung_amGegnervorbei=2;     //2.   GEGNER achtern und Abstand>DISTANZ
    public const int cBedingung_Distanzgefahren=4;            //3.   DISTANZ zurückgelegt
    public const int cBedingung_AbstandGegnerkurs=8;  //4.   DISTANZ von GEGNER.Kurs
    public const int cBedingung_Zielkurserreicht=16;
    public const int cBedingung_Warten = 32;
    public const int cBedingung_aufPunktzu = 64;
    public const int cBedingung_Zeit = 128;
    public const int cBedingung_peile_Gegner_min = 256;
    public const int cBedingung_peile_Gegner_achtern = 512;
    public const int cBedingung_peile_Gegner_voraus = 1024;
    public const int cBedingung_aufstoppen_bis_Gegner_Distanz = 2048;
    public const int cBedingung_aufstoppen = 4096;
    
    public const double opt_Fahrtstufe_halten = -1001d;
    public const double opt_Kurs_Hundelinie = -1002d;
    public const double opt_dahinter_eigener_Kurs = -1003d;
    public const double opt_dahinter_Gegnerkurs = -1004d;
    public const double opt_Raute_eigener_Kurs = -1005d;
    public const double opt_Raute_Gegnerkurs = -1006d;
    public const double opt_Dreieck = -1007d;

    //double drehen_bis,   List<double> lkurs_parameter, 
    public const double drehen_um = -1011d;
    public const double drehen_stb_bis_gegner_peilung = -1012d;
    public const double drehen_bb_bis_gegner_peilung = -1013d;
    
    //double entfernen_bis, List<double> lfahren_parameter,
    public const double entfernen_bis_zeit = -1020d;
    public const double entfernen_bis_distanz = -1021d;
    public const double entfernen_bis_gegner_peilung = -1022d;
    public const double entfernen_bis_gegner_Abstand = -1023d;
    public const double entfernen_bis_track_Abstand= -1025d;
    
    
    
    //parallel_kurs
    public const double parallel_gegner_kurs = -1030d;
    public const double parallel_gegner_kurs180 = -1031d;
    public const double parallel_eigener_kurs = -1032d;
    public const double parallel_gegner_kurs90 = -1033d;
    public const double parallel_gegner_kurs270 = -1034d;
    
    //double parallel_bis,  List<double> lparallel_partameter,
    public const double parallel_bis_zeit = -1040d;
    public const double parallel_bis_distanz = -1041d;
    public const double parallel_bis_gegner_peilung = -1042d;
    public const double parallel_bis_gegner_achtern = -1043d;
    public const double parallel_bis_gegner_voraus = -1044d;
    public const double parallel_bis_gegner_Abstand = -1045d;
    
    public const double keine_aktion = -1999d;

}
