﻿using Assets._Lightshift.Scripts.Utilities;
using SharedModels.Models.Game;
using SharedModels.Models.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HangarManager : MonoBehaviour
{
    [SerializeField] private GameObject _overViewPanel;
    [SerializeField] private GameObject _modulePanel;
    [SerializeField] private StatView _statView;

    private Player _player;

    private List<ModuleItemControl> _moduleControlList;

    private void Start()
    {
        _moduleControlList = _modulePanel.GetComponentsInChildren<ModuleItemControl>().ToList();
        foreach (var module in _moduleControlList)
        {
            module.OnClicked.AddListener(OnModuleClicked);
        }

        _player = FindObjectsOfType<Player>().FirstOrDefault(p => p.isLocalPlayer);

        RefreshHangar();
    }

    public void RefreshHangar() 
    {
        if (_player == null)
            return;

        var activeShip = _player.GetActiveShip();

        if (activeShip == null)
            return;

        foreach (var equip in activeShip.EquippedModules)
        {
            var module = _player.GetItems().FirstOrDefault(m => m.Id == equip);
            if (module == null)
                continue;

            var item = ItemService.GetItem(module.ModuleId);
            Debug.Log($"Loading Ship Module {item.name}");
            var control = _moduleControlList.FirstOrDefault(m => m.ModuleType == module.ModuleLocation);
            control.SetItem(item);
        }

        RefreshStats();
    }

    private float _timeSinceLastRefresh;
    private void Update()
    {
        _timeSinceLastRefresh += Time.deltaTime;
        if (_timeSinceLastRefresh > 1f)
        {
            _timeSinceLastRefresh = 0;
            RefreshStats();
        }
    }
    private void RefreshStats() 
    {
        if (_player == null)
            return;

        var ship = _player.GetActiveShip();
        if (ship == null)
            return;

        var stats = StatHelper.GetStatsFromShip(_player, ship);

        _statView.Clear();
        foreach (var stat in stats)
            _statView.AddStat(stat);
    }

    private void OnModuleClicked(ModuleItemControl item)
    {
        var targetType = ItemType.None;
        switch (item.ModuleType)
        {
            case ModuleType.Engine:
                targetType = ItemType.Engine;
                break;
            case ModuleType.Hull:
                ShowFleet();
                return;
            case ModuleType.PrimaryWings:
                targetType = ItemType.Wing;
                break;
            case ModuleType.SecondaryWings:
                targetType = ItemType.Wing;
                break;
            case ModuleType.Weapon:
                targetType = ItemType.Weapon;
                break;
            case ModuleType.Weapon1:
                targetType = ItemType.Weapon;
                break;
            case ModuleType.Weapon2:
                targetType = ItemType.Weapon;
                break;
            case ModuleType.Weapon3:
                targetType = ItemType.Weapon;
                break;
            case ModuleType.Weapon4:
                targetType = ItemType.Weapon;
                break;
            case ModuleType.Weapon5:
                targetType = ItemType.Weapon;
                break;
        }

        Debug.Log($"Opening {item.ModuleType} equip list");
        var listView = DialogManager.CreateListView($"Select: {item.ModuleType}");

        var activeShip = _player.GetActiveShip();

        if (activeShip == null)
            return;

        Debug.Log($"Target type is {targetType}");

        var items = _player.GetItems();
        foreach (var m in items)
        {
            var gameItem = ItemService.GetItem(m.ModuleId);
            if (gameItem == null)
                continue;

            if (gameItem.Type == targetType)
            {
                var control = listView.InstantiateItem(DialogManager.GetDefaultItemViewControl()).GetComponent<ItemViewControl>();
                control.SetDisplayName(gameItem.DisplayName);
                control.SetSprite(gameItem.Sprite);
                control.SetButtonText("Equip");
                control.onClick += (id) =>
                {
                    _player.EquipModule(m.Id, m.ModuleLocation, delegate (string moduleId) 
                    {
                        if (_player.isLocalPlayer)
                        {
                            var existing = items.FirstOrDefault(s => s.ModuleLocation == m.ModuleLocation && activeShip.EquippedModules.Contains(s.Id));
                            if (existing != null)
                                activeShip.EquippedModules.Remove(existing.Id);

                            _player.GetActiveShip().EquippedModules.Add(moduleId);
                        }

                        RefreshHangar();
                        listView.Close();
                    });
                };
            }
        }
    }

    public void ShowWings() => ShowModuleList(ItemType.Wing, "Wings");
    public void ShowWeapons() => ShowModuleList(ItemType.Weapon, "Weapons");
    public void ShowEngines() => ShowModuleList(ItemType.Engine, "Engines");

    public void ShowModuleList(ItemType type, string title) 
    {
        var listView = DialogManager.CreateListView($"{title}");

        var activeShip = _player.GetActiveShip();

        foreach (var m in _player.GetItems())
        {
            var gameItem = ItemService.GetItem(m.ModuleId);
            if (gameItem == null)
                continue;

            if (gameItem.Type == type)
            {
                var control = listView.InstantiateItem(DialogManager.GetItemViewModuleControl()).GetComponent<ItemViewStatControl>();
                control.SetItem(m);
                control.SetButtonText("Upgrade");

                control.onClick += (viewControl) =>
                {
                    Destroy(listView.gameObject);

                    var upgradeView = DialogManager.CreateUpgradeView(m);
                };
            }
        }
    }

    public void ShowFleet() 
    {
        var listView = DialogManager.CreateListView($"Fleet");


        foreach (var ship in _player.GetShipLoadouts())
        {
            var control = listView.InstantiateItem(DialogManager.GetItemViewShipControl()).GetComponent<ItemViewStatControl>();
            control.SetShip(_player, ship);
            control.SetButtonText("Upgrade");
            control.onClick += (viewControl) =>
            {
                Destroy(listView.gameObject);
            };

            control.onEquip += (statControl) => 
            {
                _player.ChangeShip(ship.Id, delegate (string shipId)
                {
                    RefreshHangar();
                    listView.Close();
                });
            };
        }
    }

    public void LeaveHangar() 
    {
        _player.TakeOff();
    }
}
