using Lightshift;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Ship : Entity
{
    private readonly SyncListEquip _equips = new SyncListEquip();

    private LightLance _lightLance;
    private Engine _engine;
    private Wing _wing;
    private Hull _hull;
    private PlayerController _input;
    private Heart _heart;
    private Shield _shield;
    private Generator _generator;
    private InventoryManager _inventoryManager;
    private ModuleData _starshipData;
    private WeaponSystem _weaponSystem;

    [SyncVar(hook = nameof(InitPlayer))]
    private PlayerData _playerData;

    [SyncVar(hook = nameof(OnStarshipChanged))]
    private string _starshipDataKey;

    private void Awake()
    {
        base.Awake();
        _lightLance = GetComponent<LightLance>();
        _engine = GetComponent<Engine>();
        _wing = GetComponent<Wing>();
        _hull = GetComponent<Hull>();
        _input = GetComponent<PlayerController>();
        _shield = GetComponent<Shield>();
        _heart = GetComponent<Heart>();
        _generator = GetComponent<Generator>();
        _inventoryManager = GetComponent<InventoryManager>();
        _inventoryManager.equipChanged += OnEquipChanged;
        _equips.Callback += OnEquipsChanged;
        _weaponSystem = GetComponent<WeaponSystem>();
    }

    private void Start()
    {
        if (isServer || isLocalPlayer)
            CameraFollow.Instance.SetTarget(gameObject.transform);
    }
    public override void OnStartServer()
    {
        var player = Server.GetPlayer(connectionToClient);

        if (player != null)
            SetDisplayName(displayName: player.Username);
        else SetDisplayName(displayName: $"Player {connectionToClient.connectionId}");

        UpdateStarship(player.GetStarship().key);

        _playerData = new PlayerData
        {
            userId = player.connectUserId,
            username = player.Username,
        };
    }

    private void OnStarshipChanged(string oldValue, string newValue)
    {
        UpdateStarship(newValue);
    }
    private void UpdateStarship(string key)
    {
        var starship = ItemManager.GetStarship(key);
        _hull.SetImage(starship.Sprite, starship.color);
        _wing.SetImage(null, Color.white);

        _starshipData = starship.data;

        UpdateShipStats();

        if (isServer)
        {
            _equips.Clear();
            _starshipDataKey = key;

            var equips = _inventoryManager.GetAllEquips();
            foreach (var equip in equips)
                _equips.Add(equip);
        }
    }

    private void OnEquipChanged(InventorySlot slot)
    {
        for (int i = 0; i < _equips.Count; i++) 
        {
            if (_equips[i].slot == slot.slotId)
            {
                //if (_starshipData != null)
                //    //Remove stats for this equip
                //    _starshipData.data -= _equips[i].data;

                _equips.Remove(_equips[i]);
                break;
            }
        }

        if (slot.item != null)
            _equips.Add(Equip.GetEquipFromItemSlot(slot));
    }

    private void OnEquipsChanged(SyncList<Equip>.Operation op, int itemIndex, Equip oldItem, Equip newItem)
    {
        /* REMOVE ITEM */
        if (oldItem != null)
        {
            _starshipData -= oldItem.data;

            var item = ItemManager.GetItem(oldItem.itemKey);
            switch (item.type)
            {
                case ItemType.Engine:
                    _engine.SetColor(Color.white);
                    break;
                case ItemType.Generator:
                    break;
                case ItemType.Wing:
                    _wing.SetImage(null, Color.white);
                    break;
                case ItemType.Weapon:
                    _weaponSystem.RemoveWeapon(oldItem.slot);
                    break;
                case ItemType.Shield:
                    break;
                case ItemType.LightLance:
                    _lightLance.SetColor(default);
                    break;
                case ItemType.MiningDrill:
                    break;
            }
        }
        else if (newItem != null && newItem.itemKey == null || newItem != null && newItem.itemKey == "") 
        {
            _wing.SetImage(null, Color.white);
        }
        /* ADD ITEM */
        else if (newItem != null)
        {
            _starshipData += newItem.data;

            var item = ItemManager.GetItem(newItem.itemKey);
            switch (item.type)
            {
                case ItemType.Engine:
                    _engine.SetColor(item.color);
                    break;
                case ItemType.Generator:
                    break;
                case ItemType.Wing:
                    _wing.SetImage(item.Sprite, item.color);
                    break;
                case ItemType.Weapon:
                    _weaponSystem.AddWeapon(item as Weapon, newItem.slot);
                    break;
                case ItemType.Shield:
                    break;
                case ItemType.LightLance:
                    _lightLance.SetColor(item.color);
                    break;
                case ItemType.MiningDrill:
                    break;
            }
        }
        UpdateShipStats();
    }

    private void UpdateShipStats() 
    {
        var stats = _starshipData;

        _engine.maxSpeed = stats.maxSpeed;
        _engine.acceleration = stats.acceleration;
        _engine.brakeForce = stats.brakeForce;
        _engine.overDriveMultiplier = stats.overDriveBoostMultiplier;
        _engine.overDrivePowerCost = stats.overDrivePowerCost;

        _hull.weight = stats.weight;

        _wing.agility = stats.agility;

        _heart.SetMaxHealth(stats.maxHealth);
        _heart.healthRegen = stats.healthRegen;


        _shield.SetMaxShield(stats.maxShield);
        _shield.shieldRegen = stats.shieldRegen;
        _generator.maxPower = stats.maxPower;
        _generator.powerRegen = stats.powerRegen;

        _lightLance.SetRange(stats.lightLanceRange);
        _lightLance.pullForce = stats.lightLancePullForce;
        _lightLance.powerCost = stats.lightLancePowerCost;     
    }

    private void FixedUpdate()
    {
        base.FixedUpdate();

        //HandlePowerRegen();
        //HandleShieldRegen();
        //HandleWeapons();
        //HandleDamageQueue();
        //HandleTargetting();

        _engine.Move(_input.VerticalAxis, _input.OverDrive);
        if (targetEntity != null && _starshipData.lightLanceRange != 0)
            _lightLance.HandleLightLance(_input.LightLance, targetEntity.transform);
        _wing.Turn(_input.HorizontalAxis);

        HandleSafeZone();

        if (_input.Weapon)
            _weaponSystem.TryFireWeapon(_input.WeaponSlot);


    }

    private void InitPlayer(PlayerData oldData, PlayerData newData)
    {
        if (oldData.userId != null && oldData.userId != "")
            PlayerManager.Instance.RemovePlayer(oldData);
        if (newData.userId != null && newData.userId != "")
            PlayerManager.Instance.AddPlayer(newData);

        SetDisplayName(displayName: newData.username);
    }


    private void OnDestroy()
    {
        PlayerManager.Instance.RemovePlayer(_playerData);
    }
}
