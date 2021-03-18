using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SharedModels.Models.Game;
using System.Linq;
using System;
using Assets._Lightshift.Scripts.Utilities;
using Michsky.UI.ModernUIPack;

public class ItemViewStatControl : ItemViewControl
{
    [SerializeField] StatView _statView;

    [SerializeField] ItemGraphicDisplay _wing1;
    [SerializeField] ItemGraphicDisplay _wing2;
    [SerializeField] ButtonManagerBasic _equipButton;

    private ShipObject _shipObject;
    private Item _item;
    public ShipObject GetShipObject() => _shipObject;
    public Item GetItem() => _item;

    public Action<ItemViewStatControl> onEquip;

    private void OnDestroy()
    {
        _statView = null;
        _wing1 = null;
        _wing2 = null;
        _equipButton = null;
        _shipObject = null;
        _item = null;
    }
    public void SetShip(Player player, ShipObject shipObject) 
    {
        _shipObject = shipObject;

        _equipButton.buttonText = "Use this ship";
        var equippedItems = player.GetItems().Where(e => shipObject.EquippedModules.Contains(e.Id));
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
                    SetSprite(item.Sprite);
                    SetLore(item.Lore);
                    SetDisplayName(item.DisplayName);
                    break;
            }
        }

        var stats = StatHelper.GetStatsFromShip(player, shipObject);
        foreach (var stat in stats)
            _statView.AddStat(stat);
    }

    public void SetItem(Item item)
    {
       var gameItem = ItemService.GetItem(item.ModuleId);
        //_wing1.Hide();
        //_wing2.Hide();

        SetSprite(gameItem.Sprite);
        SetDisplayName(gameItem.DisplayName);
        SetLore(gameItem.Lore);

        //_equipButton.buttonText = "Use this module";

        var stats = StatHelper.GetStatsFromItem(item);
        foreach (var stat in stats)
            _statView.AddStat(stat);

        _item = item;
    }

    public void OnSelectItem() 
    {
        onEquip?.Invoke(this);
    }
}
