using Assets._Lightshift.Scripts.Utilities;
using Lightshift;
using Mirror;
using Newtonsoft.Json;
using SharedModels.Models.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerShip : Ship
{
    private ShipObject _shipObject;
    private PlayerController _input;
    private List<Item> _equippedModules;
    private void Awake()
    {
        base.Awake();

        _input = GetComponent<PlayerController>();
        //_equips.Callback += OnEquipsChanged;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        //player.InventoryManager.onEquipChanged += OnEquipChanged;

        //if (player != null)
        //    SetDisplayName(displayName: player.Username);
        //else SetDisplayName(displayName: $"Player {connectionToClient.connectionId}");

        //_playerData = new PlayerData
        //{
        //    userId = player.connectUserId,
        //    username = player.Username,
        //};

        //UpdateStarship(player.GetStarship().key);

        //LoadEquips();

        //trackingRange = 100;

        //teamId = player.connectUserId;
    }

    private void Start()
    {
        base.Start();
        design.GenerateColliders();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        CmdInit();
    }

    [Command]
    private void CmdInit()
    {
        TargetRpcInitModules(connectionToClient, JsonConvert.SerializeObject(_equippedModules));
    }
    public void InitShipObject(ShipObject shipObject) 
    {
        _shipObject = shipObject;
        var stats = StatHelper.GetStatsFromShip(shipObject);

        SetModifiers(stats);

        _equippedModules = shipObject.OwnedItems.Where(s => shipObject.EquippedModules.Contains(s.Id)).ToList();
        if (_equippedModules != null && _equippedModules.Count > 0)
        {
            InitModules(_equippedModules);
            RpcInitModules(JsonConvert.SerializeObject(_equippedModules));
        }
    }

    [TargetRpc]
    private void TargetRpcInitModules(NetworkConnection target, string json) 
    {
        var modules = JsonConvert.DeserializeObject<List<Item>>(json);
        InitModules(modules);
    }

    [ClientRpc]
    private void RpcInitModules(string json)
    {
        var modules = JsonConvert.DeserializeObject<List<Item>>(json);
        InitModules(modules);
    }

    private void InitModules(List<Item> equippedModules) 
    {
        if (equippedModules == null)
            return;

        foreach (var module in equippedModules)
        {
            if (module == null)
                continue;

            var item = ItemService.GetItem(module.ModuleId);
            if (item == null)
                continue;

            switch (item.Type)
            {
                case ItemType.Wing:
                    SetWings(item.Sprite);
                    break;
                case ItemType.Hull:
                    SetHull(item.Sprite);
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!alive)
            return;

        base.FixedUpdate();

        //HandlePowerRegen();
        //HandleShieldRegen();
        //HandleWeapons();
        //HandleDamageQueue();
        //HandleTargetting();


        //if (targetEntity != null && stats.lightLanceRange != 0) 
        //    lightLance.HandleLightLance(_input.LightLance, targetEntity.kinematic);

        bool overDrive = _input.OverDrive;
        if (_input.Up)
        {
            if (hasAuthority)
                Thrust(overDrive);

            thruster.StartThruster(overDrive);
        }
        else 
        {
            thruster.StopThruster();

            if (hasAuthority)
            {
                if (_input.Down)
                    Brake();

                if (_input.Drifting)
                    kinematic.drag = 0.999f;
                else kinematic.drag = 0.99f;
            }
        }

        Turn(_input.HorizontalAxis, _input.Up);

        HandleSafeZone();

        if (_input.Weapon)
            weaponSystem.TryFireWeapon(_input.WeaponSlot);
    }
    public override void OnEnterSafezone(Entity entity)
    {
        base.OnEnterSafezone(entity);

        //if (isServer)
        //    Server.GetPlayer(connectionToClient).lastSafePosition = entity.transform.position;
    }

    private void OnDestroy()
    {
        //_equips.Callback -= OnEquipsChanged;
        
        base.OnDestroy();
    }

    public override void OnRespawn()
    {
        base.OnRespawn();

        _input.Locked = false;

        if (hasAuthority || isServer)
            CameraFollow.Instance.SetTarget(transform);
    }

    private string _currentStationDock;
    public void EnterStationDock(Landable landable) 
    {
        _currentStationDock = landable.Id;

        if (hasAuthority)
        {
            GameUIManager.Instance.ShowDockingPrompt(landable);
        }
    }

    public void LeaveStationDock()
    {
        _currentStationDock = "";
        if (hasAuthority)
        {
            GameUIManager.Instance.HideDockPromot();
        }
    }

    public override void OnDeath()
    {
        base.OnDeath();

        _input.Locked = true;
    }
}

