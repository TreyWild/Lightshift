using Lightshift;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public List<InventoryUISlot> shipInventorySlots = new List<InventoryUISlot>();
    public List<InventoryUISlot> mainInventorySlots = new List<InventoryUISlot>();
    public List<InventoryUISlot> storageInventorySlots = new List<InventoryUISlot>();
    public GameObject shipInventoryPanel;
    public GameObject mainInventoryPanel;
    public GameObject storageInventoryPanel;
    public GameObject itemSlotPrefab;
    public GameObject heldItemPrefab;
    public GameObject toolTipPrefab;

    private GameObject _toolTipObject;
    private InventoryManager _manager;
    public void BuildInventory(InventoryManager manager, int maxCargo, int maxStorage)
    {
        _manager = manager;

        storageInventoryPanel.SetActive(false);
        shipInventoryPanel.SetActive(true);

        var shipslots = shipInventoryPanel.GetComponentsInChildren<InventoryUISlot>();
        shipInventorySlots.AddRange(shipslots.ToList());

        GenerateCargoSlots(maxCargo);
        GenerateStorageSlots(maxStorage);

        var list = shipInventorySlots;
        list.AddRange(mainInventorySlots);
        list.AddRange(storageInventorySlots);

        foreach (var slot in list)
        {
            slot.OnMouseLeftClick += OnMouseLeftClick;
            slot.OnMouseMiddleClick += OnMouseMiddleClick;
            slot.OnMouseRightClick += OnMouseRightClick;
            slot.OnMouseLeave += OnMouseLeaveSlot;
            slot.OnMouseOver += OnMouseOverSlot;
        }

        gameObject.SetActive(false);
        GameUIManager.Instance.HookInventoryUI(this);
    }

    private void GenerateCargoSlots(int maxCargoSlots = 40)
    {
        for (int i = 0; i < maxCargoSlots; i++)
        {
            var itemSlot = Instantiate(itemSlotPrefab, mainInventoryPanel.transform);
            var slot = itemSlot.GetComponent<InventoryUISlot>();
            slot.slotId = i;
            slot.type = InventoryType.Cargo;
            mainInventorySlots.Add(slot);
        }
    }

    private void GenerateStorageSlots(int maxStorageSlots = 40)
    {
        for (int i = 0; i < maxStorageSlots; i++)
        {
            var itemSlot = Instantiate(itemSlotPrefab, storageInventoryPanel.transform);
            var slot = itemSlot.GetComponent<InventoryUISlot>();
            slot.slotId = i;
            slot.type = InventoryType.Storage;
            storageInventorySlots.Add(slot);
        }
    }

    public void InitInventory(List<InventoryItemSlotMessage> items)
    {
        foreach (var item in items)
            UpdateSlot(item);
    }


    public void UpdateSlot(InventoryItemSlotMessage slot)
    {
        List<InventoryUISlot> slots = null;
        switch (slot.inventory)
        {
            case InventoryType.Ship:
                slots = shipInventorySlots;
                break;
            case InventoryType.Cargo:
                slots = mainInventorySlots;
                break;
            case InventoryType.Storage:
                slots = storageInventorySlots;
                break;
        }

        var itemSlot = slots.FirstOrDefault(s => s.slotId == slot.slotId);
        if (itemSlot == null)
        {
            return;
        }

        itemSlot.type = slot.inventory;

        ItemObj item = null;
        var itemObj = ItemManager.GetItem(slot.itemKey);
        if (itemObj != null)
            item = itemObj.GetItemObj();
        if (item == null)
        {
            itemSlot.SetItem(null, 0);
        }

        else
        {
            item.data = slot.moduleData;
            if (item.type == ItemType.Weapon)
                item.weaponData = slot.weaponData;

            itemSlot.SetItem(item, slot.amount);
        }
    }

    private float _timeSinceLastClick;
    private bool _canClick = true;
    private void FixedUpdate()
    {
        _timeSinceLastClick += Time.fixedDeltaTime;
        if (_timeSinceLastClick > .1f)
        {
            _canClick = true;
            _timeSinceLastClick = 0;
        }
    }

    private void OnMouseRightClick(InventoryUISlot obj)
    {
        if (!_canClick)
            return;

        _canClick = false;
        _manager.CmdOnMouseRightClick(obj.slotId, obj.type);
    }

    private void OnMouseMiddleClick(InventoryUISlot obj)
    {
        if (!_canClick)
            return;

        _canClick = false;

        _manager.CmdOnMouseMiddleClick(obj.slotId, obj.type);
    }

    private void OnMouseLeftClick(InventoryUISlot obj)
    {
        if (!_canClick)
            return;

        _canClick = false;
        _manager.CmdOnMouseLeftClick(obj.slotId, obj.type);
    }

    public void Exit()
    {
        gameObject.SetActive(false);
        Settings.Instance.KeysLocked = false;
    }

    private GameObject _heldItem;
    public void SetHeldItem(InventoryItemSlotMessage slot)
    {
        ClearHeldItem();
        _heldItem = Instantiate(heldItemPrefab);

        ItemObj item = null;
        var itemObj = ItemManager.GetItem(slot.itemKey);
        if (itemObj != null)
            item = itemObj.GetItemObj();
        if (item == null)

        {
            //Debug.LogError("Null Item");
        }

        else
        {
            item.data = slot.moduleData;
            if (item.type == ItemType.Weapon)
                item.weaponData = slot.weaponData;
            var itemSlot = _heldItem.GetComponentInChildren<InventoryUISlot>();
            itemSlot.SetItem(item, slot.amount);
        }
    }

    public void ClearHeldItem()
    {
        if (_heldItem != null)
            Destroy(_heldItem.gameObject);
    }

    private void OnMouseOverSlot(InventoryUISlot obj)
    {
        if (obj.item == null)
            return;

        if (_toolTipObject != null)
            Destroy(_toolTipObject);

        _toolTipObject = Instantiate(toolTipPrefab);
        var tooltip = _toolTipObject.GetComponent<InventoryToolTip>();

        tooltip.DisplayTooltip(obj.item);
    }

    private void OnMouseLeaveSlot(InventoryUISlot obj)
    {
        if (_toolTipObject != null)
            Destroy(_toolTipObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(Settings.Instance.InventoryKey))
            Exit();
    }
}
