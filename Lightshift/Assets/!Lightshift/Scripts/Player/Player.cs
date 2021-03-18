using Assets._Lightshift.Scripts.Data;
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
    [SyncVar(hook = nameof(OnActiveShipChanged))]
    public string ActiveShip;
    [SyncVar] public string Id;
    private void OnCreditsChanged(int oldValue, int newValue)
    {
        onCreditsChanged?.Invoke(newValue);
    }
    private void OnBankCreditsChanged(int oldValue, int newValue)
    {
        onBankCreditsChanged?.Invoke(newValue);
    }

    private void OnActiveShipChanged(string oldValue, string newValue)
    {
        _onShipChanged?.Invoke(newValue);
    }

    public readonly SyncDictionary<ResourceType, int> Resources = new SyncDictionary<ResourceType, int>();
    public readonly SyncDictionary<string, Item> Items = new SyncDictionary<string, Item>();
    private readonly SyncDictionary<string, ShipObject> ShipLoadouts = new SyncDictionary<string, ShipObject>();


    public PlayerShip ship { get; set; }

    private Profile _profile;

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
        _profile = null;
        _account = null;
        onCreditsChanged = null;
        onBankCreditsChanged = null;
        _onUpgradePurchased = null;
        _onResetUpgrades = null;
        _onShipChanged = null;
        _onModuleEquipped = null;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        StartCoroutine(Init());
    }
    public override void OnStartServer()
    {
        base.OnStartServer();

        // ADD PLAYER TO SERVER INDEX
        Server.AddPlayer(this);


        // LOAD PROFILE
        LoadProfile(delegate 
        {
            // LOAD RESOURCES
            LoadResources();

            // LOAD ITEMS
            LoadItems(delegate 
            {
                // LOAD SHIPS
                LoadShips(delegate ()
                {
                    SetupAccount();

                    // Ensure ship is selected
                    if (ActiveShip == null || GetActiveShip() == null)
                    {
                        ActiveShip = ShipLoadouts.First().Key;
                    }

                    Debug.Log($"{Username} initialized.");
                    foreach (var player in Server.GetAllPlayers())
                    {
                        if (player == this)
                            continue;

                        Communication.ShowUserAlert(player.connectionToClient, $"{Username} connected.", Communication.AlertType.SystemMessage);
                        Communication.ShowUserAlert(player.connectionToClient, $"{Username} entered the system", Communication.AlertType.ScreenDisplay);
                    }
                });
            });
        });
    }

    private IEnumerator Init()
    {
        var proceed = GetActiveShip() != null;
        while (!proceed)
        {
            yield return new WaitForSeconds(1f);

            proceed = GetActiveShip() != null;
        }

        CmdInit();
    }

    [Command]
    private void CmdInit() 
    {
        SpawnShip();
        TargetInit();
    }

    [TargetRpc]
    private void TargetInit()
    {
        GameUIManager.Instance.ToggleLoadingScreen(false);
    }

    public List<ShipObject> GetShipLoadouts()
    {
        var items = new List<ShipObject>();
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
            items.Add(new ResourceObject { Amount = item.Value, Type = item.Key});

        return items;
    }
    public Item GetItem(string id)
    {
        if (!Items.ContainsKey(id))
            return null;
        else return Items[id];

    }

    public ShipObject GetShipLoadout(string id)
    {
        if (!ShipLoadouts.ContainsKey(id))
            return null;
        else return ShipLoadouts[id];

    }

    #region Resources
    public void LoadResources(Action callback = null) 
    {
        if (_profile.Resources == null || _profile.Resources.Count == 0)
            _profile.Resources = PlayerDefaults.GetTestResources();

        foreach (var resource in _profile.Resources)
            AddResource(resource);
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

    public bool SpendResources(List<ResourceObject> resources, float costMultiplier)
    {
        foreach (var resource in resources)
        {
            var amount = GetResource(resource.Type).Amount - (int)(resource.Amount * costMultiplier);
            if (amount < 0)
                amount = 0;

            SetResource(resource.Type, amount);
        }

        return true;
    }

    #endregion

    public void SpawnShip()
    {
        // SPAWN PLAYER SHIP
        var obj = Instantiate(LightshiftNetworkManager.GetPrefab<PlayerShip>());
        ship = obj.GetComponent<PlayerShip>();
        ship.displayName = _profile.Username;

        NetworkServer.Spawn(obj, connectionToClient);

        if (_profile.IsLanded)
        {
            var station = LandableManager.GetLandableById(_profile.LandedLocationId);
            if (station != null)
            {
                ship.transform.position = station.transform.position;
                Land(_profile.LandedLocationId);
            }
        }
        else TakeOff();
    }
    public void LoadProfile(Action callback)
    {
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
            ActiveShip = _profile.ActiveShip;
            lastCheckpointId = _profile.LastCheckPointId;
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

    public void AddShip(ShipObject ship) 
    {
        _profile.ActiveShip = ship.Id;
        _profile.IsLanded = true;
        ShipLoadouts.Add(ship.Id, ship);

        Debug.Log($"New ship built for {_account.Profile.Username}.");

        SaveShip(ship);
    }
    public void SaveShip(ShipObject ship, Action callback = null)
    {
        if (!isServer)
            return;

        ship.UserId = _account.Id;

        ShipLoadouts[ship.Id] = ship;

        HttpService.Get("game/saveship", ship,
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
            _profile.ActiveShip = ActiveShip;
            _profile.Username = Username;
            _profile.Credits = Credits;
            _profile.BankCredits = BankCredits;
            _profile.Resources = GetResources();
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

            _profile.LandedLocationId = landableId;

            _profile.IsLanded = true;

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

            _profile.IsLanded = false;
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
            var station = LandableManager.GetLandableById(_profile.LandedLocationId);
            if (station != null)
                ship.transform.position = new Vector2(station.transform.position.x, station.transform.position.y);

            ship.kinematic.rotation = UnityEngine.Random.Range(0, 360);

            ship.InitShipObject(GetActiveShip());
            //ship.SetCargo(GetActiveShip().Cargo);
            ship.Respawn();

            _profile.IsLanded = false;

            RpcTakeoff();
        }
        else CmdTakeoff();
    }

    public void LoadShips(Action callback)
    {
        if (!isServer)
            return;

        _account.Profile = _profile;
        HttpService.Get("game/getships", new JsonString { Value = _account.Id },
        delegate (List<ShipObject> ships)
        {
            foreach (var ship in ships)
            {
                if (!ShipLoadouts.ContainsKey(ship.Id))
                    ShipLoadouts.Add(ship.Id, ship);
                else ShipLoadouts[ship.Id] = ship;
            }

            Debug.Log($"Server loaded ships [{ships?.Count}]");

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
            var newShip = new ShipObject();
            newShip.Id = Guid.NewGuid().ToString();
            newShip.EquippedModules = new List<string>();

            foreach (var item in items)
                newShip.EquippedModules.Add(item.Id);

            AddShip(newShip);

            // Setup Landed location
            _profile.LandedLocationId = PlayerDefaults.GetDefaultStation();

            // Save account
            SaveAccount();
        }
    }

    public void LoadItems(Action callback)
    {
        if (!isServer)
            return;

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

    public ShipObject GetActiveShip() 
    {
        return GetShipLoadout(ActiveShip);
    }

    public void ChangeShip(string ship, Action<string> callback = null)
    {
        if (isLocalPlayer)
        {
            _onShipChanged = callback;
            CmdChangeShip(ship);
        }
    }

    [Command]
    private void CmdChangeShip(string id) 
    {
        //Ensure loadout exists
        if (!ShipLoadouts.ContainsKey(id))
            return;

        ActiveShip = id;
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
        var ship = GetActiveShip();
        if (ship == null)
            return;

        var module = GetItem(id);
        if (module == null)
            return;

        if (ship.EquippedModules == null)
            ship.EquippedModules = new List<string>();

        // Remove any old equips
        for (int i = 0; i < ship.EquippedModules.Count; i++)
        {
            var equip = ship.EquippedModules[i];
            var item = GetItem(equip);
            if (item.ModuleLocation == loc)
                ship.EquippedModules.Remove(equip);
        }

        ship.EquippedModules.Add(module.Id);

        SaveShip(GetActiveShip());

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

        if (item.Upgrades == null)
            item.Upgrades = new List<Upgrade>();

        var totalUpgrades = item.Upgrades.Sum(s => s.Level);

        // NOT ALLOWED - over max upgrades
        if (totalUpgrades >= gameItem.MaxUpgrades)
            return;

        var upgradeInfo = gameItem.Upgrades.FirstOrDefault(u => u.Id == upgradeId);

        // INVALID UPGRADE
        if (upgradeInfo == null)
            return;

        var upgrade = item.Upgrades.FirstOrDefault(e => Id == upgradeId);

        if (upgrade == null)
        {
            upgrade = new Upgrade();

            item.Upgrades.Add(upgrade);
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
        SpendResources(upgradeInfo.ResourceCost, costMultiplier);

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

    public void EjectCargo(ResourceType type, int amount) 
    {
        if (!isServer)
        {
            CmdEjectCargo((uint)type, amount);
        }
    }

    public void EjectAllCargo()
    {
        if (!isServer)
        {
            CmdEjectAllCargo();
        }
    }

    [Command]
    private void CmdEjectAllCargo()
    {

    }

    [Command]
    private void CmdEjectCargo(uint typeId, int amount) 
    {
        var type = (ResourceType)typeId;

    }


}
