using Lightshift;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerShip : Ship
{
    //private readonly SyncListEquip _equips = new SyncListEquip();
    private Player player;

    private PlayerController _input;

    [SyncVar(hook = nameof(InitPlayer))]
    private PlayerData _playerData;

    [SyncVar(hook = nameof(OnStarshipChanged))]
    private string _starshipDataKey;

    private void Awake()
    {
        base.Awake();

        _input = GetComponent<PlayerController>();
        //_equips.Callback += OnEquipsChanged;
    }
    public override void OnStartServer()
    {
        base.OnStartServer();

        player = Server.GetPlayer(connectionToClient);

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

    public override void OnStartClient()
    {
        base.OnStartClient();

        //LoadEquips();
    }
    //private void LoadEquips() 
    //{
    //    if (isServer)
    //    {
    //        _equips.Clear();
    //        var equips = player.InventoryManager.GetAllEquips();
    //        foreach (var equip in equips)
    //            _equips.Add(equip);
    //    }
    //    else 
    //    {
    //        for (int i = 0; i < _equips.Count; i++)
    //        {
    //            OnEquipsChanged(default, i, null, _equips[i]);
    //        }
    //    }
    //}
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

        //var starship = ItemManager.GetStarship(key);
        //if (starship == null)
        //    return;

        //hull.SetImage(starship.Sprite, starship.color);
        //wing.SetImage(null, Color.white);

        //stats = starship.data;

        //UpdateStats(true);
    }

    //public void OnEquipChanged(InventorySlot slot)
    //{
    //    for (int i = 0; i < _equips.Count; i++)
    //    {
    //        if (_equips[i].slot == slot.slotId)
    //        {
    //            //if (_starshipData != null)
    //            //    //Remove stats for this equip
    //            //    _starshipData.data -= _equips[i].data;

    //            _equips.Remove(_equips[i]);
    //            break;
    //        }
    //    }

    //    if (slot.item != null)
    //        _equips.Add(Equip.GetEquipFromItemSlot(slot));
    //}

    //private void OnEquipsChanged(SyncList<Equip>.Operation op, int itemIndex, Equip oldItem, Equip newItem)
    //{
    //    Debug.LogError($"{oldItem?.itemKey} {newItem?.itemKey}");
    //    /* REMOVE ITEM */
    //    if (oldItem != null)
    //    {
    //        stats -= oldItem.data;
    //        Debug.Log(oldItem.data.ToString());
    //        var item = ItemManager.GetItem(oldItem.itemKey);
    //        switch (item.type)
    //        {
    //            case ItemType.Engine:
    //                //_engine.SetColor(Color.white);
    //                Debug.Log($"Engine Removed: {item.name}");
    //                break;
    //            case ItemType.Generator:
    //                Debug.Log($"Generator Removed: {item.name}");
    //                break;
    //            case ItemType.Wing:
    //                wing.SetImage(null, Color.white);
    //                Debug.Log($"Wing Removed: {item.name}");
    //                break;
    //            case ItemType.Weapon:
    //                weaponSystem.RemoveWeapon(oldItem.slot);
    //                Debug.Log($"Weapon Removed: {item.name}");
    //                break;
    //            case ItemType.Shield:
    //                Debug.Log($"Shield Removed: {item.name}");
    //                break;
    //            case ItemType.LightLance:
    //                lightLance.SetColor(default);
    //                Debug.Log($"Lightlance Removed: {item.name}");
    //                break; 
    //            case ItemType.MiningDrill:
    //                Debug.Log($"Mining Drill Removed: {item.name}");
    //                break;
    //        }
    //    }
    //    else if (newItem != null && newItem.itemKey == null || newItem != null && newItem.itemKey == "")
    //    {
    //        wing.SetImage(null, Color.white);
    //    }
    //    /* ADD ITEM */
    //    else if (newItem != null)
    //    {
    //        stats += newItem.data;

    //        var item = ItemManager.GetItem(newItem.itemKey);

    //        Debug.Log(newItem.data.ToString());
    //        switch (item.type)
    //        {
    //            case ItemType.Engine:
    //                engine.SetColor(item.color);
    //                Debug.Log($"Engine Added: {item.name}");
    //                break;
    //            case ItemType.Generator:
    //                Debug.Log($"Generator Added: {item.name}");
    //                break;
    //            case ItemType.Wing:
    //                wing.SetImage(item.Sprite, item.color);
    //                Debug.Log($"Wing Added: {item.name}");
    //                break;
    //            case ItemType.Weapon:
    //                weaponSystem.AddWeapon(item as Weapon, newItem.slot);
    //                Debug.Log($"Weapon Added: {item.name}");
    //                break;
    //            case ItemType.Shield:
    //                Debug.Log($"Shield Added: {item.name}");
    //                break;
    //            case ItemType.LightLance:
    //                lightLance.SetColor(item.color);
    //                Debug.Log($"LightLance Added: {item.name}");
    //                break;
    //            case ItemType.MiningDrill:
    //                Debug.Log($"Mining Drill Removed: {item.name}");
    //                break;
    //        }
    //    }

    //    UpdateStats();
    //}

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

        if (Settings.Steering == Settings.SteeringMode.Axis)
        {
            if (wing.AxisAlignedAim())
                engine.Move(1, _input.OverDrive);
            else engine.Move(-1, false);
        }
        else
        {
            wing.Turn(_input.HorizontalAxis);
            engine.Move(_input.VerticalAxis, _input.OverDrive);
        }

        HandleSafeZone();

        if (_input.Weapon)
            weaponSystem.TryFireWeapon(_input.WeaponSlot);


    }

    private void InitPlayer(PlayerData oldData, PlayerData newData)
    {
        SetDisplayName(displayName: newData.username);
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

    private bool hasDied = false;
    public override void OnRespawn()
    {
        base.OnRespawn();

        // Enable Children

        _input.Locked = false;

        //if (isServer)
        //    SetPosition(Server.GetPlayer(connectionToClient).lastSafePosition);

        if (hasAuthority || isServer)
            CameraFollow.Instance.SetTarget(transform);

        //if (hasDied)
        //    UpdateStats(true);
        else hasDied = true;

    }

    public override void OnDeath()
    {
        base.OnDeath();

        _input.Locked = true;
    }
}

