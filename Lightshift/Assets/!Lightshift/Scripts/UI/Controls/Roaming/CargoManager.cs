using Lightshift;
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
        Settings.KeysLocked = true;

        _player = FindObjectsOfType<Player>().FirstOrDefault(p => p.isLocalPlayer);
        _ship = _player.ship;

        UpdateCargo();
    }

    public void UpdateCargo() 
    {
        if (_ship == null)
        {
            DialogManager.ShowMessage("An error occured loading your cargo.");
            return;
        }
        var cargoItems = _player.GetResources().Where(c => c.Amount > 0);
        cargoItems.Max(s => s.Amount);

        foreach (var cargo in cargoItems)
        {
            var control = _controls.FirstOrDefault(s => s.type == cargo.Type);
            if (control == null)
            {
                var item = Instantiate(_cargoItemPrefab, _contentPanel);
                control = item.GetComponent<CargoItemControl>();
                control.onEject += (thing) =>
                {
                    DialogManager.ShowEjectDialog($"You have {cargo.Amount} {cargo.Type}. Enter the amount you want to eject.", cargo.Amount, delegate (bool result, int amount) 
                    {
                        if (result)
                        {
                            _player.EjectResource(cargo.Type, amount);
                            Exit();
                        }
                    });
                };
            }

            control.Init(cargo.Type, cargo.Amount);
        }

        try
        {
            _capacityLabel.text = $"{cargoItems.Sum(s => s.Amount)}/{_ship.Modifiers[Modifier.Storage]}";
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
            {
                _player.EjectAllResources();
                Exit();
            }
        });
    }

    public void Exit() 
    {
        GameUIManager.Instance.ToggleCargoMenu();
    }

    private void OnDestroy()
    {
        Settings.KeysLocked = false;
    }
}
