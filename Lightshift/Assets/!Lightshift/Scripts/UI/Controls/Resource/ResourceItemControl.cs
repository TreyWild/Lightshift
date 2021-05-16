using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SharedModels.Models.Game;
using Michsky.UI.ModernUIPack;
using System;
using UnityEngine.EventSystems;

public class ResourceItemControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI _amountLabel;
    [SerializeField] private TextMeshProUGUI _toolTipLabel;
    [SerializeField] private TextMeshProUGUI _toolTipLore;
    [SerializeField] private GameObject _toolTipPanel;
    [SerializeField] private Image _icon;

    [SerializeField] private ButtonManagerBasic _button;

    public Action<ResourceType> onClicked;

    private ResourceType _type;

    public void Init(ResourceType type, int amount, bool showButton = false, string buttonText = "")
    {
        _amountLabel.text = $"{amount}";

        var resource = ItemService.GetResourceItem(type);

        if (resource == null)
            return;

        _icon.sprite = resource.Sprite;

        _button.gameObject.SetActive(showButton);
        _button.buttonText = buttonText;
        _button.UpdateUI();

        _type = type;

        _toolTipLabel.text = resource.DisplayName;
        _toolTipLore.text = resource.Lore;
        _toolTipPanel.SetActive(false);
    }

    public void ButtonClicked() 
    {
        onClicked?.Invoke(_type);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _toolTipPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _toolTipPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        _amountLabel = null;
        _toolTipLabel = null;
        _toolTipLore = null;
        _toolTipPanel = null;
        _icon = null;
        _button = null;
        onClicked = null;
    }
}
