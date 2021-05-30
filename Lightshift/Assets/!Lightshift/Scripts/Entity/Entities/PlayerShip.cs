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
    private LoadoutObject _loadoutObject;
    private PlayerController _input;
    private List<Item> _equippedModules;
    private Player _player;
    private bool _inited;
    public Player Player 
    {
        get
        {
            if (_player == null)
                _player = Server.GetPlayer(connectionToClient);

            return _player;
        }
    }
    private new void Awake()
    {
        base.Awake();

        _input = GetComponent<PlayerController>();
        //_equips.Callback += OnEquipsChanged;

        onCleanup += () =>
        {
            _input = null;
            _equippedModules = null;
        };
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

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    private void Start()
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    {
        base.Start();
        design.GenerateSolidColliders();
        design.GenerateTriggerColliders();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        
        CmdInit();
    }

    [Command(requiresAuthority = false)]
    private void CmdInit(NetworkConnectionToClient sender = null)
    {
        if (_inited) 
            TargetInitModules(sender, JsonConvert.SerializeObject(_equippedModules));
    }
    public void InitLoadoutObject(LoadoutObject loadoutObject) 
    {
        displayName = Player.Username;

        _loadoutObject = loadoutObject;
        var stats = StatHelper.GetStatsFromShip(Player, loadoutObject);

        SetModifiers(stats);

        _equippedModules = Player.GetItems().Where(s => loadoutObject.EquippedModules.Any(e => e.itemId == s.Id)).ToList();
        if (_equippedModules != null && _equippedModules.Count > 0)
        {
            InitModules(_equippedModules);
            RpcInitModules(JsonConvert.SerializeObject(_equippedModules));
            _inited = true;
        }
    }

    [ClientRpc]
    private void RpcInitModules(string json) 
    {
        var modules = JsonConvert.DeserializeObject<List<Item>>(json);
        InitModules(modules);
    }

    [TargetRpc]
    private void TargetInitModules(NetworkConnection target, string json)
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

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    private void FixedUpdate()
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    {
        if (!alive)
            return;

        base.FixedUpdate();

        //HandlePowerRegen();
        //HandleShieldRegen();
        //HandleWeapons();
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

        if (_input.Weapon)
            weaponSystem.TryFireWeapon(_input.WeaponSlot);
    }

    public override void OnEnterCheckpoint(Checkpoint checkpoint)
    {
        if (isServer && Player != null) 
            Player.lastCheckpointId = checkpoint.Id;

        if (hasAuthority)
            GameUIManager.Instance.ShowScreenText("Entered Safezone, Weapons Disabled.");
    }

    public override void OnLeaveCheckpoint(Checkpoint checkpoint)
    {
        base.OnLeaveCheckpoint(checkpoint);

        if (hasAuthority)
            GameUIManager.Instance.ShowScreenText("Leaving Safezone, Weapons Enabled.");
    }
    public override void OnRespawn()
    {
        base.OnRespawn();

        _input.Locked = false;

        if (hasAuthority)
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

