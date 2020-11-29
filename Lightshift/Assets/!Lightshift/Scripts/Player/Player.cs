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
    public string Id { get; set; }
    public PlayerShip ship { get; set; }

    [SyncVar] private Profile _profile;
    private Account _account;

    private List<ShipObject> _ships = new List<ShipObject>();

    private Action shipsLoaded;
    public override void OnStartServer()
    {
        base.OnStartServer();

        HttpService.Get("account/get", connectionToClient.authenticationData,
        delegate (Account account)
        {
            Debug.Log($"{account.Profile.Username} connected.");
            _account = account;
            _profile = _account.Profile;

            Id = account.Id;

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
                    SaveAccount();
                }

                //Tell client to land
                Land(_profile.LandedLocationId);
            });


            Server.AddPlayer(this);

            Debug.Log($"{account.Profile.Username} initialized.");
        });
    }

    public Profile GetProfile() => _profile;
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

        SaveAccount();
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
        if (!isServer)
            return;

        _account.Profile = _profile;
        HttpService.Get("account/save", _account,
        delegate (Account account)
        {
            _account = account;
            _profile = account.Profile;
            Debug.Log($"Account for {_account.CaseSensitiveUsername} saved.");
        });
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
        Debug.LogError(isServer);
        Land(landableId);
    }

    public void Land(string landableId) 
    {
        Debug.Log($"Requesting Land {landableId}");
        if (isServer)
        {
            RpcLand(landableId);

            // TO DO : Destroy ship?
        }
        else CmdLand(landableId);
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
    public ShipObject GetActiveShip() => _ships.FirstOrDefault(s => s.Id == _profile.ActiveShip);
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
        SaveAccount();
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

        TargetRpcEquipModule(ship.Id);
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

        SaveAccount();

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

        bool affordable = cost <= GetProfile().Credits;

        // INVALID FUNDS
        if (!affordable)
            return;

        _profile.Credits -= cost;
        item.SpentCredits += cost;

        SaveAccount();
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
}
