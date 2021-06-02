using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMPro;
using UnityEngine.UI;
using UnityEngine;
using SharedModels.Models.Game;
using Michsky.UI.ModernUIPack;
using Assets._Lightshift.Scripts.Utilities;

public class SortByModuleDialog : BaseDialog
{
    [SerializeField] private Transform _contentPanel;
    [SerializeField] private GameObject _itemPrefab;

    public Action onClearClicked;
    public Action<ItemType> onConfirm;

    private void Start()
    {
        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            var button = Instantiate(_itemPrefab, _contentPanel).GetComponent<ButtonManagerBasic>();
            button.buttonText = type.ToString();

            var image = button.GetComponent<Image>();
            image.color = ColorHelper.FromItemType(type);

            button.buttonEvent.AddListener(delegate ()
            {
                onConfirm(type);
                Exit();
            });
        }
    }

    public void Clear()
    {
        onClearClicked?.Invoke();
        onClearClicked = null;
        Destroy(gameObject);
    }

    public void Exit() 
    {
        Destroy(gameObject);
    }
}
