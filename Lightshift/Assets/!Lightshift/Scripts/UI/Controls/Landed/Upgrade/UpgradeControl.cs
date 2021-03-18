using Assets._Lightshift.Scripts.Data;
using Assets._Lightshift.Scripts.Utilities;
using Michsky.UI.ModernUIPack;
using SharedModels.Models.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeControl : MonoBehaviour, IPointerExitHandler
{
    public Modifier Type;

    [SerializeField]
    private TextMeshProUGUI _label;

    [SerializeField]
    private GameObject _upgradePanel;

    [SerializeField]
    private Transform _costHolderTransform;

    [SerializeField]
    private ButtonManagerBasic _button;

    [SerializeField] 
    private Image _buttonImage;

    [SerializeField] private Image _titleImage;
    [SerializeField] private GameObject _resourceItemControlPrefab;
    public Action<string, List<ResourceObject>, int, Modifier> OnUpgrade;

    private int level;

    private string id;

    private List<ResourceObject> _cost;

    private void OnDestroy()
    {
        _label = null;
        _upgradePanel = null;
        _button = null;
        _buttonImage = null;
        _titleImage = null;
        OnUpgrade = null;
        _costHolderTransform = null;
        _resourceItemControlPrefab = null;
        _cost = null;
    }

    public void Init(Upgrade upgrade, UpgradeInfo upgradeInfo, int totalUpgrades, Player player, bool upgradeLimit = false) 
    {
        if (upgrade == null)
        {
            upgrade = new Upgrade();
            upgrade.Id = upgradeInfo.Id;
        }
        Type = upgradeInfo.Type;
        level = upgrade.Level;
        id = upgrade.Id;

        // Setup display
        var value = upgradeInfo.Value / 10;
        if (value > 0)
            SetLabel($"{upgradeInfo.Type}: +{Math.Round(value, 2)}");
        else SetLabel($"{upgradeInfo.Type}: {Math.Round(value), 2}");

        _titleImage.color = ColorHelper.FromModifier(upgradeInfo.Type);

        // Setup Cost
        var multiplier = (upgrade.Level + totalUpgrades + 1) * 1.15f;
        SetupCostDisplay(upgradeInfo.ResourceCost, multiplier);

        bool affordable = player.CheckResourceAffordable(upgradeInfo.ResourceCost, multiplier);

        // Setup upgrade Display
        SetUpgradeAmount(upgrade.Level, upgradeLimit, affordable);

    }
    private void SetupCostDisplay(List<ResourceObject> resources, float multiplier) 
    {
        _cost = new List<ResourceObject>();
        foreach (var resource in resources)
        {
            var obj = Instantiate(_resourceItemControlPrefab, _costHolderTransform);
            var script = obj.GetComponent<ResourceItemControl>();

            var cost = (int)(resource.Amount * multiplier);
            script.Init(resource.Type, cost);
            _cost.Add(new ResourceObject { Type = resource.Type,  Amount = cost});
        }
    }

    private void SetLabel(string text) 
    {
        _label.text = text;
    }

    private void SetUpgradeAmount(int amount, bool limit = false, bool affordable = true) 
    {
        var statBlocks = _upgradePanel.GetComponentsInChildren<Image>();

        // Setup gray blocks
        for (int i = 0; i < 10; i++)
            statBlocks[i].color = ColorHelper.FromHex("#464646");

        //Setup green blocks
        for (int i = 0; i < amount; i++)
            statBlocks[i].color = ColorHelper.FromHex("#4AFF7D");

        if (amount == 10)
        {
            _button.buttonText = "MAX";
            _button.enabled = false;
            _buttonImage.color = ColorHelper.FromHex("#E92D4A");
        }
        else if (!limit && affordable)
        {
            _buttonImage.color = ColorHelper.FromHex("#2D96E9");
            _button.buttonText = "UPGRADE";
            _button.enabled = true;
        }
        else if (limit)
        {
            _buttonImage.color = ColorHelper.FromHex("#E97D2D");
            _button.buttonText = "LIMIT REACHED";
            _button.enabled = false;
        }
        else if (!affordable)
        {
            _buttonImage.color = ColorHelper.FromHex("#E92D66");
            _button.buttonText = "CAN'T AFFORD";
            _button.enabled = false;
        }

        _buttonText = _button.buttonText;
        _button.UpdateUI();
    }

    public void Upgrade()
    {
        if (_button.enabled)
        {
            if (_button.buttonText == "CONFIRM")
                OnUpgrade?.Invoke(id, _cost, level, Type);
            else _button.buttonText = "CONFIRM";

            _button.UpdateUI();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _button.buttonText = _buttonText;
        _button.UpdateUI();
    }

    private string _buttonText;

}
