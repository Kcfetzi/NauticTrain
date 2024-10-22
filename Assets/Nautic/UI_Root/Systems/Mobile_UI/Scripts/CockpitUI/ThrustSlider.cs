
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThrustSlider : MonoBehaviour
{
    [SerializeField] private CockpitUIController _cockpitUIController;
    
    [SerializeField] private TMP_Text _thrustText;
    [SerializeField] private TMP_Text _wantedThrustText;
    [SerializeField] private Slider _thrustSlider;
    
    private void Awake()
    {
        _thrustSlider.onValueChanged.AddListener(SetWantedThrust);
    }

    public void SetThrust(float thrust)
    {
        _thrustSlider.value = (int)thrust;
        _thrustText.text = (int)thrust + " KN";
        _wantedThrustText.text = (int)thrust + " KN";
    }

    public void SetWantedThrust(float thrust)
    {
        int amount = Mathf.Clamp((int)thrust,-5, 18);
        _cockpitUIController.SetWantedThrust(amount);
        _wantedThrustText.text = amount + " KN";
    }
}
