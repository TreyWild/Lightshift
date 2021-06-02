using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SharedModels.Models.Game;
using Assets._Lightshift.Scripts.Utilities;

public enum ItemViewType 
{
    Upgrade,
    Equip
}
public class ItemView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TMP_InputField _search;
    [SerializeField] private Transform _contentPanel;
    [SerializeField] private GameObject _itemPrefab;

    public ItemViewType type;
    
    private List<Item> _items;
    private List<ItemViewControl> _itemControls;

    public Action<Item> onClick;
    public void Init(List<Item> items, string title = "Select Module")  
    {
        _items = items;
        if (_items == null)
            _items = new List<Item>();

        ShowItems();

        _search.onValueChanged.AddListener (delegate (string text) { ShowItems(); });
        _title.text = title;
    }

    private bool _showAllModifiers = true;
    private bool _showAllModules = true;

    private Modifier _modifier = Modifier.Health;
    private ItemType _type = ItemType.MiningDrill;

    public void ShowItems() 
    {
        if (_itemControls != null)
        {
            for (int r = 0; r < _itemControls.Count; r++)
                Destroy(_itemControls[r].gameObject);
        }
        _itemControls = new List<ItemViewControl>();

        var items = _items.ToList();
        if (!_showAllModifiers)
            items = items.Where(i => StatHelper.GetStatsFromItem(i).FirstOrDefault(s => s.Type == _modifier) != null).ToList();

        if (!_showAllModules)
            items = items.Where(i => ItemService.GetItem(i.ModuleId) != null && ItemService.GetItem(i.ModuleId).Type == _type).ToList();


        if (_search.text != "")
            items = items.Where(i => ItemService.GetItem(i.ModuleId) != null && ItemService.GetItem(i.ModuleId).DisplayName.ToUpper().Contains(_search.text.ToUpper())).ToList();

        foreach (var item in items)
        {
            var obj = Instantiate(_itemPrefab, _contentPanel);
            var control = obj.GetComponent<ItemViewControl>();
            control.Init(item);

            _itemControls.Add(control);

            control.onClickInfo += (c) => 
            {
                
            };

            control.onClick += (c) =>
            {
                onClick?.Invoke(item);
            };
        }
    }

    public void SortBy() 
    {
        if (type == ItemViewType.Equip)
            DialogManager.ShowModifierDialog(delegate (Modifier modifier)
            {
                _showAllModifiers = false;
                _modifier = modifier;
                ShowItems();
            }, delegate (bool clear)
            {
                if (clear)
                {
                    _showAllModifiers = true;
                }

                ShowItems();
            });
        else if (type == ItemViewType.Upgrade)
        {
            DialogManager.ShowSortByModuleDialog(delegate (ItemType type)
            {
                _showAllModules = false;
                _type = type;
                ShowItems();
            }, delegate (bool clear)
            {
                if (clear)
                {
                    _showAllModules = true;
                }

                ShowItems();
            });
        }
    }

    public void Exit()
    {
        Destroy(gameObject);
    }
}