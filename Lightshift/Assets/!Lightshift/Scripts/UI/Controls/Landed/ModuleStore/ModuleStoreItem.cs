using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SharedModels.Models.Game;
using Assets._Lightshift.Scripts.Utilities;
using UnityEngine.EventSystems;
using System;

public class ModuleStoreItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameLabel;
    //[SerializeField] private TextMeshProUGUI _nameBackground;
    [SerializeField] private ItemGraphicDisplay _icon;
    [SerializeField] private Image _background;

    public ModuleItem item;
    public void Init(ModuleItem item) 
    {
        this.item = item;
        _nameLabel.text = item.DisplayName;
        _icon.InitializeGraphic(item.Sprite);
    }

    public Action<ModuleStoreItem> _onSelect;
    public void Click()
    {
        //SetSelected();
        _onSelect?.Invoke(this);
    }

    public void SetSelected(bool selected = true)
    {
        if (selected)
            _background.color = ColorHelper.FromHex($"#cdff9e");
        else _background.color = ColorHelper.FromHex($"#b5cbdd");
    }
}
