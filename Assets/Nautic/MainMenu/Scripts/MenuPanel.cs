using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuPanel : MonoBehaviour
{
    [SerializeField] private List<MenuButton> _buttons;

    public void Init(UnityAction<int> clickCallback)
    {
        foreach (MenuButton button in _buttons)
        {
            button.Init(clickCallback);
        }
    }
    
    public void SetButtonActive(int choice)
    {
        for (int i = 0; i < _buttons.Count; i++)
        {
            _buttons[i].SetActive(choice == i);
        }
    }
}
