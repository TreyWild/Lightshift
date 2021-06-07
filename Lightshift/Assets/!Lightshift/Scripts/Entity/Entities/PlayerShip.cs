using Assets._Lightshift.Scripts.Services;
using Assets._Lightshift.Scripts.SolarSystem;
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
    private Item[] _equippedModules;
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
            TargetInitModules(sender, _loadoutObject, _equippedModules);
    }
    public void InitLoadoutObject(LoadoutObject loadoutObject) 
    {
        _loadoutObject = loadoutObject;

        displayName = Player.Username;

        var stats = StatHelper.GetStatsFromShip(Player, loadoutObject);

        SetModifiers(stats);

        _equippedModules = Player.GetItems().Where(s => loadoutObject.EquippedModules.Any(e => e.itemId == s.ModuleId)).ToArray();
        if (_equippedModules != null && _equippedModules.Count() > 0)
        {
            InitLoadout(loadoutObject, _equippedModules);
            RpcInitLoadout(loadoutObject, _equippedModules);
            _inited = true;
        }
    }

    [ClientRpc]
    private void RpcInitLoadout(LoadoutObject loadout, Item[] itemData) 
    {
        InitLoadout(loadout, itemData);
    }

    [TargetRpc]
    private void TargetInitModules(NetworkConnection target, LoadoutObject loadout, Item[] itemData)
    {
        InitLoadout(loadout, itemData);
    }

    private void InitLoadout(LoadoutObject loadout, Item[] itemData) 
    {
        if (loadout.EquippedModules != null)
            foreach (var equip in loadout.EquippedModules)
            {
                var module = ItemService.GetItem(equip.itemId);
                if (module == null)
                    continue;

                switch (module.Type)
                {
                    case ItemType.Wing:
                        SetWings(module.Sprite);
                        break;
                    case ItemType.Hull:
                        SetHull(module.Sprite);
                        break;
                    case ItemType.Weapon:

                        var weapon = module as Weapon;
                        if (weapon == null)
                            continue;

                        var item = itemData.FirstOrDefault(i => i.ModuleId == module.Id);
                        if (item.Id == null)
                            continue;

                        var stats = StatHelper.GetStatsFromItem(item);

                        foreach (var stat in stats)
                        {
                            switch (stat.Type)
                            {
                                case Modifier.Damage:
                                    weapon.weaponData.bulletData.damage += stat.Value;
                                    break;
                                case Modifier.Refire:
                                    weapon.weaponData.refire += stat.Value;
                                    break;
                                case Modifier.Range:
                                    weapon.weaponData.bulletData.range += stat.Value;
                                    break;
                                case Modifier.Speed:
                                    weapon.weaponData.bulletData.speed += stat.Value;
                                    break;
                                //case Modifier.PowerRegen:
                                //    weapon.weaponData.powerCost += stat.Value;
                                //    break;
                            }
                        }

                        switch (equip.location)
                        {
                            case ModuleLocation.Weapon1:
                                weaponSystem.AddWeapon(weapon, 0);
                                break;
                            case ModuleLocation.Weapon2:
                                weaponSystem.AddWeapon(weapon, 1);
                                break;
                            case ModuleLocation.Weapon3:
                                weaponSystem.AddWeapon(weapon, 2);
                                break;
                            case ModuleLocation.Weapon4:
                                weaponSystem.AddWeapon(weapon, 3);
                                break;
                            case ModuleLocation.Weapon5:
                                weaponSystem.AddWeapon(weapon, 4);
                                break;
                        }
                        break;

                    case ItemType.LightLance:
                        
                        break;
                    case ItemType.MiningDrill:
                        break;
                }
            }
    }

    private new void FixedUpdate()
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
                    kinematic.drag = 0f;
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

    [TargetRpc]
    public void TargetStartLandingAnimation(string landableId) 
    {
        var landable = LandableManager.GetLandableById(landableId);
        if (landable == null)
            return;

        StartCoroutine(landShip(landable.transform.position, landableId));
    }



    private IEnumerator landShip(Vector2 landingLocation, string landableId) 
    {
        kinematic.velocity = Vector2.zero;
        while (transform.localScale.x > .3f)
        {
            transform.Rotate(new Vector3(0,0, 90 * Time.deltaTime));
            var vector = Vector2.MoveTowards(kinematic.position, landingLocation, 5 * Time.deltaTime);
            kinematic.position = vector;
            transform.localScale = new Vector2(transform.localScale.x - .8f * Time.deltaTime, transform.localScale.y - .8f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        CmdFinishLandingAnimation(landableId);
    }

    [Command]
    private void CmdFinishLandingAnimation(string landableId) 
    {
        _player.Land(landableId, true);
    }
}

