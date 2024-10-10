using Groupup;
using UnityEngine;

public class CockpitController : MonoBehaviour
{
    [SerializeField] private GameObject _minesweeperCockpit;
    [SerializeField] private GameObject _cargoCockpit;

    private void Awake()
    {
        _minesweeperCockpit.SetActive(false);
        _cargoCockpit.SetActive(false);
    }

    public void SetCockpitLayout(NauticType type)
    {
        _minesweeperCockpit.SetActive(type == NauticType.MineSweeper);
        _cargoCockpit.SetActive(type == NauticType.Cargo);
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
