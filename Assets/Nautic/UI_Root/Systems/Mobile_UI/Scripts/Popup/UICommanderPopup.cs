using MPUIKIT;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UICommanderPopup : UIPopup
{
    [SerializeField] private MPImage _commander;
    [SerializeField] private Button _next;
    [SerializeField] private TMP_Text _msg;
    
    private UnityAction _submitCallback;
    
    public void Init(string text, bool commanderLeft, UnityAction submitCallback = null)
    {
        
        RectTransform buttonRectTransform = _next.GetComponent<RectTransform>();
        RectTransform commanderRectTransform = _commander.rectTransform;
        if (commanderLeft)
        {
            // Setze die Anker und Pivot für linksbündig
            commanderRectTransform.anchorMin = new Vector2(0, 0.5f);
            commanderRectTransform.anchorMax = new Vector2(0, 0.5f);
            commanderRectTransform.pivot = new Vector2(0, 0.5f);

            // Setze den Abstand vom linken Rand
            commanderRectTransform.anchoredPosition = new Vector2(-165, 0);
            
            buttonRectTransform.anchorMin = new Vector2(1, 0f);
            buttonRectTransform.anchorMax = new Vector2(1, 0f);
            buttonRectTransform.pivot = new Vector2(1, 0.5f);
            
            buttonRectTransform.anchoredPosition = new Vector2(-50, 75);
        }
        else
        {
            // Setze die Anker und Pivot für rechtsbündig
            commanderRectTransform.anchorMin = new Vector2(1, 0.5f);
            commanderRectTransform.anchorMax = new Vector2(1, 0.5f);
            commanderRectTransform.pivot = new Vector2(1, 0.5f);

            // Setze den Abstand vom rechten Rand
            commanderRectTransform.anchoredPosition = new Vector2(165, 0);
            
            buttonRectTransform.anchorMin = new Vector2(0, 0f);
            buttonRectTransform.anchorMax = new Vector2(0, 0f);
            buttonRectTransform.pivot = new Vector2(0, 0.5f);
            
            buttonRectTransform.anchoredPosition = new Vector2(50, 75);
        }
        
        
        _msg.text = text;
        _submitCallback = submitCallback;
    }

    public void On_Click_Submit()
    {
        _submitCallback?.Invoke();
        PopupManager.Instance.Hide();
    }
}
