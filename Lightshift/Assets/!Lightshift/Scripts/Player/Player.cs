using Assets._Lightshift.Scripts.Data;
using Assets._Lightshift.Scripts.SolarSystem;
using Assets._Lightshift.Scripts.Web;
using Mirror;
using Newtonsoft.Json;
using PlayerIOClient;
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
    [SyncVar(hook = nameof(OnCreditsChanged))] public int Credits;
    [SyncVar(hook = nameof(OnBankCreditsChanged))] public int BankCredits;
    private void OnCreditsChanged(int oldValue, int newValue)
    {
        onCreditsChanged?.Invoke(newValue);
    }
    private void OnBankCreditsChanged(int oldValue, int newValue)
    {
        onBankCreditsChanged?.Invoke(newValue);
    }

    [SyncVar] public string Username;
    [SyncVar] public string ActiveShip;
    [SyncVar] public string Id;
    public PlayerShip ship { get; set; }

    private Profile _profile;

    private Account _account;

    private List<ShipObject> _ships = new List<ShipObject>();

    public Action<int> onCreditsChanged;

    public Action<int> onBankCreditsChanged;
    public override void OnStartServer()
    {
        base.OnStartServer();

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

            // SPAWN PLAYER SHIP
            var obj = Instantiate(LightshiftNetworkManager.GetPrefab<PlayerShip>());
            ship = obj.GetComponent<PlayerShip>();
            ship.displayName = _profile.Username;

            NetworkServer.Spawn(obj, connectionToClient);

            LoadShips(delegate () 
            {
                // Add default ship to user
                if (_ships == null || _ships.Count == 0)
                {
                    var newShip = PlayerDefaults.GetDefaultShip();
                    AddShip(newShip);

                    _profile.LandedLocationId = PlayerDefaults.GetDefaultStation();
                }

                // Ensure ship is selected
                if (_profile.ActiveShip == null)
                {
                    _profile.ActiveShip = _ships.FirstOrDefault().Id;
                }

                Credits = _profile.Credits;
                Username = _profile.Username;
                ActiveShip = _profile.ActiveShip;
                Id = account.Id;

                if (_profile.IsLanded)
                {
                    var station = LandableManager.GetLandableById(_profile.LandedLocationId);
                    if (station != null)
                    {
                        ship.transform.position = station.transform.position;
                        Land(_profile.LandedLocationId);
                    }
                }
                //Tell client to land

            });


            Server.AddPlayer(this);

            Debug.Log($"{account.Profile.Username} initialized.");
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

        Debug.Log($"New ship built for {_account.Profile.Username}.");

        TargetLoadShip(JsonConvert.SerializeObject(ship));

        SaveShip(ship);
    }
    public void SaveShip(ShipObject ship, Action callback = null)
    {
        if (!isServer)
            return;

        ship.UserId = _account.Id;
        HttpService.Get("game/saveship", ship,
        delegate (bool result)
        {
            if (result)
                Debug.Log($"Ship saved.");
            else Debug.LogError("Ship was not saved.");

            callback?.Invoke();
        });
    }

    public void SaveAccount()
    {
        if (isServer)
        {

            _profile.ActiveShip = ActiveShip;
            _profile.Username = Username;
            _profile.Credits = Credits;
            _profile.BankCredits = BankCredits;

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
            var station = LandableManager.GetLandableById(_profile.LandedLocationId);
            if (station != null)
                ship.transform.position = new Vector2(station.transform.position.x, station.transform.position.y);

            ship.transform.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(0, 360));

            ship.InitShipObject(GetActiveShip());
            ship.SetCargo(GetActiveShip().Cargo);
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
            _ships = ships;
            Debug.Log($"Server loaded ships [{ships?.Count}]");

            TargetLoadShips(JsonConvert.SerializeObject(_ships));

            if (ActiveShip == null || ActiveShip == "")
                ActiveShip = _ships.FirstOrDefault().Id;

            callback?.Invoke();
        });
    }


    [TargetRpc]
    public void TargetLoadShips(string json) 
    {
        _ships = JsonConvert.DeserializeObject<List<ShipObject>>(json);
        Debug.Log($"Client loaded ships [{_ships?.Count}]");
    }

    [TargetRpc]
    public void TargetLoadShip(string json)
    {
        var ship = JsonConvert.DeserializeObject<ShipObject>(json);
        if (ship == null)
            return;

        var existing = _ships.FirstOrDefault(s => s.Id == ship.Id);
        if (existing != null)
            _ships.Remove(existing);

        _ships.Add(ship);
        Debug.Log($"Added ship to [{_account.CaseSensitiveUsername}]");
    }

    public ShipObject GetActiveShip() => _ships.FirstOrDefault(s => s.Id == ActiveShip);
    public List<ShipObject> GetAllShips() => _ships;

    private Action<Item> _onUpgradePurchased;
    public void BuyUpgrade(string itemId, Modifier type, Action<Item> callback)
    {
        if (isLocalPlayer)
        {
            CmdBuyUpgrade(itemId, (uint)type);
            _onUpgradePurchased = callback;
        }
    }

    private Action<Item> _onResetUpgrades;
    public void ResetUpgrades(string itemId, Action<Item> callback)
    {
        if (isLocalPlayer)
        {
            CmdResetUpgrades(itemId);
            _onResetUpgrades = callback;
        }
    }

    private Action<string> _onShipChanged;
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

        var ship = GetAllShips().FirstOrDefault(s => s.Id == id);
        if (ship == null)
            return;

        _profile.ActiveShip = ship.Id;
        TargetRpcChangeShip(ship.Id);
    }

    [TargetRpc]
    private void TargetRpcChangeShip(string id) 
    {
        _onShipChanged?.Invoke(id);
    }

    private Action<string> _onModuleEquipped;
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

        var module = ship.OwnedItems.FirstOrDefault(m => m.Id == id);
        if (module == null)
            return;

        if (ship.EquippedModules == null)
            ship.EquippedModules = new List<string>();

        // Remove any old equips
        for (int i = 0; i < ship.EquippedModules.Count; i++)
        {
            var equip = ship.EquippedModules[i];
            var item = ship.OwnedItems.FirstOrDefault(e => e.Id == equip);
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

    [Command]
    private void CmdResetUpgrades(string itemId) 
    {
        var item = GetActiveShip().OwnedItems.FirstOrDefault(e => e.Id == itemId);
        if (item == null)
            return;

        var gameItem = ItemService.GetItem(item.ModuleId);
        if (gameItem == null || gameItem.Upgrades == null)
            return;

        _profile.Credits += item.SpentCredits;

        item.SpentCredits = 0;
        item.Upgrades = null;

        SaveShip(GetActiveShip());

        TargetRpcUpgradesReset(JsonConvert.SerializeObject(item));
    }

    [TargetRpc]
    private void TargetRpcUpgradesReset(string json) 
    {
        var item = JsonConvert.DeserializeObject<Item>(json);
        if (item != null)
            _onResetUpgrades?.Invoke(item);
    }

    [Command]
    private void CmdBuyUpgrade(string itemId, uint type)
    {
        var item = GetActiveShip().OwnedItems.FirstOrDefault(e => e.Id == itemId);
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

        var upgradeInfo = gameItem.Upgrades.FirstOrDefault(u => u.Type == (Modifier)type);

        // INVALID UPGRADE
        if (upgradeInfo == null)
            return;

        var upgrade = item.Upgrades.FirstOrDefault(e => e.Type == upgradeInfo.Type);
        if (upgrade == null)
        {
            upgrade = new Upgrade();
            upgrade.Type = upgradeInfo.Type;

            item.Upgrades.Add(upgrade);
        }

        // ALREADY MAXED
        if (upgrade.Level >= 10)
            return;

        var cost = upgradeInfo.Cost * (int)((upgrade.Level + totalUpgrades + 1) * 1.15f);

        bool affordable = cost <= Credits;

        // INVALID FUNDS
        if (!affordable)
            return;

        Credits -= cost;
        item.SpentCredits += cost;

        upgrade.Level++;

        SaveShip(GetActiveShip());

        TargetRpcUpgraded(JsonConvert.SerializeObject(item));
    }

    [TargetRpc]
    private void TargetRpcUpgraded(string json)
    {
        var item = JsonConvert.DeserializeObject<Item>(json);
        if (item != null)
            _onUpgradePurchased?.Invoke(item);
    }

    private void OnDestroy()
    {
        Debug.LogError($"{isServer} {isLocalPlayer} {hasAuthority} {isClient}");
        SaveAccount();
    }

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

    public void EjectCargo(CargoType type, int amount) 
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
        var type = (CargoType)typeId;

    }


}
