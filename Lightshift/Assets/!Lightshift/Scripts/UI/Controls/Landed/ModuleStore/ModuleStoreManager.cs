using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SharedModels.Models.Game;
using Assets._Lightshift.Scripts.Utilities;
using UnityEngine.EventSystems;
using System.Linq;
using Assets._Lightshift.Scripts.SolarSystem;
using System;

public class ModuleStoreManager : MonoBehaviour
{
    [SerializeField] private Transform _itemPanel;
    [SerializeField] private GameObject _itemPrefab;

    [SerializeField] private Transform _statPanel;
    [SerializeField] private GameObject _statObjectPrefab;

    [SerializeField] private Transform _costPanel;
    [SerializeField] private GameObject _resourcePrefab;

    [SerializeField] private TextMeshProUGUI _creditCost;
    [SerializeField] private TextMeshProUGUI _creditAmount;

    [SerializeField] private TextMeshProUGUI _displayNameLabel;
    [SerializeField] private Image _displayNameColor;

    [SerializeField] private TextMeshProUGUI _lore;
    [SerializeField] private TextMeshProUGUI _maxUpgradesLabel;

    [SerializeField] private GameObject _ownedPanel;

    private ModuleStoreLandable _store;
    private Player _player;
    private bool _owned;

    private void Awake()
    {
        _player = FindObjectsOfType<Player>().FirstOrDefault(p => p.isLocalPlayer);

        if (_player != null)
        {
            _player.onCreditsChanged += (credits) =>
            {
                _creditAmount.text = credits.ToString();
            };

            _player.Items.Callback += Items_Callback;

            var landable = LandableManager.GetLandableById(_player.LandedLocationId);
            if (landable == null)
            {
                //Debug.LogError($"Landable is NULL.");

                return;
            }

            var store = landable as ModuleStoreLandable;
            if (store == null)
            {
                //Debug.LogError($"Store Landable is NULL.");
                return;
            }

            _store = store;
            PopulateStoreItems();

        }

    }

    private void Items_Callback(Mirror.SyncIDictionary<string, Item>.Operation op, string key, Item item)
    {
        SelectItem(_storeControls.FirstOrDefault());
    }

    private List<ModuleStoreItem> _storeControls = new List<ModuleStoreItem>();

    public void PopulateStoreItems() 
    {
        if (_store == null)
            return;
        if (_store.Items == null)
            return;

        foreach (var item in _store.Items)
        {
            var obj = Instantiate(_itemPrefab, _itemPanel);
            var control = obj.GetComponent<ModuleStoreItem>();

            control.Init(item);
            control._onSelect += OnStoreItemSelected;

            _storeControls.Add(control);
        }

        SelectItem(_storeControls.FirstOrDefault());


    }

    private ModuleItem _selectedItem;
    private void SelectItem(ModuleStoreItem obj)
    {
        if (obj == null)
            return;

        _selectedItem = ItemService.GetItem(obj.item.Id);
        

        // Ensure only one control is visually selected
        foreach (var control in _storeControls)
        {
            if (control == obj)
                control.SetSelected();
            else
                control.SetSelected(false);
        }

        PopulateItemInformation();
    }

    private void OnStoreItemSelected(ModuleStoreItem obj)
    {
        SelectItem(obj);
    }

    private void PopulateItemInformation() 
    {
        _displayNameLabel.text = _selectedItem.DisplayName;
        _lore.text = _selectedItem.Lore;
        _displayNameColor.color = _selectedItem.DisplayColor;

        _creditAmount.text = _player.Credits.ToString();
        _creditCost.text = _selectedItem.creditsCost.ToString();

        PopulateResourceCost();

        PopulateStats();

        _owned = _player.Items.Any(s => s.Value.ModuleId == _selectedItem.Id);
        _ownedPanel.SetActive(_owned);
    }

    public void BuyWithResources() 
    {
        if (_owned)
        {
            DialogManager.ShowMessage($"You already own this item, silly!");
            return;
        }
        string message = $"Are you sure that you want to purchase <color=#{ColorHelper.ToHex(_selectedItem.DisplayColor)}>{_selectedItem.DisplayName}</color> with ";
        string line = "";

        foreach (var resource in _selectedItem.resourceCost)
        {
            if (line != "")
                line += ", ";
            line += $"{resource.Amount} <color=#{ColorHelper.ToHex(ColorHelper.FromResourceType(resource.Type))}>{resource.Type.ToString().ToSentence()}</color>";
        }

        DialogManager.ShowDialog(message + line + "?", delegate (bool result)
        {
            if (result) 
            {
                var affordable = _player.CheckResourceAffordable(_selectedItem.resourceCost);
                if (affordable)
                    _player.BuyModuleWithResources(_selectedItem.Id);
                else DialogManager.ShowMessage($"You cannot afford that!");
            }
        });
    }

    public void BuyWithCredits() 
    {
        if (_owned)
        {
            DialogManager.ShowMessage($"You already own this item, silly!");
            return;
        }

        string message = $"Are you sure that you want to purchase <color=#{ColorHelper.ToHex(_selectedItem.DisplayColor)}>{_selectedItem.DisplayName}</color> with {_selectedItem.creditsCost} Credits?";

        DialogManager.ShowDialog(message, delegate (bool result)
        {
            if (result)
            {
                if (_player.Credits >= _selectedItem.creditsCost)
                    _player.BuyModuleWithCredits(_selectedItem.Id);
                else DialogManager.ShowMessage($"You cannot afford that!");
            }
        });
    }


    private List<GameObject> _resourceUICache = new List<GameObject>();
    private void PopulateResourceCost() 
    {
        _maxUpgradesLabel.text = $"Max Upgrades: {_selectedItem.MaxUpgrades}";

        if (_resourceUICache.Count > 0)
        for (int i = 0; i < _resourceUICache.Count; i++)
            Destroy(_resourceUICache[i]);
        

        _resourceUICache.Clear();

        foreach (var resource in _selectedItem.resourceCost)
        {
            var obj = Instantiate(_resourcePrefab, _costPanel).GetComponent<ResourceItemControl>();
            obj.Init(resource.Type, resource.Amount);

            _resourceUICache.Add(obj.gameObject);
        }
    }

    private List<GameObject> _statUICache = new List<GameObject>();
    private void PopulateStats()
    {
        if (_statUICache.Count > 0)
            for (int i = 0; i < _statUICache.Count; i++)
            Destroy(_statUICache[i]);


        _statUICache.Clear();

        var indexed = new List<GameModifier>();
        foreach (var stat in _selectedItem.Stats)
        {
            var obj = Instantiate(_statObjectPrefab, _statPanel).GetComponent<ModuleStatControl>();

            var potential = _selectedItem.Upgrades.Where(s => s.Type == stat.Type).Sum(s => s.Value);
            obj.Init(stat, potential);

            indexed.Add(stat);
            _statUICache.Add(obj.gameObject);
        }

        foreach (var stat in _selectedItem.Upgrades)
        {
            if (indexed.Any(s => s.Type == stat.Type))
                continue;

            var obj = Instantiate(_statObjectPrefab, _statPanel).GetComponent<ModuleStatControl>();

            var potential = _selectedItem.Upgrades.Where(s => s.Type == stat.Type).Sum(s => s.Value);
            obj.Init(new GameModifier { Type = stat.Type, Value = 0 }, potential);

            _statUICache.Add(obj.gameObject);
        }

    
    }

    public void ExitStore()
    {
        _player.TakeOff();
    }

}
