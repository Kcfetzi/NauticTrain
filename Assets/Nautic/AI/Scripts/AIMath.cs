using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class AIMath
{
    public const double grad = AIConst.grad;
    
    
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::Geometrische Grundfunktionen::::::::::::::::::::::::::::::::::::::
    //Absolutpeilung -180<Absolut_Peilung_Punkt<180
    public static double Absolut_Peilung_Punkt(double posx, double posz, double px, double pz)
    {
        double gw = Math.Atan2(px-posx,pz-posz)*180 / Math.PI;
        if (gw < 0) gw += 360;
        return gw;
    }
    
    //Relativpeilung
   public static double Relativpeilung_Peilung_Punkt(double posx, double posz, double bearing, double px, double pz)
   {
       double ap = AIMath.Absolut_Peilung_Punkt(posx, posz, px, pz);
       double rp = Diff_Winkel(bearing, ap);
       return rp;
   }
   
   
   //Abstand Punkt-Gerade, Schnittpunkt wird zurückgegeben
   //Es kann ermittelt werden, ob der Schnittpunkt Peilung bb,stb, voraus oder acheraus liegt
   public static double Abstand_Punkt_Gerade(double px, double py, double gx, double gy, double gw)
   {
       double psx = 0, psy = 0;
       bool b=Schnitt_Gerade1_Gerade2(gx, gy, gw, px, py, gw + 90, ref psx, ref psy);
       return Math.Sqrt((px-psx)*(px-psx)+(py-psy)*(py-psy));
   }
   
   //Problem Bsp Winkel 1=10 Grad, Winkel 2=340 Grad ->Resutat müsste sein -30 Grad
   // winkel 1 zu winkel 2: 10-340=-330
   public static double Diff_Winkel(double winkel_von, double winkel_nach)
   {
       double diff_winkel = AIMath.norm(winkel_nach - winkel_von,180);
       //if (diff_winkel > 180) diff_winkel -= 360;
       //if (diff_winkel < -180) diff_winkel += 360;
       return diff_winkel;
   }
   
   public static bool Winkel_durchlaufen(double zielwinkel,double winkel1, double winkel2)
   {
       double d1 = Diff_Winkel(winkel1, zielwinkel);
       double d2=Diff_Winkel(winkel2, zielwinkel);
       int s1=Math.Sign(d1);
       int s2=Math.Sign(d2);
       return (s1 != s2 && s1!=0 && Math.Abs(d1)<90d && Math.Abs(d2)<90d);
   }
   
   public static double Dist_XZ(double p1x,double p1z, double p2x, double p2z)
   {
       return Math.Sqrt((p1x - p2x) * (p1x - p2x) + (p1z - p2z) * (p1z - p2z));
   }

   public static double norm(double winkel, int norm)
   {
       double w = winkel;
       if (norm == 360)
       {
           while (w >=360) w -= 360;
           while (w < 0) w += 360;
           return w;
       }
       else
       {
           while (w > 180) w -= 360;
           while (w <= -180) w += 360;
           return w;
       }
   }
   // (1)
   // g1x+a*sin(g1w)=g2x+b*sin(g2w)  |*cos(g1w)
   // g1y+a*cos(g1w)=g2y+b*cos(g2w)  |*sin(g1w)  
   // g1x*cos(g1w)+a*sin(g1w)*cos(g1w)=g2x*cos(g1w)+b*sin(g2w)*cos(g1w)
   // g1y*sin(g1w)+a*cos(g1w)*sin(g1w)=g2y*sin(g1w)+b*cos(g2w)*sin(g1w)
   // Subtraktion
   // g1x*cos(g1w)-g1y*sin(g1w)=g2x*cos(g1w)-g2y*sin(g1w)+b*(sin(g2w)*cos(g1w)-cos(g2w)*sin(g1w))
   // b=(g1x*cos(g1w)-g1y*sin(g1w)-g2x*cos(g1w)+g2y*sin(g1w))/(sin(g2w)*cos(g1w)-cos(g2w)*sin(g1w))
   // px=g2x+b*sin(g2w);g2y+b*cos(g2w)
   // (2)
   // g1x+a*sin(g1w)=g2x+b*sin(g2w)  |*cos(g2w)
   // g1y+a*cos(g1w)=g2y+b*cos(g2w)  |*sin(g2w) 
   // g1x*cos(g2w)+a*sin(g1w)*cos(g2w)=g2x*cos(g2w)+b*sin(g2w)*cos(g2w) 
   // g1y*sin(g2w)+a*cos(g1w)*sin(g2w)=g2y*sin(g2w)+b*cos(g2w)*sin(g2w)  
   // Subtraktion
   // g1x*cos(g2w)-g1y*sin(g2w)+a*(sin(g1w)*cos(g2w)-cos(g1w)*sin(g2w))=g2x*cos(g2w)-g2y*sin(g2w)
   // a=(g2x*cos(g2w)-g2y*sin(g2w)-g1x*cos(g2w)+g1y*sin(g2w))/(sin(g1w)*cos(g2w)-cos(g1w)*sin(g2w))
   // px=g1x+a*sin(g1w);py=g1y+a*cos(g1w)
   
   public static bool Schnitt_Gerade1_Gerade2(double g1x, double g1y, double g1winkel, double g2x, double g2y, double g2winkel,ref double px, ref double py)
   {
       double g1w = g1winkel * grad;
       double g2w = g2winkel * grad;
       double detb=(Math.Sin(g2w)*Math.Cos(g1w)-Math.Cos(g2w)*Math.Sin(g1w));
       double deta=(Math.Sin(g1w)*Math.Cos(g2w)-Math.Cos(g1w)*Math.Sin(g2w));
       if (deta != 0d)
       {
           double a = (g2x * Math.Cos(g2w) - g2y * Math.Sin(g2w) - g1x * Math.Cos(g2w) + g1y * Math.Sin(g2w)) / deta;
           px=g1x+a*Math.Sin(g1w); py = g1y + a * Math.Cos(g1w);
           return true;
       }
       if (detb != 0d)
       {
           double b=(g1x*Math.Cos(g1w)-g1y*Math.Sin(g1w)-g2x*Math.Cos(g1w)+g2y*Math.Sin(g1w))/detb;
           px=g2x+b*Math.Sin(g2w); py = g1y + b * Math.Cos(g2w);
           return true;
       }
       return false;
   }
   
   
   
   //g1x+a*(g2x-g1x)=p1x+b*(p2x-p1x)    a<=0<=1, 0<=b<=1     a==(p1x-g1x+b*(p2x-p1x))/(g2x-g1x))
   //g1y+a*(g2y-g1y)=p1y+b*(p2y-p1y)    a<=0<=1, 0<=b<=1     g1y+(p1x-g1x+b*(p2x-p1x))/(g2x-g1x))*(g2y-g1y)=p1y+b+(p2xy-p1y)
   //g1y+ (p1x-g1x+b*(p2x-p1x))/(g2x-g1)) *(g2y-g1y) = p1y+b*(p2y-p1y) | *(g2x-g1x)
   //g1y*(g2x-g1x)+ b* (p2x-p1x) * (g2y-g2x)= p1y*(g2x-g1x)+b*(p2y-p1y)*(g2x-g1)
   //b*(p2x-p1x)*(g2y-g2x)-b*(p2y-p1y)*(g2x-g1x)=p1y*(g2x-g1x)-g1y*(g2x-g1x)
   //b=[p1y*(g2x-g1x)-g1y*(g2x-g1x)]/[(p2x-p1x)*(g2y-g2x)-(p2y-p1y)*(g2x-g1x)]
   
   //g1x+a*(g2x-g1x)=p1x+b*(p2x-p1x)    a<=0<=1, 0<=b<=1     b=(g1x-p1x+a*(g2x-g1x))/(p2x-p1x)
   //g1y+a*(g2y-g1y)=p1y+b*(p2y-p1y)    a<=0<=1, 0<=b<=1     g1y+a*(g2y-g1y)=p1y+((g1x-p1x+a*(g2x-g1x))/(p2x-p1x))*(p2y-p1y)
   //g1y+a*(g2y-g1y)=p1y+((g1x-p1x+a*(g2x-g1x))/(p2x-p1x))*(p2y-p1y)                   |*(p2x-p1x)
   //g1y*(p2x-p1x)+a*(g2y-g1y)*(p2x-p1x)=p1y*(p2x-p1x)+(g1x-p1x+a*(g2x-g1x))*(p2y-p1y)  
   
   public static bool Schnitt_Gerade1XZ_Gerade2XZ(double g1x, double g1y, double g2x, double g2y, double p1x, double p1y, double p2x, double p2y,ref double px, ref double py)
   {
       double a=0;
       double b=0;
       ;
       double detb=(p2x-p1x)*(g2y-g1y)-(p2y-p1y)*(g2x-g1x);
       double deta=(g2x-g1x)*(p2y-p1y)-(g2y-g1y)*(p2x-p1x);
       if (deta != 0d)
       {
           a = ((g1y-p1y)*(p2x-p1x)-(g1x-p1x)*(p2y-p1y)) / deta;
           if (a < 0 || a > 1) return false;
           px=g1x+a* (g2x-g1x); py = g1y + a * (g2y-g1y);

       }
       if (detb != 0d)
       {
           b=((p1y-g1y)*(g2x-g1x)-(p1x-g1x)*(g2y-g1y))/detb;
           if (b < 0 || b > 1) return false;
           px=p1x+b* (p2x-p1x); py = p1y + b * (p2y-p1y);
       }
       
       return true;
   }
   
   
   
   public static bool Schnitt_Richtung1_auf_Gerade23(double g1x, double g1y, double g1winkel, double g2x, double g2y, double g3x,double g3y,ref double px, ref double py)
   {
       double g1w = g1winkel * grad;
       double g2w = Absolut_Peilung_Punkt(g2x,g2y,g3x,g3y) * grad;
       
       //double detb=(Math.Sin(g2w)*Math.Cos(g1w)-Math.Cos(g2w)*Math.Sin(g1w));
       // g1x+a*sin(g1w)=g2x+b*sin(g2w)  |*cos(g1w)                     :sin(g2w)=>(g3x-g2x)           
       // g1y+a*cos(g1w)=g2y+b*cos(g2w)  |*sin(g1w)                     :cos(g2w)=(g3y-g2y) 
       
       double detb=((g3x-g2x)*Math.Cos(g1w)-(g3y-g2y)*Math.Sin(g1w));
       double deta=(Math.Sin(g1w)*(g3y-g2y)-Math.Cos(g1w)*(g3x-g2x));
       
       double a = -1d, b = -1d;
       
       if (detb != 0d)
       {
           b=(g1x*Math.Cos(g1w)-g1y*Math.Sin(g1w)-g2x*Math.Cos(g1w)+g2y*Math.Sin(g1w))/detb;
           
           px=g2x+b*(g3x-g2x); py = g2y + b * (g3y-g2y);
            
       }
      
       if (deta != 0d)
       {
            a = (g2x * (g3y-g2y) - g2y * (g3x-g2x)  - g1x * (g3y-g2y) + g1y * (g3x-g2x) ) / deta;
            px=g1x+a*Math.Sin(g1w); py = g1y + a * Math.Cos(g1w);
            
       }
       

       if (a > 0 && b > 0 && b < 1)
       {
           return true;
       }
       return false;
   }
   
   
   
   // (1)
   // g1x+a*sin(g1w)=g2x+b*sin(g2w)  |*cos(g1w)                     :sin(g2w)=>(g3x-g2x)           
   // g1y+a*cos(g1w)=g2y+b*cos(g2w)  |*sin(g1w)                     :cons(g2w)=(g3y-g2y)   
   // g1x*cos(g1w)+a*sin(g1w)*cos(g1w)=g2x*cos(g1w)+b*sin(g2w)*cos(g1w)
   // g1y*sin(g1w)+a*cos(g1w)*sin(g1w)=g2y*sin(g1w)+b*cos(g2w)*sin(g1w)
   // Subtraktion
   // g1x*cos(g1w)-g1y*sin(g1w)=g2x*cos(g1w)-g2y*sin(g1w)+b*(sin(g2w)*cos(g1w)-cos(g2w)*sin(g1w))
   // b=(g1x*cos(g1w)-g1y*sin(g1w)-g2x*cos(g1w)+g2y*sin(g1w))/(sin(g2w)*cos(g1w)-cos(g2w)*sin(g1w))
   // b=(g1x*cos(g1w)-g1y*sin(g1w)-g2x*cos(g1w)+g2y*sin(g1w))/((g3x-g2x)*cos(g1w)-(g3y-g2y)*sin(g1w))
   // px=g2x+b*sin(g2w);g2y+b*cos(g2w)
   // (2)
   // g1x+a*sin(g1w)=g2x+b*sin(g2w)  |*cos(g2w)
   // g1y+a*cos(g1w)=g2y+b*cos(g2w)  |*sin(g2w) 
   // g1x*cos(g2w)+a*sin(g1w)*cos(g2w)=g2x*cos(g2w)+b*sin(g2w)*cos(g2w) 
   // g1y*sin(g2w)+a*cos(g1w)*sin(g2w)=g2y*sin(g2w)+b*cos(g2w)*sin(g2w)  
   // Subtraktion
   // g1x*cos(g2w)-g1y*sin(g2w)+a*(sin(g1w)*cos(g2w)-cos(g1w)*sin(g2w))=g2x*cos(g2w)-g2y*sin(g2w)
   // a=(g2x*cos(g2w)-g2y*sin(g2w)-g1x*cos(g2w)+g1y*sin(g2w))/(sin(g1w)*cos(g2w)-cos(g1w)*sin(g2w))
   // px=g1x+a*sin(g1w);py=g1y+a*cos(g1w)
   
   
   // Wie weit ist ein Punkt von einem Abschnitt entfernt (rechtwinklig)
   // geliefert wird -1, falls ausserhalb des Aschnitts
   // ferner wird mit faktorabschnitt geliefert, ob man sich vor dem Startpunkt (<0) oder nach dem Endpunkt(>0) befindet
   public static double Abstand_P1_AbschnittP2P3(ref double faktorabschnitt,double g1x, double g1y,  double g2x,double g2y, double g3x,double g3y ,ref double px, ref double py)
   {
       
       double g1w = Absolut_Peilung_Punkt(g2x,  g2y, g3x, g3y)+90;
       if (g1w > 360) g1w= g1w - 360;
       g1w = g1w * grad;
       
       // g1x+a*sin(g1w)=g2x+b*sin(g2w)  |*cos(g1w)                     :sin(g2w)=>(g3x-g2x)           
       // g1y+a*cos(g1w)=g2y+b*cos(g2w)  |*sin(g1w)                     :cos(g2w)=(g3y-g2y) 
       
       
       double detb=((g3x-g2x)*Math.Cos(g1w)-(g3y-g2y)*Math.Sin(g1w));
       double deta=(Math.Sin(g1w)*(g3y-g2y)-Math.Cos(g1w)*(g3x-g2x));
       faktorabschnitt = 0d;
       if (detb != 0d)
       {
            double b=(g1x*Math.Cos(g1w)-g1y*Math.Sin(g1w)-g2x*Math.Cos(g1w)+g2y*Math.Sin(g1w))/detb;
            faktorabschnitt = b;
            px=g2x+b*(g3x-g2x); py = g2y + b * (g3y-g2y);
            
       }
       else
       {
           if (deta != 0d)
           {
                  double a = (g2x * (g3y-g2y) - g2y * (g3x-g2x)  - g1x * (g3y-g2y) + g1y * (g3x-g2x) ) / deta;
                  px=g1x+a*Math.Sin(g1w); py = g1y + a * Math.Cos(g1w);
                  return Dist_XZ(px,py,g1x,g1y);
           }
       }
       if (faktorabschnitt<0)  return Dist_XZ(g1x,g1y,g2x,g2y); //liegt aunsserhalb der Strecke, davor
       if (faktorabschnitt>1)  return Dist_XZ(g1x,g1y,g3x,g3y); //liegt aunsserhalb der Strecke, danach
       return Dist_XZ(px,py,g1x,g1y);
       
       return -1d;
   }
   
   
   //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::Dreisatz Grundfunktionen::::::::::::::::::::::::::::::::::::::
   //"i"=Interpolieren,"e"=Extrapolieren,"k"=nächstkleineres,"g"=nächstgrösseres
   public static double Wert_Ermitteln(string wie,double x0,List<double> x, List<double> w)
   {
       int i = 0,i_from,i_to, i_step;
       if (w.Count == 1) return w[0];
       if (x[0] < x[1]) { i_from = 0; i_to = x.Count-2; i_step = 1; } else { i_from = x.Count-1; i_to = 1; i_step = -1; }
       if (x0<x[i_from])
       {
           if (wie=="e") {i=i_from; goto dreisatz;}
           return w[i_from];
       }
       if (x0>x[i_to+i_step])
       {
           if (wie=="e") {i=i_to; goto dreisatz;}
           return w[i_to+i_step];
       }
       
       for (i=i_from;i<=i_to;i+=i_step)
       {
           if ( x0 <=x[i+i_step]) break;
       }
       if (wie == "g") return w[i + 1];
       if (wie == "k") return w[i];
       
       dreisatz:
        double w0=w[i] +((x0-x[i])/(x[i+i_step] - x[i]))  * (w[i+i_step] - w[i]), w1=w0 ;
        if (i <= i_to - 1) w1 = w[i+1] + ((x0 - x[i+1]) / (x[i + i_step+1] - x[i+1])) * (w[i + i_step+1] - w[i+1]);
        return w0 ;
   }

   public static double Wert_Ermitteln2(string wie, double x0,double y0,List<double> y, List<List<double>> llx, List<List<double>> llw)
   {
       int i = 0;
       double w0, w1;
       if (y0<y[0])
       {
           if (wie=="e") {i=0; goto dreisatz;}
           return Wert_Ermitteln(wie, x0, llx[0], llw[0]);
       }
       if (y0>y[y.Count-1])
       {
           if (wie=="e") {i=y.Count-2; goto dreisatz;}
           return Wert_Ermitteln(wie, x0, llx[y.Count-1], llw[y.Count-1]);
       }

       for (i=0;i<=y.Count-2;i++)
       {
           if ( y0 <=y[i+1]) break;
       }
       
       dreisatz:
       
       w0 = Wert_Ermitteln(wie, x0, llx[i], llw[i]);
       w1 = Wert_Ermitteln(wie, x0, llx[i+1], llw[i+1]);
       
       if (wie == "g") return w1;
       if (wie == "k") return w0;
      
       return w0 +((y0-y[i])/(y[i+1] - y[i]))  * (w1 - w0) ; 
   }

   
  
   
}
