using System.Collections;
using System.Collections.Generic;
using Groupup;
using UnityEngine;

public class SettingsPopup : UIPopup
{
    public void OnClick_Restart()
    {
        SceneLoader.Instance.LoadPresetByName("MainMenu", true);
    }

    public void OnClick_Close()
    {
        PopupManager.Instance.Hide();
    }
}
