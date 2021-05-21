using Assets._Lightshift.Scripts.Utilities;
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
    [SerializeField] private GameObject _loadoutPanel;
    [SerializeField] private GameObject _loadoutPrefab;


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

        var activeShip = _player.GetActiveLoadout();

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
            RefreshLoadouts();
        }
    }
    private void RefreshStats() 
    {
        if (_player == null)
            return;

        var ship = _player.GetActiveLoadout();
        if (ship == null)
            return;

        var stats = StatHelper.GetStatsFromShip(_player, ship);

        _statView.Clear();
        foreach (var stat in stats)
            _statView.AddStat(stat);
    }

    private List<LoadoutObject> _loadouts = new List<LoadoutObject>();
    private void RefreshLoadouts() 
    {
        if (_player.GetShipLoadouts().Count != _loadouts.Count)
        {
            foreach (var loadout in _player.GetShipLoadouts())
            {
                var existing = _loadouts.FirstOrDefault(l => l.Id == loadout.Id);
                if (existing == null)
                    AddLoadout(loadout);
            }
        }
    }
    private void AddLoadout(LoadoutObject loadout) 
    {
        var control = Instantiate(_loadoutPrefab, _loadoutPanel.transform).GetComponent<LoadoutControl>();
        control.Init(_player, loadout);
        _loadouts.Add(loadout);
    }

    public void BuyLoadout() 
    {
        DialogManager.ShowMessage("Method not implemented.");
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
                targetType = ItemType.Hull;
                break;
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

        var activeShip = _player.GetActiveLoadout();

        if (activeShip == null)
            return;

        Debug.Log($"Target type is {targetType}");

        var items = _player.GetItems().Where(i => ItemService.GetItem(i.ModuleId) != null && ItemService.GetItem(i.ModuleId).Type == targetType).ToList();

        var itemView = DialogManager.ShowItemViewDialog();

        itemView.onEquip += (item) =>
        {
            _player.EquipModule(item.Id, item.ModuleLocation, delegate (string moduleId)
            {
                if (_player.isLocalPlayer)
                {
                    var existing = items.FirstOrDefault(s => s.ModuleLocation == item.ModuleLocation && activeShip.EquippedModules.Contains(s.Id));
                    if (existing != null)
                        _player.GetActiveLoadout().EquippedModules.Remove(existing.Id);

                    _player.GetActiveLoadout().EquippedModules.Add(moduleId);
                }

                RefreshHangar();
                itemView.Exit();
            });
        };

        itemView.Init(items, $"Module Select: {targetType}");

    }

    public void ShowModuleList(ItemType type, string title) 
    {
        //var listView = DialogManager.CreateListView($"{title}");

        //var activeShip = _player.GetActiveLoadout();

        //foreach (var m in _player.GetItems())
        //{
        //    var gameItem = ItemService.GetItem(m.ModuleId);
        //    if (gameItem == null)
        //        continue;

        //    if (gameItem.Type == type)
        //    {
        //        var control = listView.InstantiateItem(DialogManager.GetItemViewModuleControl()).GetComponent<ItemViewStatControl>();
        //        control.SetItem(m);
        //        control.SetButtonText("Upgrade");

        //        control.onClick += (viewControl) =>
        //        {
        //            Destroy(listView.gameObject);

        //            var upgradeView = DialogManager.CreateUpgradeView(m);
        //        };
        //    }
        //}
    }

    public void LeaveHangar() 
    {
        _player.TakeOff();
    }
}
