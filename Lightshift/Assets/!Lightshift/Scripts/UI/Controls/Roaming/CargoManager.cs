using SharedModels.Models.Game;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CargoManager : MonoBehaviour
{
    [SerializeField] private GameObject _cargoItemPrefab;
    [SerializeField] private Transform _contentPanel;
    [SerializeField] private TextMeshProUGUI _capacityLabel;
    private Player _player;
    private PlayerShip _ship;

    private List<CargoItemControl> _controls = new List<CargoItemControl>();
    void Start()
    {
        _player = FindObjectsOfType<Player>().FirstOrDefault(p => p.isLocalPlayer);
        _ship = FindObjectsOfType<PlayerShip>().FirstOrDefault(p => p.isLocalPlayer);

        UpdateCargo();
    }

    public void UpdateCargo() 
    {
        if (_ship == null)
        {
            DialogManager.ShowMessage("An error occured loading your cargo.");
            return;
        }
        var cargoItems = _ship.Cargo;

        foreach (var cargo in cargoItems)
        {
            var control = _controls.FirstOrDefault(s => s.type == cargo.Key);
            if (control == null)
            {
                var item = Instantiate(_cargoItemPrefab, _contentPanel);
                control = item.GetComponent<CargoItemControl>();
                control.onEject += (thing) =>
                {
                    DialogManager.ShowEjectDialog($"You have {cargo.Value} {cargo.Key}. Enter the amount you want to eject.", cargo.Value, delegate (bool result, int amount) 
                    {
                        if (result)
                            _player.EjectCargo(cargo.Key, cargo.Value);
                    });
                };
            }

            control.Init(cargo.Key, cargo.Value);
        }

        try
        {
            _capacityLabel.text = $"{cargoItems.Sum(s => s.Value)}/{_ship.Modifiers[Modifier.CargoCapacity]}";
        }
        catch 
        {
            _capacityLabel.text = $"0";

        }
    }

    public void EjectAllCargo() 
    {
        DialogManager.ShowDialog($"Are you sure you want to EJECT ALL of your cargo?", delegate (bool result)
        {
            if (result)
                _player.EjectAllCargo();
        });
    }

    public void Exit() 
    {
        GameUIManager.Instance.ToggleCargoMenu();
    }
}
