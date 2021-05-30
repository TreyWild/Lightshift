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

        _player.ShipLoadouts.Callback += ShipLoadouts_Callback;
        RefreshHangar();
        RefreshLoadouts();
    }

    private void ShipLoadouts_Callback(Mirror.SyncIDictionary<string, LoadoutObject>.Operation op, string key, LoadoutObject item)
    {
        RefreshLoadouts();
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
            var module = _player.GetItems().FirstOrDefault(m => m.Id == equip.itemId);
            if (module == null)
                continue;

            var item = ItemService.GetItem(module.ModuleId);
            Debug.Log($"Loading Ship Module {item.name}");
            var control = _moduleControlList.FirstOrDefault(m => m.ModuleLocation == equip.location);
            control.SetItem(item);
        }

        RefreshStats();
    }

    //private float _timeSinceLastRefresh;
    //private void Update()
    //{
    //    _timeSinceLastRefresh += Time.deltaTime;
    //    if (_timeSinceLastRefresh > .1f)
    //    {
    //        _timeSinceLastRefresh = 0;
    //        RefreshStats();
    //        RefreshLoadouts();
    //    }
    //}
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

    private void RefreshLoadouts() 
    {
        if (_loadoutControls != null && _loadoutControls.Count > 0)
            foreach (var loadout in _loadoutControls)
                Destroy(loadout.gameObject);

        _loadoutControls.Clear();
        foreach (var loadout in _player.GetShipLoadouts())
           AddLoadout(loadout);
    }

    private List<LoadoutControl> _loadoutControls = new List<LoadoutControl>();
    private void AddLoadout(LoadoutObject loadout) 
    {
        var control = Instantiate(_loadoutPrefab, _loadoutPanel.transform).GetComponent<LoadoutControl>();
        control.Init(_player, loadout);
        _loadoutControls.Add(control);
    }

    public void BuyLoadout() 
    {
        DialogManager.ShowMessage("Method not implemented.");
    }

    private void OnModuleClicked(ModuleItemControl moduleItemControl)
    {
        var targetType = ItemType.None;
        switch (moduleItemControl.ModuleLocation)
        {
            case ModuleLocation.Engine:
                targetType = ItemType.Engine;
                break;
            case ModuleLocation.Hull:
                targetType = ItemType.Hull;
                break;
            case ModuleLocation.PrimaryWings:
                targetType = ItemType.Wing;
                break;
            case ModuleLocation.SecondaryWings:
                targetType = ItemType.Wing;
                break;
            case ModuleLocation.Weapon:
                targetType = ItemType.Weapon;
                break;
            case ModuleLocation.Weapon1:
                targetType = ItemType.Weapon;
                break;
            case ModuleLocation.Weapon2:
                targetType = ItemType.Weapon;
                break;
            case ModuleLocation.Weapon3:
                targetType = ItemType.Weapon;
                break;
            case ModuleLocation.Weapon4:
                targetType = ItemType.Weapon;
                break;
            case ModuleLocation.Weapon5:
                targetType = ItemType.Weapon;
                break;
        }

        var activeShip = _player.GetActiveLoadout();

        if (activeShip == null)
            return;

        Debug.Log($"Target type is {targetType}");

        var items = _player.GetItems().Where(i => ItemService.GetItem(i.ModuleId) != null && ItemService.GetItem(i.ModuleId).Type == targetType).ToList();

        var itemView = DialogManager.ShowItemViewDialog();

        itemView.onClick += (Item item) =>
        {
            _player.EquipModule(item.Id, moduleItemControl.ModuleLocation, delegate (string moduleId)
            {
                if (_player.isLocalPlayer)
                {
                    if (activeShip.EquippedModules == null)
                        activeShip.EquippedModules = new EquipObject[0];

                    // Ensure there's enough room in the array
                    bool hasExistingItem = activeShip.EquippedModules.Any(s => s.location == moduleItemControl.ModuleLocation);
                    if (!hasExistingItem)
                    {
                        //var list = item.Upgrades.ToList();
                        //list.Add(new Upgrade());
                        //item.Upgrades = list.ToArray();
                        activeShip.EquippedModules = activeShip.EquippedModules.Append(new EquipObject { location = moduleItemControl.ModuleLocation });
                    }

                    var module = ItemService.GetItem(moduleId);

                    if (module == null || module.InstallLocations == null)
                        return;

                    bool allowedAction = module.InstallLocations.Contains(moduleItemControl.ModuleLocation);

                    if (!allowedAction)
                    {
                        DialogManager.ShowMessage("You tried to equip a module where it's not allowed.");
                        return;
                    }

                    // Equip Module
                    for (int i = 0; i < activeShip.EquippedModules.Count(); i++)
                    {
                        var equip = activeShip.EquippedModules[i];
                        if (equip.location == moduleItemControl.ModuleLocation) {
                            activeShip.EquippedModules[i].itemId = item.Id;
                            continue;
                        }

                        //remove existing equips in other locations
                        if (equip.itemId == item.Id)
                            activeShip.EquippedModules[i].itemId = null;
                    }
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
