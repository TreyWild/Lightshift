using Mirror;
using PlayerIOClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public string connectUserId;
    public NetworkConnection connection;
    public DatabaseObject PlayerObject;
    public InventoryManager InventoryManager;
    public PlayerShip ship;

    [SyncVar(hook = nameof(OnInitPlayer))]
    private PlayerData data;

    private void OnInitPlayer(PlayerData oldData, PlayerData newData)
    {  
        PlayerManager.Instance.AddPlayer(newData);
    }

    public void InitPlayer()
    {
        data = new PlayerData 
        {
            username = Username,
            userId = connectUserId,
        };
    }
    private void OnDestroy()
    {
        PlayerManager.Instance.RemovePlayer(data);
    }

    public string Username
    {
        //Get username from database object
        get => PlayerObject.GetString("username", "");
        set
        {
            //Save new username to database object
            PlayerObject.Set("username", value);

            //We'll use this uppercase varient when checking against existing usernames.
            PlayerObject.Set("accountInfo.uppercaseUsername", value.ToUpperInvariant());
            PlayerObject.Save();
        }
    }

    public void ConsumeAuthKey() 
    {
        PlayerObject.Remove("authKey");
        PlayerObject.Save();
    }

    public int InventoryMaxCargoCapacity
    {
        get => PlayerObject.GetInt("inventory.maxCargoCapacity", 40);
        set 
        {
            PlayerObject.Set("inventory.maxCargoCapacity", value);
            PlayerObject.Save();
        }
    }

    public int InventoryMaxStorageCapacity
    {
        get => PlayerObject.GetInt("inventory.maxStorageCapacity", 40);
        set
        {
            PlayerObject.Set("inventory.maxStorageCapacity", value);
            PlayerObject.Save();
        }
    }

    public Vector2 lastSafePosition
    {
        get 
        {
            var x = PlayerObject.GetFloat("lastSafeZone.x", 0);
            var y = PlayerObject.GetFloat("lastSafeZone.y", 0);

            return new Vector2(x, y);
        }
        set
        {
            PlayerObject.Set("lastSafeZone.x", value.x);
            PlayerObject.Set("lastSafeZone.z", value.y);
            PlayerObject.Save();
        }
    }

    //public Inventory GetInventory(InventoryType type)
    //{
    //    Inventory inventory = null;

    //    var items

    //    return inventory;
    //}

    public Starship GetStarship() 
    {
        return ItemManager.GetStarship("960b64a8-dd85-4cc0-a7c3-bf3c9a2ffa04");
    }
}
