using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;

public class InventoryUISlot : MonoBehaviour, IPointerClickHandler
{
    public int slotId;
    public InventoryType type;
    public ItemType requiredItemType;

    public Item item;

    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _amountText;

    public event Action<InventoryUISlot> OnMouseLeftClick;
    public event Action<InventoryUISlot> OnMouseRightClick;
    public event Action<InventoryUISlot> OnMouseMiddleClick;
    public void SetItem(Item item, int amount)
    {
        if (amount == 1 || amount == 0)
            _amountText.text = "";
        else _amountText.text = $"{amount}";

        this.item = item;

        if (item != null)
        {
            _icon.sprite = item.Sprite;
            _icon.color = item.color;
        }
        else
        {
            _icon.sprite = null;
            _icon.color = new Color(1, 1, 1, .05f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button) 
        {
            case InputButton.Left:
                OnMouseLeftClick?.Invoke(this);
                break;
            case InputButton.Right:
                OnMouseRightClick?.Invoke(this);
                break;
            case InputButton.Middle:
                OnMouseMiddleClick.Invoke(this);
                break;
        }
    }
}
