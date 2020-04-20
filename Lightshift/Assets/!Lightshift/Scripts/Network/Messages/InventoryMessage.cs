using UnityEngine;
using System.Collections;
using Mirror;

public class InventoryItemSlotMessage : MessageBase
{
    public int slotId;
    public int amount;

    public string itemKey;
    public ModuleData data;

    public InventoryType inventory;
}
