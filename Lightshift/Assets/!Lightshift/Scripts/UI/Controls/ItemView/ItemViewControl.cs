using Michsky.UI.ModernUIPack;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemViewControl : MonoBehaviour
{
    [SerializeField] private ItemGraphicDisplay _graphicDisplay;
    [SerializeField] private TextMeshProUGUI _lore;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private ButtonManagerBasic _button;
    public Action<ItemViewControl> onClick;
    public void Click() 
    {
        onClick?.Invoke(this);
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

    public void SetLore(string lore)
    {
        _lore.text = lore;
    }

    public void SetSprite(Sprite sprite) 
    {
        _graphicDisplay.InitializeGraphic(sprite);
    }

    public void SetColor(Color color)
    {
        _graphicDisplay.SetColor(color);
    }
}
