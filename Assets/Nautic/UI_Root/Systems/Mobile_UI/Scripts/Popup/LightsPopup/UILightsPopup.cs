using System.Collections.Generic;
using Groupup;
using UnityEngine;
using UnityEngine.Events;

public class UILightsSignalPopup : UIPopup
{

    [SerializeField] private ShipLights _lights;
    [SerializeField] private ShipSymbols _symbols;

    private Question _contextQuestion;
    private UnityAction _submitCallback;
    
    // does the user selected lights or symbols
    private bool _useLights;
    public bool UseLights => _useLights;

    private void OnEnable()
    {
        SetActive(_useLights);
    }


    // if this popup is opened by a question to get context answers question and unityaction is set.
    public void Init(Question question, UnityAction submitCallback)
    {
        _contextQuestion = question;
        _submitCallback = submitCallback;
    }
    
    /**
     * All lights with color. Top[0], Mid[1], MidLeft[2], MidRight[3] Bot[4], Left[5], Right[6]
     * In case _useLights is false its all symbols with shape, [0] = top, [1] = topLeft, [2] = topRight, [3] = midLeft, [4] = midRight, [5] = bottomLeft, [6] = bottomRight,
     */
    private List<int> GetLightsAndSymbols()
    {
        List<int> setSignales = null;

        if (UseLights)
        {
            setSignales = _lights.GetLights();
        }
        else
        {
            setSignales = _symbols.GetSymbols();
        }

        setSignales.Add(UseLights ? 1 : 0);

        return setSignales;
    }

    public void OnCLick_Submit()
    {
        // if no contextQuestion is set, this is used for normal gameplay
        if (_contextQuestion == null)
        {
            NauticObject obj = ResourceManager.GetInterface<ObjectsInterface>().SelectedObject;
            if (obj && obj.LightController)
            {
                obj.Data.LightsOrSymbols = GetLightsAndSymbols();
                obj.LightController.SetLightsAndSymbols(obj.Data.LightsOrSymbols);
            }
        }
        // otherwise it is used from a question to get context informations
        else
        {
            PopupManager.Instance.ShowQuestionPopup(_contextQuestion, _submitCallback, "Antwort vom Lichtpopup");
        }

        PopupManager.Instance.Hide();
    }
    
    public void SetActive(bool lights)
    {
        _useLights = lights;
        _lights.SetActive(lights);
        _symbols.SetActive(!lights);
    }
}

