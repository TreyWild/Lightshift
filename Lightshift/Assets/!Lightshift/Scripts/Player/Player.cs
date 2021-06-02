using Assets._Lightshift.Scripts.Data;
using Assets._Lightshift.Scripts.Network;
using Assets._Lightshift.Scripts.SolarSystem;
using Assets._Lightshift.Scripts.Utilities;
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
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.Video;

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
    [SyncVar(hook = nameof(OnXpChanged))] public int Xp;
    [SyncVar(hook = nameof(OnLevelChanged))] public int Level;
    private void OnCreditsChanged(int oldValue, int newValue)
    {
        onCreditsChanged?.Invoke(newValue);
    }
    private void OnXpChanged(int oldValue, int newValue)
    {
        if (isLocalPlayer)
            RefreshXpBar();
    }

    private void OnLevelChanged(int oldValue, int newValue)
    {
        if (isLocalPlayer)
            RefreshXpBar();
    }

    private void RefreshXpBar() 
    {
        GameUIManager.Instance.XpBar.SetLabel($"Level {Level}");
        GameUIManager.Instance.XpBar.SetProgress(Xp, Xp * 1.25f, true);
    }

    private void OnBankCreditsChanged(int oldValue, int newValue)
    {
        onBankCreditsChanged?.Invoke(newValue);
    }

    private void OnActiveLoadoutChanged(string oldValue, string newValue)
    {
        _onShipChanged?.Invoke(newValue);
    }

    public readonly SyncDictionary<ResourceType, int> BankResources = new SyncDictionary<ResourceType, int>();
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

        RefreshXpBar();

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
    private bool _bankResourcesLoaded;
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
        if (!_bankResourcesLoaded)
        {
            LoadBankResources(() => InitPlayerData());
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

    public Item GetItem(string id)
    {
        if (!Items.ContainsKey(id))
            return Item.Empty();
        else return Items[id];

    }


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
            Xp = _profile.XP;
            Level = _profile.Level;
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
            _profile.XP = Xp;
            _profile.Level = Level;
            _profile.Bank = GetBankResources();
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

    

    public void SetupAccount()
    {
        // IF PLAYER HAS NO SHIPS, ADD DEFAULT
        if (ShipLoadouts == null || ShipLoadouts.Count == 0)
        {
            // Load default items
            var items = ItemService.GetPlayerDefaults().Items;

            // Add new ship object
            var newShip = new LoadoutObject();
            newShip.Id = Guid.NewGuid().ToString();

            foreach (var item in items)
                AddItem(new Item
                {
                    ModuleId = item.Id,
                    UserId = _account.Id,
                });

            AddLoadout(ItemService.GetPlayerDefaults().Loadout);

            // Setup Landed location
            var station = FindObjectsOfType<Station>().FirstOrDefault(s => s.defaultStation);
            if (station != null)
                LandedLocationId = station.Id;

            Credits = ItemService.GetPlayerDefaults().Credits;

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
    #region Land/Takeoff

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

    #endregion
    #region Loadout
    public LoadoutObject GetShipLoadout(string id)
    {
        if (!ShipLoadouts.ContainsKey(id))
            return LoadoutObject.Empty();
        else return ShipLoadouts[id];

    }

    public void SaveLoadout(LoadoutObject ship, Action callback = null)
    {
        if (!isServer)
            return;

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

    public void AddLoadout(LoadoutObject ship)
    {
        ship.Id = Guid.NewGuid().ToString();
        ship.UserId = _account.Id;
        ActiveLoadout = ship.Id;
        ShipLoadouts.Add(ship.Id, ship);

        Debug.Log($"New ship built for {_account.Profile.Username}.");

        SaveLoadout(ship);
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

            if (ActiveLoadout == null || ActiveLoadout == "" || !ShipLoadouts.ContainsKey(ActiveLoadout))
            {
                ActiveLoadout = ShipLoadouts.FirstOrDefault().Key;
            }
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

    public void EquipModule(string id, ModuleLocation location, Action<string> callback = null)
    {
        if (isLocalPlayer)
        {
            //_onModuleEquipped = callback;
            CmdEquipModule(id, location);
        }
    }

    [Command]
    private void CmdEquipModule(string id, ModuleLocation loc)
    {
        var ship = GetActiveLoadout();
        if (ship == null)
            return;

        var item = GetItem(id);
        if (item == null)
            return;

        if (ship.EquippedModules == null)
            ship.EquippedModules = new EquipObject[0];

        // Ensure there's enough room in the array
        bool hasExistingItem = ship.EquippedModules.Any(s => s.location == loc);
        if (!hasExistingItem)
        {
            //var list = item.Upgrades.ToList();
            //list.Add(new Upgrade());
            //item.Upgrades = list.ToArray();
            ship.EquippedModules = ship.EquippedModules.Append(new EquipObject {location = loc });
        }

        var module = ItemService.GetItem(item.ModuleId);

        if (module == null || module.InstallLocations == null)
            return;

        bool allowedAction = module.InstallLocations.Contains(loc);

        if (!allowedAction)
            return;

        // Equip Module
        for (int i = 0; i < ship.EquippedModules.Count(); i++)
        {
            var equip = ship.EquippedModules[i];
            if (equip.location == loc)
            {
                ship.EquippedModules[i].itemId = item.ModuleId;
                continue;
            }

            // Remove equips from existing locations
            if (equip.itemId == item.ModuleId)
                ship.EquippedModules[i].itemId = null;
        }

        SaveLoadout(ship);
    }
    #endregion
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

        // Init array
        if (item.Upgrades == null)
            item.Upgrades = new Upgrade[0];

        // Ensure there's enough room in the array
        bool hasExistingItem = item.Upgrades.Any(s => s.Id == upgradeId);
        if (!hasExistingItem)
        {
            //var list = item.Upgrades.ToList();
            //list.Add(new Upgrade());
            //item.Upgrades = list.ToArray();
            item.Upgrades = item.Upgrades.Append(new Upgrade());
        }

        var totalUpgrades = item.Upgrades.Sum(s => s.Level);

        // NOT ALLOWED - over max upgrades
        if (totalUpgrades >= gameItem.MaxUpgrades)
            return;

        var upgradeInfo = gameItem.Upgrades.FirstOrDefault(u => u.Id == upgradeId);

        // INVALID UPGRADE
        if (upgradeInfo == null)
            return;

        // For statement because structs are annoying
        for (int i = 0; i < item.Upgrades.Count(); i++)
        {
            var upgrade = item.Upgrades[i];

            if (upgrade.Id == null)
                upgrade.Id = upgradeId;

            if (upgrade.Id != upgradeId)
                continue;

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
            upgrade.Level++;

            item.Upgrades[i] = upgrade;
        }

        SaveItem(item);
        SaveAccount();

    }

    #endregion
    #region Buy Items
    [Command]
    private void CmdBuyModuleWithResources(string moduleId)
    {
        var gameItem = ItemService.GetItem(moduleId);
        if (gameItem == null || gameItem.Upgrades == null)
            return;

        var cost = gameItem.resourceCost;
        bool affordable = CheckResourceAffordable(cost);
        if (!affordable)
            return;

        SpendResources(cost);

        var item = new Item
        {
            Id = Guid.NewGuid().ToString(),
            MaxUpgrades = gameItem.MaxUpgrades,
            ModuleId = gameItem.Id,
            UserId = Id
        };

        AddItem(item);
    }

    public void BuyModuleWithResources(string moduleId) 
    {
        CmdBuyModuleWithResources(moduleId);
    }
    [Command]
    private void CmdBuyModuleCredits(string moduleId)
    {
        var gameItem = ItemService.GetItem(moduleId);
        if (gameItem == null || gameItem.Upgrades == null)
            return;

        var cost = gameItem.creditsCost;
        bool affordable = Credits >= cost;
        if (!affordable)
            return;

        Credits -= cost;

        var item = new Item
        {
            Id = Guid.NewGuid().ToString(),
            MaxUpgrades = gameItem.MaxUpgrades,
            ModuleId = gameItem.Id,
            UserId = Id
        };

        AddItem(item);
    }
    public void BuyModuleWithCredits(string moduleId)
    {
        CmdBuyModuleCredits(moduleId);
    }

    #endregion
    #region Bank/Cargo
    public void LoadBankResources(Action callback = null)
    {
        _bankResourcesLoaded = true;

        if (_profile.Bank == null)
        {
            // Add resources to new account
            _profile.Bank = new List<ResourceObject>();
        }

        foreach (var resource in _profile.Bank)
            AddResource(resource);

        callback.Invoke();
    }

    public ResourceObject GetBankResource(ResourceType type)
    {
        if (!BankResources.ContainsKey(type))
            return new ResourceObject { Amount = 0, Type = type };
        else return new ResourceObject { Amount = Resources[type], Type = type };

    }
    public void SetBankResource(ResourceObject resource) => SetBankResource(resource.Type, resource.Amount);
    public void SetBankResource(ResourceType type, int value)
    {
        if (isServer)
        {
            if (!BankResources.ContainsKey(type))
                BankResources.Add(type, value);
            else
                BankResources[type] = value;
        }
    }

    public void AddBankResource(ResourceObject resource) => AddBankResource(resource.Type, resource.Amount);
    public void AddBankResource(ResourceType type, int value)
    {
        if (isServer)
        {
            if (!BankResources.ContainsKey(type))
                BankResources.Add(type, value);
            else
                BankResources[type] += value;
        }
    }

    public void TakeBankResource(ResourceType type, int value)
    {
        if (isServer)
        {
            if (!BankResources.ContainsKey(type))
                BankResources.Add(type, value);
            else
                BankResources[type] -= value;

            if (BankResources[type] < 0)
                BankResources[type] = 0;
        }
    }
    public List<ResourceObject> GetBankResources()
    {
        var items = new List<ResourceObject>();
        foreach (var item in BankResources)
        {
            items.Add(new ResourceObject { Amount = item.Value, Type = item.Key });
        }
        return items;
    }

    public void LoadResources(Action callback = null)
    {
        _resourcesLoaded = true;

        if (_profile.Resources == null)
        {
            // Add resources to new account
            _profile.Resources = ItemService.GetPlayerDefaults().Resources;
        }

        foreach (var resource in _profile.Resources)
            AddResource(resource);


        callback.Invoke();
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

        obj.transform.position = ship.transform.position + new Vector3(UnityEngine.Random.Range(-3, 3), UnityEngine.Random.Range(3, -3));
        NetworkServer.Spawn(obj);

        TakeResource(type, amount);
    }

    public int GetCargoCapacity() 
    {
        var stats = StatHelper.GetStatsFromShip(this, GetActiveLoadout());
        if (stats == null)
            return 0;
        var stat = stats.FirstOrDefault(s => s.Type == Modifier.Storage);
        if (stat != null)
            return (int)stat.Value;
        return 0; 
    }

    public void PickupResource(ResourceType type, int amount)
    {
        var cargoCount = Resources.Sum(s => s.Value);
        var capacity = GetCargoCapacity();
        var total = cargoCount + amount;

        // Eject remaining when over capacity
        if (total > capacity)
        {
            var maths = total - capacity;
            if (maths > 0)
                EjectResource(type, maths);
            amount -= maths;
        }
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

    public void SpendResources(List<ResourceObject> cost)
    {
        foreach (var resource in cost)
        {
            // Calculate Expense
            var amount = GetResource(resource.Type).Amount - resource.Amount;
            if (amount < 0)
                amount = 0;

            // Update resources for client 
            SetResource(resource.Type, amount);
        }
    }

    public Item SpendResources(Item item, List<ResourceObject> cost, float costMultiplier)
    {
        // Ensure item spent resources aren't null
        if (item.SpentResources == null)
            item.SpentResources = new ResourceObject[0];

        // Iterate costs
        foreach (var resource in cost)
        {
            //Ensure item spent resoureces contains cost type
            if (!item.SpentResources.Any(r => r.Type == resource.Type))
            {
                var spentResource = new ResourceObject { Type = resource.Type, Amount = 0 };
                item.SpentResources = item.SpentResources.Append(spentResource);
            }

            // Find item spent resource object and modify it
            for (int i = 0; i < item.SpentResources.Count(); i++)
            {
                // If not resource type, continue.
                if (resource.Type != item.SpentResources[i].Type)
                    continue;

                // Calculate Expense
                var expense = (int)(resource.Amount * costMultiplier);
                var amount = GetResource(resource.Type).Amount - expense;
                if (amount < 0)
                    amount = 0;

                // Modify resource
                item.SpentResources[i].Amount += expense;

                // Update resources for client 
                SetResource(resource.Type, amount);
            }
        }

        return item;
    }

    public void DepositAllCargo()
    {
        CmdDepositAll();
    }

    [Command]
    private void CmdDepositAll()
    {
        var cargo = GetResources();

        foreach (var item in cargo)
        {
            AddBankResource(item.Type, item.Amount);
            TakeResource(item.Type, item.Amount);
        }

        SaveAccount();
    }

    public void WithdrawAllCargo()
    {
        CmdWithdrawAll();
    }

    [Command]
    private void CmdWithdrawAll() 
    {
        var bank = GetBankResources();

        foreach (var item in bank)
        {
            AddResource(item.Type, item.Amount);
            TakeBankResource(item.Type, item.Amount);
        }

        SaveAccount();
    }

    public void BankTransaction(BankAction action, ResourceType type, int balance)
    {
        CmdBankAction(action, type, balance);
    }

    [Command]
    private void CmdBankAction(BankAction action, ResourceType type, int balance) 
    {
        switch (action) 
        {
            case BankAction.Deposit:

                var cargo = GetResource(type);

                // If there is less or equal cargo than the amount specified in CMD
                if (balance > cargo.Amount)
                {
                    // Add to bank
                    AddBankResource(type, balance);

                    // Remove from cargo
                    SetResource(type, 0);
                }// If there is more or equal cargo than the amount specified in CMD
                else if (balance > 0)
                {
                    // Add to bank
                    AddBankResource(type, balance);

                    // Remove from cargo
                    SetResource(type, cargo.Amount - balance);
                }
                break;

            case BankAction.Withdraw:

                var bank = GetBankResource(type);

                // If there is less or equal in the bank than specified by CMD
                if (balance > bank.Amount)
                {
                    // Add to cargo
                    AddResource(type, balance);

                    // Remove from Bank
                    SetBankResource(type, 0);
                }
                // If there is more or equal in the bank than specified by the CMD
                else if (balance > 0)
                {
                    // Add to cargo
                    AddResource(type, balance);

                    // Remove from bank
                    SetBankResource(type, bank.Amount - balance);
                }
                break;
        }

        SaveAccount();
    }
    #endregion
}
