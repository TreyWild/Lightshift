using UnityEngine;
using System.Collections;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Security.Cryptography.X509Certificates;

public class InventoryManager : NetworkBehaviour
{
    private Player _player => Server.GetPlayer(connectionToClient);

    [SerializeField] private Inventory _shipInventory;
    [SerializeField] private Inventory _storageInventory;
    [SerializeField] private Inventory _cargoInventory;

    private InventoryUI _inventoryUI;
    [SerializeField] private GameObject _inventoryPrefab;

    private InventorySlot _heldSlot;
    private bool _holdingItem;

    public override void OnStartClient()
    {
        base.OnStartClient();
        CmdRequestInventory();
    }

    private List<InventoryItemSlotMessage> AdjustSlots(Inventory inventory, InventoryType type)
    {
        var list = new List<InventoryItemSlotMessage>();
        foreach (var slot in inventory.slots)
        {
            slot.inventory = type;
            list.Add(GetInventorySlotMessage(slot));
        }
        return list;
    }

    [Command]
    public void CmdOnMouseLeftClick(int slot, InventoryType inventoryType)
    {
        var inventory = GetInventory(inventoryType);
        var itemSlot = inventory.slots.FirstOrDefault(s => s.slotId == slot);
        if (itemSlot == null)
        {
            itemSlot = new InventorySlot
            {
                inventory = inventoryType,
                slotId = slot,
            };

            inventory.AddItem(itemSlot);
        }

        bool itemSlotEmpty = (itemSlot.item == null);

        // Prevent Cheating extra slots
        if (inventoryType == InventoryType.Cargo && slot > _player.InventoryMaxCargoCapacity)
            return;
        else if (inventoryType == InventoryType.Storage && slot > _player.InventoryMaxStorageCapacity)
            return;
        else if (slot < 0)
            return;

        //PLACE HELD ITEM ON EMPTY SLOT
        if (itemSlotEmpty && _holdingItem)
        {
            // Make sure stuff can't be put where it shouldn't be..
            if (itemSlot != null)
                if (itemSlot.identifier != _heldSlot.item.type && itemSlot.type != ItemType.None)
                    return;

            itemSlot.amount = _heldSlot.amount;
            itemSlot.item = _heldSlot.item;

            UpdateSlot(itemSlot);
            ClearHeldItem();

        } // ADD TO EXISTING ITEM STACK STACK
        //else if (itemSlot != null && _heldSlot != null && _heldSlot.item != null) 
        //{
        //    if (itemSlot.item.type != _heldSlot.item.type)
        //        return;

        //    var total = itemSlot.amount + _heldSlot.amount;
        //    var remaining = total - itemSlot.item.maxStack;
        //    if (remaining > 0)
        //    {
        //        itemSlot.amount = itemSlot.item.maxStack;
        //        _heldSlot.amount = remaining;
        //        SetHeldItem(_heldSlot);
        //    }
        //    else ClearHeldItem();

        //    UpdateSlot(itemSlot);
        //} // PICKUP ITEM FROM SLOT
        else if (!itemSlotEmpty && !_holdingItem)
        {
            SetHeldItem(itemSlot);
            RemoveItem(itemSlot);
        }
    }

    [Command]
    public void CmdOnMouseRightClick(int slot, InventoryType inventoryType)
    {
        //Debug.LogError($"Recieved {slot} {inventoryType}");
        //var inventory = GetInventory(inventoryType);
        //var itemSlot = inventory.slots.FirstOrDefault(s => s.slotId == slot);

        //// Prevent Cheating extra slots
        //if (inventoryType == InventoryType.Cargo && slot > _player.InventoryMaxCargoCapacity)
        //    return;
        //else if (inventoryType == InventoryType.Storage && slot > _player.InventoryMaxStorageCapacity)
        //    return;
        //else if (slot < 0)
        //    return;

        ////There's no item slot where the player clicked. Add one from hand?
        //if (itemSlot == null)
        //{
        //    Debug.LogError("It's null");
        //    // Check to see if we're holding anything
        //    if (_heldSlot != null && _heldSlot.item != null)
        //    {
        //        var newSlot = new InventorySlot
        //        {
        //            amount = 1,
        //            inventory = inventoryType,
        //            item = _heldSlot.item,
        //            slotId = slot,
        //        };

        //        if (_heldSlot.amount == 1)
        //        {
        //            ClearHeldItem();
        //        }
        //        else
        //        {
        //            _heldSlot.amount -= 1;
        //        }

        //        inventory.AddItem(newSlot);
        //        UpdateSlot(newSlot);
        //        SetHeldItem(_heldSlot);
        //    }
        //}
        //else // Item Slot is not null. Pickup / Add to pile 
        //{
        //    // Check to see if we're holding anything
        //    if (_heldSlot != null && _heldSlot.item != null)
        //    {
        //        if (itemSlot == null || itemSlot.item == null)
        //        itemSlot = new InventorySlot
        //        {
        //            inventory = inventoryType,
        //            item = _heldSlot.item,
        //            slotId = slot,
        //        };

        //        itemSlot.amount += 1;

        //        if (_heldSlot.amount == 1)
        //        {
        //            ClearHeldItem();
        //        }
        //        else
        //        {
        //            _heldSlot.amount -= 1;
        //        }

        //        inventory.AddItem(itemSlot);
        //        UpdateSlot(itemSlot);
        //        SetHeldItem(_heldSlot);
        //    }
        //    else if ((_heldSlot == null || _heldSlot.item == null) && itemSlot.item != null) // SPLIT STACK IN HALF
        //    {
        //        if (itemSlot.amount == 1)
        //        {
        //            SetHeldItem(itemSlot);
        //            inventory.RemoveItem(itemSlot);
        //            UpdateSlot(itemSlot);
        //        }
        //        else
        //        {

        //            int largerValue = (itemSlot.amount / 2) + itemSlot.amount%2;
        //            int smallerValue = itemSlot.amount / 2;

        //            var newSlot = new InventorySlot
        //            {
        //                inventory = inventoryType,
        //                item = itemSlot.item,
        //                slotId = slot,
        //                amount = smallerValue,
        //            };

        //            itemSlot.amount = largerValue;

        //            SetHeldItem(newSlot);
        //            UpdateSlot(itemSlot);
        //        }
        //    }
        //}
    }

    [Command]
    public void CmdOnMouseMiddleClick(int slot, InventoryType inventoryType)
    {

    }


    [ClientRpc]
    private void RpcBuildInventory(int maxCargo, int maxStorage)
    {
        if (!hasAuthority)
            return;

        _inventoryUI = Instantiate(_inventoryPrefab).GetComponent<InventoryUI>();
        _inventoryUI.BuildInventory(this, maxCargo, maxStorage);

        CmdRequestInventorySlots();
    }

    private void SetHeldItem(InventorySlot slot)
    {
        _heldSlot = CloneSlot(slot);
        _holdingItem = true;
        RpcSetHeldItemSlot(GetInventorySlotMessage(slot));
    }

    public InventorySlot CloneSlot(InventorySlot slot) 
    {
        var newSlot = new InventorySlot
        {
            amount = slot.amount,
            identifier = slot.identifier,
            inventory = slot.inventory,
            item = slot.item,
            slotId = slot.slotId,
            type = slot.type,
        };

        return newSlot;
    }

    private void ClearHeldItem() 
    {
        _heldSlot = null;
        _holdingItem = false;
        RpcClearHeldItemSlot();
    }

    public Action<InventorySlot> onEquipChanged;
    private void UpdateSlot(InventorySlot slot) 
    {
        if (slot.inventory == InventoryType.Ship)
            onEquipChanged?.Invoke(slot);

        RpcUpdateInventorySlot(GetInventorySlotMessage(slot));
    }

    private void RemoveItem(InventorySlot slot) 
    {
        slot.item = null;
        slot.amount = 0;

        UpdateSlot(slot);
    }

    [ClientRpc]
    private void RpcClearHeldItemSlot() 
    {
        _inventoryUI.ClearHeldItem();
    }

    [ClientRpc]
    private void RpcSetHeldItemSlot(InventoryItemSlotMessage item)
    {
        _inventoryUI.SetHeldItem(item);
    }

    public InventoryItemSlotMessage GetInventorySlotMessage(InventorySlot slot)
    {
        var message = new InventoryItemSlotMessage
        {
            amount = slot.amount,
            inventory = slot.inventory,

            slotId = slot.slotId
        };

        if (slot.item != null)
        {
            message.itemKey = slot.item.key;
            message.moduleData = slot.item.data;

            if (slot.item.type == ItemType.Weapon)
                message.weaponData = (slot.item as Weapon).weaponData;
        }

        return message;
    }

    [ClientRpc]
    private void RpcUpdateInventorySlot(InventoryItemSlotMessage slot)
    { 
        if (!hasAuthority)
            return;

        _inventoryUI.UpdateSlot(slot);
    }

    [Command]
    private void CmdRequestInventory()
    {
        RpcBuildInventory(_player.InventoryMaxCargoCapacity, _player.InventoryMaxStorageCapacity);
    }

    [Command]
    private void CmdRequestInventorySlots()
    {
        var slots = new List<InventoryItemSlotMessage>();
        slots.AddRange(AdjustSlots(_shipInventory, InventoryType.Ship));
        slots.AddRange(AdjustSlots(_storageInventory, InventoryType.Storage));
        slots.AddRange(AdjustSlots(_cargoInventory, InventoryType.Cargo));

        foreach (var slot in slots)
            RpcUpdateInventorySlot(slot);
    }


    public Inventory GetInventory(InventoryType inventoryType) 
    {
        switch (inventoryType) 
        {
            case InventoryType.Cargo:
                return _cargoInventory;
            case InventoryType.Ship:
                return _shipInventory;
            default:
                return _storageInventory;
        }
    }

    public List<Equip> GetEquippedWeapons() 
    {
        var list = new List<Equip>();
        foreach (var slot in _shipInventory.slots)
            if (slot.item != null)
                if (slot.identifier == ItemType.Weapon)
                    list.Add(Equip.GetEquipFromItemSlot(slot));

        return list;
    }

    public List<Equip> GetAllEquips()
    {
        var list = new List<Equip>();
        foreach (var slot in _shipInventory.slots)
            if (slot.item != null)
                    list.Add(Equip.GetEquipFromItemSlot(slot));

        return list;
    }
}
