using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public  class CMsgBox
{
    
      private NauticObject HighlightPos = new NauticObject();
      private float curr_timescale;
      private UnityAction<string> _var_Callback;
      public CMsgBox(string text, double lat, double lon, UnityAction<string> var_Callback = null)
      {
          if (AIglobal.bsuppressMsgBox) return;
          curr_timescale = Time.timeScale;
          _var_Callback = var_Callback;
          PopupManager.Instance.ShowInputPopup(text,callback_MsgBox); 
          AIMap.Punkt(lat, lon, 10, Color.red);
          Time.timeScale = 0.3f;
      }
      public CMsgBox(string text , UnityAction<string> var_Callback = null)
      {
          if (AIglobal.bsuppressMsgBox) return;
          curr_timescale = Time.timeScale;
          _var_Callback = var_Callback;
          PopupManager.Instance.ShowInputPopup(text,callback_MsgBox); 
          Time.timeScale = 0.3f;
      }
    
      private void callback_MsgBox(string txt)
      {
          _var_Callback?.Invoke(txt);
          if (HighlightPos!=null) AIglobal.m_ObjSpawnerSO.DeleteNauticObject(HighlightPos);
          Time.timeScale = (curr_timescale<1f) ? 1f  :curr_timescale;
      }
    
}
