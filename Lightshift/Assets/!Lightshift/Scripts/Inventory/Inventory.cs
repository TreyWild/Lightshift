using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InventoryType 
{
    Ship,
    Cargo,
    Storage
}

[Serializable]
public class Inventory 
{
    public InventoryType type;

    public List<InventorySlot> slots = new List<InventorySlot>();

    public void RemoveItem(InventorySlot slot) 
    {
        if (slots.Contains(slot))
            slots.Remove(slot);
    }

    public void AddItem(InventorySlot slot) 
    {
        if (!slots.Contains(slot))
            slots.Add(slot);
    }
}
