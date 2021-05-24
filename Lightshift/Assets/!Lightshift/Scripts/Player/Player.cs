﻿using Assets._Lightshift.Scripts.Data;
using Assets._Lightshift.Scripts.Network;
using Assets._Lightshift.Scripts.SolarSystem;
using Assets._Lightshift.Scripts.Web;
using Mirror;
using SharedModels;
using SharedModels.Models.Game;
using SharedModels.Models.User;
using SharedModels.WebRequestObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SyncVar]
    public string lastCheckpointId;

    [SyncVar(hook = nameof(OnCreditsChanged))] public int Credits;
    [SyncVar(hook = nameof(OnBankCreditsChanged))] public int BankCredits;
    [SyncVar] public string Username;
    [SyncVar(hook = nameof(OnActiveLoadoutChanged))]
    public string ActiveLoadout;
    [SyncVar] public string Id;
    [SyncVar] public string LandedLocationId;
    [SyncVar] public bool IsLanded;
    private void OnCreditsChanged(int oldValue, int newValue)
    {
        onCreditsChanged?.Invoke(newValue);
    }
    private void OnBankCreditsChanged(int oldValue, int newValue)
    {
        onBankCreditsChanged?.Invoke(newValue);
    }

    private void OnActiveLoadoutChanged(string oldValue, string newValue)
    {
        _onShipChanged?.Invoke(newValue);
    }

    public readonly SyncDictionary<ResourceType, int> Resources = new SyncDictionary<ResourceType, int>();
    public readonly SyncDictionary<string, Item> Items = new SyncDictionary<string, Item>();
    public readonly SyncDictionary<string, LoadoutObject> ShipLoadouts = new SyncDictionary<string, LoadoutObject>();


    public PlayerShip ship { get; set; }

    [SyncVar]
    public Profile _profile;

    private Account _account;

    public Action<int> onCreditsChanged;

    public Action<int> onBankCreditsChanged;

    private Action<string> _onUpgradePurchased;
    private Action<string> _onResetUpgrades;
    private Action<string> _onShipChanged;
    private Action<string> _onModuleEquipped;

    private void OnDestroy()
    {
        if (isServer)
        {
            SaveAccount();

            Communication.ShowBroadcastAlert($"{_account.Profile.Username} has left the system.", Communication.AlertType.SystemMessage);
        }

        ship = null;
        _account = null;
        onCreditsChanged = null;
        onBankCreditsChanged = null;
        _onUpgradePurchased = null;
        _onResetUpgrades = null;
        _onShipChanged = null;
        _onModuleEquipped = null;
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        Debug.Log($"Has authority: {hasAuthority}");

        CmdInit();
    }
    public override void OnStartServer()
    {
        base.OnStartServer();

        // ADD PLAYER TO SERVER INDEX
        Server.AddPlayer(this);
    }

    private bool _loadoutsLoaded;
    private bool _accountLoaded;
    private bool _itemsLoaded;
    private bool _resourcesLoaded;
    private bool _initPreAccountData;
    private bool _requirePlayerInit;
    private void InitPlayerData() 
    {
        if (!_accountLoaded)
        {
            LoadProfile(() => InitPlayerData());
            return;
        }
        if (!_resourcesLoaded)
        {
            LoadResources(() => InitPlayerData());
            return;
        }
        if (!_itemsLoaded)
        {
            LoadItems(() => InitPlayerData());
            return;
        }
        if (!_loadoutsLoaded)
        {
            LoadLoadouts(() => InitPlayerData());
            return;
        }
        if (!_initPreAccountData)
        {
            SetupAccount();

            InitShipLoadout();

            UserJoinedBroadcast();

            _initPreAccountData = true;

            InitPlayerData();
            return;
        }

        if (!_requirePlayerInit)
        {
            _requirePlayerInit = true;
            TargetRequireInit();
            return;
        }
    }

    [TargetRpc]
    private void TargetRequireInit() 
    {
        StartCoroutine(WaitForLoadouts());
    }

    public IEnumerator WaitForLoadouts()
    {
        while (ShipLoadouts.Count == 0)
            yield return new WaitForSeconds(1f);

        CmdInit();
    }

    private void InitShipLoadout() 
    {
        // Ensure ship is selected
        if (ActiveLoadout == null || GetActiveLoadout() == null)
        {
            ActiveLoadout = ShipLoadouts.First().Key;
        }
    }

    private void UserJoinedBroadcast() 
    {
        Debug.Log($"{Username} initialized.");
        foreach (var player in Server.GetAllPlayers())
        {
            if (player == this)
                continue;

            Communication.ShowUserAlert(player.connectionToClient, $"{Username} connected.", Communication.AlertType.SystemMessage);
            Communication.ShowUserAlert(player.connectionToClient, $"{Username} entered the system", Communication.AlertType.ScreenDisplay);
        }
    }

    [Command]
    private void CmdInit() 
    {
        if (!_requirePlayerInit)
        {
            InitPlayerData();
            return;
        }
        else
        {
            SpawnShip();
            FinalInit();
        }
    }

    [TargetRpc]
    private void FinalInit()
    {
        GameUIManager.Instance.ToggleLoadingScreen(false);
    }

    public List<LoadoutObject> GetShipLoadouts()
    {
        var items = new List<LoadoutObject>();
        foreach (var item in ShipLoadouts)
            items.Add(item.Value);

        return items;
    }
    public List<Item> GetItems()
    {
        var items = new List<Item>();
        foreach (var item in Items)
            items.Add(item.Value);

        return items;
    }
    public List<ResourceObject> GetResources()
    {
        var items = new List<ResourceObject>();
        foreach (var item in Resources)
        {
            items.Add(new ResourceObject { Amount = item.Value, Type = item.Key });
        }
        return items;
    }
    public Item GetItem(string id)
    {
        if (!Items.ContainsKey(id))
            return Item.Empty();
        else return Items[id];

    }

    public LoadoutObject GetShipLoadout(string id)
    {
        if (!ShipLoadouts.ContainsKey(id))
            return LoadoutObject.Empty();
        else return ShipLoadouts[id];

    }

    #region Resources
    public void LoadResources(Action callback = null) 
    {
        _resourcesLoaded = true;

        if (_profile.Resources == null)
        {
            // Add resources to new account
            _profile.Resources = PlayerDefaults.GetTestResources();
        }

        foreach (var resource in _profile.Resources)
            AddResource(resource);


        callback.Invoke();
    }

    public ResourceObject GetResource(ResourceType type) 
    {
        if (!Resources.ContainsKey(type))
            return new ResourceObject { Amount = 0, Type = type };
        else return new ResourceObject { Amount = Resources[type], Type = type };

    }
    public void SetResource(ResourceObject resource) => SetResource(resource.Type, resource.Amount);
    public void SetResource(ResourceType type, int value)
    {
        if (isServer)
        {
            if (!Resources.ContainsKey(type))
                Resources.Add(type, value);
            else
                Resources[type] = value;
        }
    }

    public void EjectAllResources() 
    {
        var resources = Resources.ToList();
        foreach (var resource in resources)
            EjectResource(resource.Key, resource.Value);
    }

    public void EjectResource(ResourceType type, int amount) 
    {
        if (amount == 0)
            return;

        var obj = Instantiate(LightshiftNetworkManager.GetPrefab<DroppedItem>());

        var item = obj.GetComponent<DroppedItem>();
        item.Init(new ResourceObject { Type = type, Amount = amount }, ship);

        obj.transform.position = ship.transform.position + new Vector3(UnityEngine.Random.Range(0,5), UnityEngine.Random.Range(0, 5));
        NetworkServer.Spawn(obj);

        TakeResource(type, amount);
    }

    public void PickupResource(ResourceType type, int amount) 
    {
        // TO DO : Sound Effects, etc
        AddResource(new ResourceObject { Amount = amount, Type = type });
    }

    public void AddResource(ResourceObject resource) => AddResource(resource.Type, resource.Amount);
    public void AddResource(ResourceType type, int value)
    {
        if (isServer)
        {
            if (!Resources.ContainsKey(type))
                Resources.Add(type, value);
            else
                Resources[type] += value;
        }
    }

    public void TakeResource(ResourceType type, int value)
    {
        if (isServer)
        {
            if (!Resources.ContainsKey(type))
                Resources.Add(type, value);
            else
                Resources[type] -= value;

            if (Resources[type] < 0)
                Resources[type] = 0;
        }
    }

    public bool CheckResourceAffordable(List<ResourceObject> resources, float costMultiplier = 1)
    {
        foreach (var expense in resources)
        {
            bool affordable = (int)(expense.Amount * costMultiplier) <= GetResource(expense.Type).Amount;

            // NOT AFFORDABLE
            if (!affordable)
                return false;
        }

        return true;
    }

    public Item SpendResources(Item item, List<ResourceObject> resources, float costMultiplier)
    {
        var spentResources = item.SpentResources.ToList();
        if (spentResources == null)
            spentResources = new List<ResourceObject>();

        foreach (var resource in resources)
        {
            var cost = (int)(resource.Amount * costMultiplier);
            var amount = GetResource(resource.Type).Amount - cost;
            if (amount < 0)
                amount = 0;

            Debug.LogError($"Resource Type: {resource.Type}:{resource.Amount}");
            
            SetResource(resource.Type, amount);

            var spentResource = spentResources.FirstOrDefault(r => r.Type == resource.Type);
            if (!spentResources.Any(r => r.Type == resource.Type))
            {
                spentResource = new ResourceObject { Type = resource.Type, Amount = 0 };
                spentResources.Add(spentResource);
            }
            spentResource.Amount += cost;
        }

        item.SpentResources = spentResources.ToArray();

        return item;
    }

    #endregion

    public void SpawnShip()
    {
        // SPAWN PLAYER SHIP
        var obj = Instantiate(LightshiftNetworkManager.GetPrefab<PlayerShip>());
        ship = obj.GetComponent<PlayerShip>();
        ship.displayName = Username;

        NetworkServer.Spawn(obj, connectionToClient);

        if (IsLanded)
        {
            var station = LandableManager.GetLandableById(LandedLocationId);
            if (station != null)
            {
                ship.transform.position = station.transform.position;
                Land(LandedLocationId);
            }
        }
        else TakeOff();
    }
    public void LoadProfile(Action callback)
    {
        _accountLoaded = true;
        HttpService.Get("account/get", connectionToClient.authenticationData,
        delegate (Account account)
        {
            Debug.Log($"Connected [{account.CaseSensitiveUsername}].");
            _account = account;
            _profile = _account.Profile;
            if (_profile == null)
                _profile = new Profile
                {
                    Username = account.CaseSensitiveUsername,
                    Level = 1,
                };

            // Init Pre-play Fields
            Credits = _profile.Credits;
            Username = _profile.Username;
            ActiveLoadout = _profile.ActiveLoadout;
            lastCheckpointId = _profile.LastCheckPointId;
            IsLanded = _profile.IsLanded;
            LandedLocationId = _profile.LandedLocationId;
            Id = account.Id;

            callback?.Invoke();
        });
    }

    public NetworkConnection GetConnection() 
    {
        if (isServer)
            return connectionToClient;
        else
            return connectionToServer;
    }

    public void AddLoadout(LoadoutObject ship) 
    {
        _profile.ActiveLoadout = ship.Id;
        ShipLoadouts.Add(ship.Id, ship);

        Debug.Log($"New ship built for {_account.Profile.Username}.");

        SaveLoadout(ship);
    }
    public void SaveLoadout(LoadoutObject ship, Action callback = null)
    {
        if (!isServer)
            return;

        ship.UserId = _account.Id;

        ShipLoadouts[ship.Id] = ship;

        HttpService.Get("game/saveloadout", ship,
        delegate (bool result)
        {
            if (result)
                Debug.Log($"Ship saved.");
            else Debug.LogError("Ship was not saved.");

            callback?.Invoke();
        });
    }
    public void AddItem(Item item)
    {
        item.Id = Guid.NewGuid().ToString();

        if (!Items.ContainsKey(item.Id))
            Items.Add(item.Id, item);

        Debug.Log($"New item built for {Username}.");

        SaveItem(item);
    }
    public void SaveItem(Item item, Action callback = null)
    {
        if (!isServer)
            return;

        item.UserId = _account.Id;

        Items[item.Id] = item;

        HttpService.Get("game/saveitem", item,
        delegate (bool result)
        {
            if (result)
                Debug.Log($"Item saved.");
            else Debug.LogError("Item was not saved.");

            callback?.Invoke();
        });
    }

    public void SaveAccount()
    {
        if (isServer)
        {

            _profile.LastCheckPointId = lastCheckpointId;
            _profile.ActiveLoadout = ActiveLoadout;
            _profile.Username = Username;
            _profile.Credits = Credits;
            _profile.BankCredits = BankCredits;
            _profile.Resources = GetResources();
            _profile.IsLanded = IsLanded;
            _profile.LandedLocationId = LandedLocationId;
            _account.Profile = _profile;
            HttpService.Get("account/save", _account,
            delegate (Account account)
            {
                _account = account;
                _profile = account.Profile;
                Debug.Log($"Account for {_account.CaseSensitiveUsername} saved.");
            });
        }
    }

    [ClientRpc]
    private void RpcLand(string landableId)
    {
        // TO DO : Landing effect
        if (hasAuthority)
        {
            LandableManager.Land(landableId);
        }
    }

    [Command]
    private void CmdLand(string landableId) 
    {
        Land(landableId);
    }

    public void Land(string landableId) 
    {
        Debug.Log($"Requesting Land {landableId}");
        if (isServer)
        {
            RpcLand(landableId);

            LandedLocationId = landableId;

            IsLanded = true;

            SaveAccount();

            ship.SetLanding();
        }
        else CmdLand(landableId);
    }

    [ClientRpc]
    private void RpcTakeoff()
    {
        // TO DO : Landing effect
        if (hasAuthority)
        {
            GameUIManager.Instance.LeaveLandable();
        }
    }

    [Command]
    private void CmdTakeoff()
    {
        TakeOff();
    }

    public void TakeOff()
    {
        if (isServer)
        {
            var station = LandableManager.GetLandableById(LandedLocationId);
            if (station != null)
                ship.SetPosition(new Vector2(station.transform.position.x, station.transform.position.y));

            ship.kinematic.rotation = UnityEngine.Random.Range(0, 360);

            ship.InitLoadoutObject(GetActiveLoadout());
            //ship.SetCargo(GetActiveLoadout().Cargo);
            ship.Respawn();

            IsLanded = false;

            RpcTakeoff();
        }
        else CmdTakeoff();
    }

    public void LoadLoadouts(Action callback)
    {
        if (!isServer)
            return;

        _loadoutsLoaded = true;

        _account.Profile = _profile;
        HttpService.Get("game/getloadouts", new JsonString { Value = _account.Id },
        delegate (List<LoadoutObject> ships)
        {
            foreach (var ship in ships)
            {
                if (!ShipLoadouts.ContainsKey(ship.Id))
                    ShipLoadouts.Add(ship.Id, ship);
                else ShipLoadouts[ship.Id] = ship;
            }

            Debug.Log($"Loadouts for {Username} loaded: [{ships?.Count}]");

            callback?.Invoke();
        });
    }

    public void SetupAccount() 
    {
        // IF PLAYER HAS NO SHIPS, ADD DEFAULT
        if (ShipLoadouts == null || ShipLoadouts.Count == 0)
        {
            // Load default items
            var items = PlayerDefaults.GetDefaultItems();

            var existingItems = GetItems();
            foreach (var item in items)
            {
                //Ensure no duplicate defaults
                if (existingItems.FirstOrDefault(s=> s.ModuleId == item.ModuleId) == null)
                    AddItem(item);
            }

            // Add new ship object
            var newShip = new LoadoutObject();
            newShip.Id = Guid.NewGuid().ToString();
            var equips = new List<string>();

            foreach (var item in items)
                equips.Add(item.Id);

            newShip.EquippedModules = equips.ToArray();

            AddLoadout(newShip);

            // Setup Landed location
            LandedLocationId = PlayerDefaults.GetDefaultStation();

            // Save account
            SaveAccount();
        }
    }

    public void LoadItems(Action callback)
    {
        if (!isServer)
            return;

        _itemsLoaded = true;

        HttpService.Get("game/getitems", new JsonString { Value = _account.Id },
        delegate (List<Item> items)
        {
            foreach (var item in items)
            {
                if (!Items.ContainsKey(item.Id))
                    Items.Add(item.Id, item);
                else Items[item.Id] = item;
            }

            Debug.Log($"Server loaded [{Items?.Count}] items for player {Username}");

            callback?.Invoke();
        });
    }

    public LoadoutObject GetActiveLoadout() 
    {
        return GetShipLoadout(ActiveLoadout);
    }

    public void ChangeLoadout(string ship, Action<string> callback = null)
    {
        if (ActiveLoadout == ship)
            return;

        if (isLocalPlayer)
        {
            _onShipChanged = callback;
            CmdChangeLoadout(ship);
        }
    }

    [Command]
    private void CmdChangeLoadout(string id) 
    {
        //Ensure loadout exists
        if (!ShipLoadouts.ContainsKey(id))
            return;

        ActiveLoadout = id;
    }

    public void RenameLoadout(string loadoutId, string name)
    {
        if (isLocalPlayer)
        {
            CmdRenameLoadout(loadoutId, name);
        }
    }

    [Command]
    private void CmdRenameLoadout(string id, string name)
    {
        //Ensure loadout exists
        if (!ShipLoadouts.ContainsKey(id))
            return;

        var shipLoadout = ShipLoadouts[id];
        shipLoadout.Name = name;

        SaveLoadout(shipLoadout);
    }

    public void EquipModule(string id, ModuleType location, Action<string> callback = null)
    {
        if (isLocalPlayer)
        {
            _onModuleEquipped = callback;
            CmdEquipModule(id, (uint)location);
        }
    }

    [Command]
    private void CmdEquipModule(string id, uint location)
    {
        // Short for location
        var loc = (ModuleType)location;
        var ship = GetActiveLoadout();
        if (ship == null)
            return;

        var module = GetItem(id);
        if (module == null)
            return;

        var equippedModules = ship.EquippedModules.ToList() ;
        if (equippedModules == null)
            equippedModules = new List<string>();

        // Remove any old equips
        for (int i = 0; i < equippedModules.Count; i++)
        {
            var equip = ship.EquippedModules[i];
            var item = GetItem(equip);
            if (item.ModuleLocation == loc)
                equippedModules.Remove(equip);
        }

        equippedModules.Add(module.Id);

        ship.EquippedModules = equippedModules.ToArray();

        SaveLoadout(GetActiveLoadout());

        TargetRpcEquipModule(module.Id);
    }

    [TargetRpc]
    private void TargetRpcEquipModule(string id)
    {
        _onModuleEquipped?.Invoke(id);
    }

    #region ModuleUpgrade
    public void BuyUpgrade(string itemId, string upgradeId, Action<string> callback)
    {
        if (isLocalPlayer)
        {
            CmdBuyUpgrade(itemId, upgradeId);
            _onUpgradePurchased = callback;
        }
    }


    public void ResetUpgrades(string itemId, Action<string> callback)
    {
        if (isLocalPlayer)
        {
            CmdResetUpgrades(itemId);
            _onResetUpgrades = callback;
        }
    }

    [Command]
    private void CmdResetUpgrades(string itemId) 
    {
        var item = GetItem(itemId);
        if (item == null)
            return;

        if (item.SpentResources == null)
            return;

        var gameItem = ItemService.GetItem(item.ModuleId);
        if (gameItem == null || gameItem.Upgrades == null)
            return;

        foreach (var expense in item.SpentResources)
            AddResource(expense);

        item.SpentResources = null;
        item.Upgrades = null;

        SaveItem(item);

        SaveAccount();
    }

    [TargetRpc]
    private void TargetRpcUpgradesReset(string itemId) 
    {
        _onResetUpgrades?.Invoke(itemId);
    }

    [Command]
    private void CmdBuyUpgrade(string itemId, string upgradeId)
    {
        var item = GetItem(itemId);
        if (item == null)
            return;

        var gameItem = ItemService.GetItem(item.ModuleId);
        if (gameItem == null || gameItem.Upgrades == null)
            return;

        var upgrades = item.Upgrades.ToList();
        if (upgrades == null)
            upgrades = new List<Upgrade>();

        var totalUpgrades = item.Upgrades.Sum(s => s.Level);

        Debug.LogError($"Upgrade ID (Player): {upgradeId}");


        // NOT ALLOWED - over max upgrades
        if (totalUpgrades >= gameItem.MaxUpgrades)
            return;

        var upgradeInfo = gameItem.Upgrades.FirstOrDefault(u => u.Id == upgradeId);

        // INVALID UPGRADE
        if (upgradeInfo == null)
            return;

        var upgrade = item.Upgrades.FirstOrDefault(e => e.Id == upgradeId);

        if (upgrade == null)
        {
            upgrade = new Upgrade();
            upgrade.Id = upgradeId;
            upgrades.Add(upgrade);
        }

        // ALREADY MAXED
        if (upgrade.Level >= 10)
            return;

        var costMultiplier = (int)((upgrade.Level + totalUpgrades + 1) * 1.15f);

        // CHECK IF CAN AFFORD
        bool affordable = CheckResourceAffordable(upgradeInfo.ResourceCost, costMultiplier);
        if (!affordable)
            return;

        // SPEND CREDITS
        item = SpendResources(item, upgradeInfo.ResourceCost, costMultiplier);

        //Update upgrade array
        item.Upgrades = upgrades.ToArray();

        upgrade.Level++;

        SaveItem(item);
        SaveAccount();

    }

    #endregion

    public void BankTransaction(BankAction action, int credits)
    {
        CmdBankAction((uint)action, credits);
    }

    [Command]
    private void CmdBankAction(uint actionId, int credits) 
    {
        var action = (BankAction)actionId;

        switch (action) 
        {
            case BankAction.Deposit:
                if (credits > Credits)
                {
                    BankCredits += Credits;
                    Credits = 0;
                }
                else if (credits > 0)
                {
                    BankCredits += credits;
                    Credits -= credits;
                }
                break;
            case BankAction.Withdraw:
                if (credits > BankCredits)
                {
                    Credits += BankCredits;
                    BankCredits = 0;
                }
                else if (credits > 0)
                {
                    Credits += credits;
                    BankCredits -= credits;
                }
                break;
        }
    }
}
