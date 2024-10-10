using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundsSignalPopup : UIPopup
{

    public void Init()
    {
        
    }
    
    
    public void OnClick_Submit()
    {
        PopupManager.Instance.Hide();
    }
}
