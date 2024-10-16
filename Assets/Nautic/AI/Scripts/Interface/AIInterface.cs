
using UnityEngine;
using UnityEngine.Events;
using Groupup;

   /** 
   * Structurerfile
   * This is a generated file from Structurer.
   * If you edit this file, stick to the format from Funcs, UnityActions and methods, to keep on getting all benefits from Strukturer.
   */ 
public class AIInterface : InterfaceSOBase
{
    public int ScenarioChoice;
    
    public UnityAction OnInit_Szenario;
    public UnityAction OnUserInteractionStopped;

    public void Reset()
    {
        OnInit_Szenario = null;
        OnUserInteractionStopped = null;
    }
    
    public void Init_Szenario()
    {
        OnInit_Szenario?.Invoke();
    }
    public void UserInteractionStopped()
    {
        OnUserInteractionStopped?.Invoke();
    }
    
    
    
}
