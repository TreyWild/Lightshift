using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SharedModels.Models.Game;
using System.Linq;
using Assets._Lightshift.Scripts.Utilities;
using Mirror;

public class UpgradeView : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] private TextMeshProUGUI _displayLabel;
    [SerializeField] private TextMeshProUGUI _upgradesRemainingLabel;

    [Header("Content")]
    [SerializeField] private Transform _contentTransform;

    [Header("Resources")]
    [SerializeField] private Transform _resourceTransform;
    [SerializeField] private GameObject _resourceItemControlPrefab;

    [Header("Control")]
    [SerializeField] private GameObject _controlPrefab;

    [Header("Reset Upgrade Prefab")]
    [SerializeField] private GameObject _resetUpgradesPrefab;

    private Item _item;
    private ModuleItem _gameItem;

    private List<UpgradeControl> _controls = new List<UpgradeControl>();

    private string itemId;
    private Player _player 
    {
        get 
        {
            if (_playerCache == null)
                _playerCache = FindObjectsOfType<Player>().FirstOrDefault(p => p.isLocalPlayer);

            return _playerCache;
        }
        set => _playerCache = value;
    }
    private Player _playerCache;
    private void Awake()
    {
        if (_player != null)
        {
            _player.Resources.Callback += OnResourceChanged;
            _player.Items.Callback += OnItemsChanged;
            LoadResources();
        }
    }

    private void OnItemsChanged(SyncIDictionary<string, Item>.Operation op, string key, Item item)
    {
        RefreshView();
    }

    private void OnResourceChanged(SyncIDictionary<ResourceType, int>.Operation op, ResourceType key, int item)
    {
        LoadResources();
    }

    public void InitializeUpgrades(Item item)
    {
        _item = item;
        itemId = item.Id;

        _gameItem = ItemService.GetItem(item.ModuleId);
        if (_gameItem == null || _gameItem.Upgrades == null)
            return;

        var upgrades = new List<Upgrade>();
        if (_item.Upgrades != null)
            upgrades = _item.Upgrades.ToList();

        var totalUpgrades = upgrades.Sum(s => s.Level);
        _upgradesRemainingLabel.text = $"{_gameItem.MaxUpgrades - totalUpgrades}";
        _displayLabel.text = _gameItem.DisplayName;

        foreach (var upgradeInfo in _gameItem.Upgrades)
        {
            var script = _controls.FirstOrDefault(c => c.Type == upgradeInfo.Type);
            if (script == null)
            {
                script = Instantiate(_controlPrefab, _contentTransform).GetComponent<UpgradeControl>();
                script.OnUpgrade += OnUpgrade;
            }
            var upgrade = upgrades.FirstOrDefault(e => e.Id == upgradeInfo.Id);
            if (upgrade == null)
            {
                upgrade = new Upgrade { };
                upgrade.Id = upgradeInfo.Id;

                upgrades.Add(upgrade);
            }
            script.Init(upgrade, upgradeInfo, totalUpgrades, _player, totalUpgrades >= _gameItem.MaxUpgrades);
            _controls.Add(script);
        }

        item.Upgrades = upgrades.ToArray();
    }

    private List<ResourceItemControl> _resourceList = new List<ResourceItemControl>();
    private void LoadResources()
    {
        if (_resourceList == null)
            _resourceList = new List<ResourceItemControl>();

        foreach (var resource in _resourceList)
        {
            if (resource == null)
                continue;
            if (resource.gameObject != null)
            {
                Destroy(resource.gameObject);
            }
        }

        _resourceList.Clear();
        _resourceList = new List<ResourceItemControl>();

        var resources = _player.GetResources();
        foreach (var resource in resources)
        {
            var obj = Instantiate(_resourceItemControlPrefab, _resourceTransform);
            var script = obj.GetComponent<ResourceItemControl>();
            script.Init(resource.Type, resource.Amount);
            _resourceList.Add(script);
        }
    }
    private void RefreshView() 
    {
        _item = _player.GetItem(itemId);
        InitializeUpgrades(_item);
        //if (_item == null)
        //{
        //    Debug.LogError("UpgradeView:RefreshView:Item is null");
        //    return;
        //}

        //_gameItem = ItemService.GetItem(_item.ModuleId);
        //if (_gameItem == null || _gameItem.Upgrades == null)
        //    return;

        //if (_item.Upgrades == null)
        //    _item.Upgrades = new List<Upgrade>();

        //var totalUpgrades = _item.Upgrades.Sum(s => s.Level);
        //_upgradesRemainingLabel.text = $"{_gameItem.MaxUpgrades - totalUpgrades}";
        //_displayLabel.text = _gameItem.DisplayName;

        //foreach (var upgradeInfo in _gameItem.Upgrades)
        //{
        //    var script = _controls.FirstOrDefault(c => c.Type == upgradeInfo.Type);
        //    var upgrade = _item.Upgrades.FirstOrDefault(e => e.Id == upgradeInfo.Id);
        //    script.Init(upgrade, upgradeInfo, totalUpgrades, _player, totalUpgrades >= _gameItem.MaxUpgrades);
        //}
    }

    private void OnUpgrade(string id, List<ResourceObject> cost, int upgradeLevel, Modifier type)
    {
        //Debug.LogError($"Upgrade ID (UpgradeView): {id}");

        _player.BuyUpgrade(_item.Id, id, delegate (string itemId)
        {
            RefreshView();
        });
    }

    public void ResetUpgrades()
    {
        var obj = Instantiate(_resetUpgradesPrefab);
        var script = obj.GetComponent<UpgradeResetView>();

        script.Init(_item.SpentResources.ToList());
        script.OnReset += () => 
        {
            _player.ResetUpgrades(_item.Id, delegate (string itemId)
            {
                RefreshView();
            });
        };
        // TO DO : Request upgrade

        //DialogManager.ShowDialog($"Are you sure you want to reset ALL of your upgrades? You will get back a total of <#ECD961>{_item.SpentCredits}</color> credits!", delegate (bool result)
        //{
        //    if (result)
        //    {
        //        _player.ResetUpgrades(_item.Id, delegate (Item item)
        //        {
        //            _item = item;
        //            RefreshView();
        //        });
        //    }
        //});
    }

    public void Close()
    {
        Destroy(gameObject);
    }
}
