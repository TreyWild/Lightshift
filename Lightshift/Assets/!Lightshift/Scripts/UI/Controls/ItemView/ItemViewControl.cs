using Michsky.UI.ModernUIPack;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using SharedModels.Models.Game;
using Assets._Lightshift.Scripts.Utilities;

public class ItemViewControl : MonoBehaviour
{
    [SerializeField] private ItemGraphicDisplay _graphicDisplay;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private ButtonManagerBasic _button;
    public Action<ItemViewControl> onClick;
    public Action<ItemViewControl> onClickInfo;
    public void Click() 
    {
        onClick?.Invoke(this);
    }

    public void Init(Item item)
    {
        var gameItem = ItemService.GetItem(item.ModuleId);
        SetDisplayName(gameItem.DisplayName);
        SetSprite(gameItem.Sprite);
        SetColor(item.Color);
    }

    public void ClickInfo()
    {
        onClickInfo?.Invoke(this);
    }

    public void SetButtonText(string text) 
    {
        _button.buttonText = text;
        _button.UpdateUI();
    }
    public void SetDisplayName(string display) 
    {
        _title.text = display;
    }

    public void SetSprite(Sprite sprite) 
    {
        _graphicDisplay.InitializeGraphic(sprite);
    }

    public void SetColor(Color color)
    {
        _graphicDisplay.SetColor(color);
    }

    public void SetColor(string color)
    {
        _graphicDisplay.SetColor(ColorHelper.FromHex(color));
    }
}
