using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SharedModels.Models.Game;
using System.Linq;
using Assets._Lightshift.Scripts.Utilities;

public class UpgradeView : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] private TextMeshProUGUI _displayLabel;
    [SerializeField] private TextMeshProUGUI _creditsLabel;
    [SerializeField] private TextMeshProUGUI _upgradesRemainingLabel;

    [Header("Content")]
    [SerializeField] private Transform _contentTransform;

    [Header("Control")]
    [SerializeField] private GameObject _controlPrefab;

    private Item _item;
    private ModuleItem _gameItem;

    private List<UpgradeControl> _controls = new List<UpgradeControl>();

    private Player _player;

    private void Awake()
    {
        _player = FindObjectsOfType<Player>().FirstOrDefault(p => p.isLocalPlayer);

        _creditsLabel.text = $"{_player.GetProfile().Credits}";
    }
    public void InitializeUpgrades(Item item)
    {
        _item = item;

        _gameItem = ItemService.GetItem(item.ModuleId);
        if (_gameItem == null || _gameItem.Upgrades == null)
            return;

        if (_item.Upgrades == null)
            _item.Upgrades = new List<Upgrade>();

        var totalUpgrades = _item.Upgrades.Sum(s => s.Level);
        _upgradesRemainingLabel.text = $"{_gameItem.MaxUpgrades - totalUpgrades}";
        _displayLabel.text = _gameItem.DisplayName;

        foreach (var upgradeInfo in _gameItem.Upgrades)
        {
            var script = Instantiate(_controlPrefab, _contentTransform).GetComponent<UpgradeControl>();
            var upgrade = _item.Upgrades.FirstOrDefault(e => e.Type == upgradeInfo.Type);
            if (upgrade == null)
            {
                upgrade = new Upgrade { };
                upgrade.Type = upgradeInfo.Type;

                _item.Upgrades.Add(upgrade);
            }
            script.Init(upgrade, upgradeInfo, totalUpgrades, _player.GetProfile().Credits, totalUpgrades >= _gameItem.MaxUpgrades);

            script.OnUpgrade += OnUpgrade;
            _controls.Add(script);
        }
    }

    private void RefreshView() 
    {
        if (_item.Upgrades == null)
            _item.Upgrades = new List<Upgrade>();

        var totalUpgrades = _item.Upgrades.Sum(s => s.Level);
        _upgradesRemainingLabel.text = $"{_gameItem.MaxUpgrades - totalUpgrades}";
        _creditsLabel.text = $"{_player.GetProfile().Credits}";

        foreach (var upgradeInfo in _gameItem.Upgrades)
        {
            var script = _controls.FirstOrDefault(c => c.Type == upgradeInfo.Type);
            var upgrade = _item.Upgrades.FirstOrDefault(e => e.Type == upgradeInfo.Type);
            script.Init(upgrade, upgradeInfo, totalUpgrades, _player.GetProfile().Credits, totalUpgrades >= _gameItem.MaxUpgrades);
        }
    }

    private void OnUpgrade(Modifier type, int cost, int upgradeLevel)
    {
        DialogManager.ShowDialog($"Are you sure you want to upgrade <#{ColorUtility.ToHtmlStringRGB(ColorHelper.FromModifier(type))}>{type}</color> to level <#ECD961>{upgradeLevel + 1}</color> and spend <#ECD961>{cost}</color> credits?", delegate (bool result)
        {
            if (result)
            {
                _player.BuyUpgrade(_item.Id, type, delegate (Item item)
                {
                    _item = item;
                    RefreshView();
                });
            }
        });
        // TO DO : Request upgrade

    }

    public void ResetUpgrades()
    {
        DialogManager.ShowDialog($"Are you sure you want to reset ALL of your upgrades? You will get back a total of <#ECD961>{_item.SpentCredits}</color> credits!", delegate (bool result)
        {
            if (result)
            {
                _player.ResetUpgrades(_item.Id, delegate (Item item)
                {
                    _item = item;
                    RefreshView();
                });
            }
        });
    }

    public void Close()
    {
        Destroy(gameObject);
    }
}
