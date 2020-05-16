using Lightshift;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerShip : Entity
{
    private readonly SyncListEquip _equips = new SyncListEquip();
    private Player player;
    private LightLance _lightLance;
    private Engine _engine;
    private Wing _wing;
    private Hull _hull;
    private PlayerController _input;
    private Heart _heart;
    private Shield _shield;
    private Generator _generator;
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
        _equips.Callback += OnEquipsChanged;
        _weaponSystem = GetComponent<WeaponSystem>();

        Debug.LogError(_heart.healthRegen);
    }

    private void Start()
    {
        base.Start();

        if (isServer || hasAuthority)
            CameraFollow.Instance.SetTarget(gameObject.transform);
    }
    public override void OnStartServer()
    {
        base.OnStartServer();

        player = Server.GetPlayer(connectionToClient);

        player.InventoryManager.onEquipChanged += OnEquipChanged;

        if (player != null)
            SetDisplayName(displayName: player.Username);
        else SetDisplayName(displayName: $"Player {connectionToClient.connectionId}");

        _playerData = new PlayerData
        {
            userId = player.connectUserId,
            username = player.Username,
        };

        UpdateStarship(player.GetStarship().key);

        LoadEquips();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        LoadEquips();
    }
    private void LoadEquips() 
    {
        if (isServer)
        {
            _equips.Clear();
            var equips = player.InventoryManager.GetAllEquips();
            foreach (var equip in equips)
                _equips.Add(equip);
        }
        else 
        {
            for (int i = 0; i < _equips.Count; i++)
            {
                OnEquipsChanged(default, i, null, _equips[i]);
            }
        }
    }
    private void OnStarshipChanged(string oldValue, string newValue)
    {
        UpdateStarship(newValue);
    }
    private void UpdateStarship(string key)
    {
        if (isServer)
        {
            _starshipDataKey = key;
        }

        var starship = ItemManager.GetStarship(key);
        _hull.SetImage(starship.Sprite, starship.color);
        _wing.SetImage(null, Color.white);

        _starshipData = starship.data;

        UpdateShipStats(true);
    }

    public void OnEquipChanged(InventorySlot slot)
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
        Debug.LogError($"{oldItem?.itemKey} {newItem?.itemKey}");
        /* REMOVE ITEM */
        if (oldItem != null)
        {
            _starshipData -= oldItem.data;
            Debug.Log(oldItem.data.ToString());
            var item = ItemManager.GetItem(oldItem.itemKey);
            switch (item.type)
            {
                case ItemType.Engine:
                    //_engine.SetColor(Color.white);
                    Debug.Log($"Engine Removed: {item.name}");
                    break;
                case ItemType.Generator:
                    Debug.Log($"Generator Removed: {item.name}");
                    break;
                case ItemType.Wing:
                    _wing.SetImage(null, Color.white);
                    Debug.Log($"Wing Removed: {item.name}");
                    break;
                case ItemType.Weapon:
                    _weaponSystem.RemoveWeapon(oldItem.slot);
                    Debug.Log($"Weapon Removed: {item.name}");
                    break;
                case ItemType.Shield:
                    Debug.Log($"Shield Removed: {item.name}");
                    break;
                case ItemType.LightLance:
                    _lightLance.SetColor(default);
                    Debug.Log($"Lightlance Removed: {item.name}");
                    break; 
                case ItemType.MiningDrill:
                    Debug.Log($"Mining Drill Removed: {item.name}");
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

            Debug.Log(newItem.data.ToString());
            switch (item.type)
            {
                case ItemType.Engine:
                    _engine.SetColor(item.color);
                    Debug.Log($"Engine Added: {item.name}");
                    break;
                case ItemType.Generator:
                    Debug.Log($"Generator Added: {item.name}");
                    break;
                case ItemType.Wing:
                    _wing.SetImage(item.Sprite, item.color);
                    Debug.Log($"Wing Added: {item.name}");
                    break;
                case ItemType.Weapon:
                    _weaponSystem.AddWeapon(item as Weapon, newItem.slot);
                    Debug.Log($"Weapon Added: {item.name}");
                    break;
                case ItemType.Shield:
                    Debug.Log($"Shield Added: {item.name}");
                    break;
                case ItemType.LightLance:
                    _lightLance.SetColor(item.color);
                    Debug.Log($"LightLance Added: {item.name}");
                    break;
                case ItemType.MiningDrill:
                    Debug.Log($"Mining Drill Removed: {item.name}");
                    break;
            }
        }

        UpdateShipStats();
    }

    private void UpdateShipStats(bool refill = false)
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

        if (refill)
        {
            _heart.health = stats.maxHealth;
            _shield.shield = stats.maxShield;
            _generator.power = stats.maxPower;
        }
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
        SetDisplayName(displayName: newData.username);
    }

    public override void OnEnterSafezone(Entity entity)
    {
        base.OnEnterSafezone(entity);

        if (isServer)
            Server.GetPlayer(connectionToClient).lastSafePosition = entity.transform.position;
    }

    private void OnDestroy()
    {
        _equips.Callback -= OnEquipsChanged;
        
        base.OnDestroy();
    }
}

