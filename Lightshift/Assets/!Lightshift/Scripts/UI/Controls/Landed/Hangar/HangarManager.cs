using Assets._Lightshift.Scripts.Utilities;
using SharedModels.Models.Game;
using SharedModels.Models.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HangarManager : LandedState
{
    [SerializeField] private GameObject _overViewPanel;
    [SerializeField] private GameObject _modulePanel;
    [SerializeField] private StatView _statView;
    [SerializeField] private GameObject _loadoutPanel;
    [SerializeField] private GameObject _loadoutPrefab;


    private List<ModuleItemControl> _moduleControlList;

    private void Start()
    {
        _moduleControlList = _modulePanel.GetComponentsInChildren<ModuleItemControl>().ToList();
        foreach (var module in _moduleControlList)
        {
            module.OnClicked.AddListener(OnModuleClicked);
        }

        player = FindObjectsOfType<Player>().FirstOrDefault(p => p.isLocalPlayer);

        player.ShipLoadouts.Callback += ShipLoadouts_Callback;
        RefreshHangar();
        RefreshLoadouts();
    }

    private void OnDestroy()
    {
        if (player != null)
            player.ShipLoadouts.Callback -= ShipLoadouts_Callback;
    }
    private void ShipLoadouts_Callback(Mirror.SyncIDictionary<string, LoadoutObject>.Operation op, string key, LoadoutObject item)
    {
        RefreshLoadouts();
        RefreshHangar();
    }

    public void RefreshHangar() 
    {
        if (player == null)
            return;

        var activeShip = player.GetActiveLoadout();

        if (activeShip == null)
            return;

        foreach (var equip in activeShip.EquippedModules)
        {
            var module = player.GetItems().FirstOrDefault(m => m.ModuleId == equip.itemId);
            if (module.ModuleId == null)
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
        if (player == null)
            return;

        var ship = player.GetActiveLoadout();
        if (ship == null)
            return;

        var stats = StatHelper.GetStatsFromShip(player, ship);

        _statView.Clear();
        foreach (var stat in stats)
            _statView.AddStat(stat);
    }

    private void RefreshLoadouts() 
    {
        if (_loadoutControls != null && _loadoutControls.Count > 0)
            foreach (var loadout in _loadoutControls)
            {
                if (loadout.gameObject != null)
                    Destroy(loadout.gameObject);
            }

        _loadoutControls.Clear();

        foreach (var loadout in player.GetShipLoadouts())
           AddLoadout(loadout);
    }

    private List<LoadoutControl> _loadoutControls = new List<LoadoutControl>();
    private void AddLoadout(LoadoutObject loadout) 
    {
        var control = Instantiate(_loadoutPrefab, _loadoutPanel.transform).GetComponent<LoadoutControl>();
        control.Init(player, loadout);
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

        var activeShip = player.GetActiveLoadout();

        if (activeShip == null)
            return;

        Debug.Log($"Target type is {targetType}");

        var items = player.GetItems().Where(i => ItemService.GetItem(i.ModuleId) != null && ItemService.GetItem(i.ModuleId).Type == targetType).ToList();

        var itemView = DialogManager.ShowItemViewDialog();

        itemView.onClick += (Item item) =>
        {
            player.EquipModule(item.Id, moduleItemControl.ModuleLocation);

            itemView.Exit();
        };

        itemView.Init(items, $"Module Select: {targetType}");

    }

    public void ShowModuleList(ItemType type, string title) 
    {
        //var listView = DialogManager.CreateListView($"{title}");

        //var activeShip = player.GetActiveLoadout();

        //foreach (var m in player.GetItems())
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
}
