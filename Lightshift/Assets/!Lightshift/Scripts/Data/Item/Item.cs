using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Item", menuName = "Lightshift/Create Item", order = 2)]

public class Item : ScriptableObject
{
    public string key = Guid.NewGuid().ToString();
    public string displayName;
    public string lore;
    public ItemType type;

    public ModuleData data;

    public Sprite Icon;
    public Sprite Sprite;

    public Color color = Color.white;

    public int maxStack = 999;
}

public class Equip
{
    public int slot;
    public string itemKey;
    public ModuleData data;

    public static Equip GetEquipFromItemSlot(InventorySlot itemSlot) 
    {
        return new Equip
        {
            itemKey = itemSlot.item.key,
            data = itemSlot.item.data,
            slot = itemSlot.slotId,
        };
    }

    //public static Item GetItemFromEquip(Equip equip) 
    //{
    //    var item = ItemManager.GetItem(equip.itemKey);
    //    item.data = equip.data;
    //    return item;
    //}
}

public enum ItemType 
{
    None,
    Scrap,
    Engine,
    Wing,
    Hull,
    Weapon,
    Generator,
    LightLance,
    Shield,
    MiningDrill,
}