using Mirror;
using PlayerIOClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public string connectUserId;
    public Ship playerShip;
    public NetworkConnection connection;
    public DatabaseObject PlayerObject;

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
        }
    }

    public int InventoryMaxStorageCapacity
    {
        get => PlayerObject.GetInt("inventory.maxStorageCapacity", 40);
        set
        {
            PlayerObject.Set("inventory.maxStorageCapacity", value);
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
        return ItemManager.GetStarship("default");
    }
}
