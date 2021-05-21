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

public class ItemView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TMP_InputField _search;
    [SerializeField] private Transform _contentPanel;
    [SerializeField] private GameObject _itemPrefab;

    private List<Item> _items;
    private List<ItemViewControl> _itemControls;

    public Action<Item> onEquip;
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
    private Modifier _modifier = Modifier.Health;
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
        

        if (_search.text != "")
            items = items.Where(i => ItemService.GetItem(i.ModuleId) != null && ItemService.GetItem(i.ModuleId).DisplayName.Contains(_search.text)).ToList();

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
                onEquip?.Invoke(item);
            };
        }
    }

    public void SortBy() 
    {
        DialogManager.ShowModifierDialog(delegate (Modifier modifier)
        {
            _showAllModifiers = false;
            ShowItems();
        }, delegate (bool clear ) 
        {
            if (clear)
            {
                _showAllModifiers = true;
            }

            ShowItems();
        });
    }

    public void Exit()
    {
        Destroy(gameObject);
    }
}