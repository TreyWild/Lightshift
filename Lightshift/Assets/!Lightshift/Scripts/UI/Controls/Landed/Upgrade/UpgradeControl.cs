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
using UnityEngine.UI;

public class UpgradeControl : MonoBehaviour
{
    public Modifier Type;

    [SerializeField]
    private TextMeshProUGUI _label;

    [SerializeField]
    private GameObject _upgradePanel;

    [SerializeField]
    private ButtonManagerBasic _button;

    [SerializeField] 
    private Image _buttonImage;

    [SerializeField] private Image _titleImage;
    [SerializeField] private TextMeshProUGUI _titleLabel;

    [SerializeField] private TextMeshProUGUI _costLabel;

    public Action<Modifier, int, int> OnUpgrade;

    private int cost;

    private int level;
    public void Init(Upgrade upgrade, UpgradeInfo upgradeInfo, int totalUpgrades, int credits, bool upgradeLimit = false) 
    {
        if (upgrade == null)
        {
            upgrade = new Upgrade();
            upgrade.Type = upgradeInfo.Type;
        }
        Type = upgrade.Type;
        level = upgrade.Level;

        // Setup display
        var value = upgradeInfo.Value / 10;
        if (value > 0)
            SetLabel($"{upgrade.Type}: +{Math.Round(value, 2)}");
        else SetLabel($"{upgrade.Type}: {Math.Round(value), 2}");

        _titleImage.color = ColorHelper.FromModifier(upgrade.Type);


        // Setup Cost
        cost = upgradeInfo.Cost * (int)((upgrade.Level + totalUpgrades + 1) * 1.15f);
        _costLabel.text = $"Cost: {cost}";

        bool affordable = cost <= credits;

        // Setup upgrade Display
        SetUpgradeAmount(upgrade.Level, upgradeLimit, affordable);

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

        _button.UpdateUI();
    }

    public void Upgrade()
    {
        if (_button.enabled)
            OnUpgrade?.Invoke(Type, cost, level);
    }
}
