using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SharedModels.Models.Game;
using System.Linq;

public class LoadoutControl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameLabel;
    [SerializeField] ItemGraphicDisplay _wing1;
    [SerializeField] ItemGraphicDisplay _wing2;
    [SerializeField] ItemGraphicDisplay _hull;
    public LoadoutObject GetLoadout() => _loadoutObject;

    private LoadoutObject _loadoutObject;
    private Player _player;
    public void Init(Player player, LoadoutObject loadoutObject)
    {
        _loadoutObject = loadoutObject;

        if (_loadoutObject.Name == null)
            _loadoutObject.Name = "Default Loadout";

        _nameLabel.text = _loadoutObject.Name;

        _player = player;

        var equippedItems = player.GetItems().Where(e => loadoutObject.EquippedModules.Contains(e.Id));
        foreach (var equip in equippedItems)
        {
            var item = ItemService.GetItem(equip.ModuleId);

            switch (item.Type)
            {
                case ItemType.Wing:
                    _wing1.InitializeGraphic(item.Sprite);
                    _wing2.InitializeGraphic(item.Sprite);
                    break;
                case ItemType.Hull:
                    _hull.InitializeGraphic(item.Sprite);
                    break;
            }
        }
    }

    public void ChangeName() 
    {
        DialogManager.ShowInput($"Enter new loadout Name", _loadoutObject.Name, delegate (string name) 
        {
            _nameLabel.text = name;
            _player.RenameLoadout(_loadoutObject.Id, name);
        });
    }

    public void SelectLoadout()
    {
        _player.ChangeLoadout(_loadoutObject.Id);
    }
}
