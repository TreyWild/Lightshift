using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class InventorySlot
{
    public int slotId;
    public Item item;
    public int amount;
    public ItemType type;
    public InventoryType inventory;
    public ItemType identifier;
}
