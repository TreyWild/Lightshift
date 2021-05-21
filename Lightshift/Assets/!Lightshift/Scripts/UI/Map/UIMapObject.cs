using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIMapObject : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameLabel;
    [SerializeField] private TextMeshProUGUI _loreLabel;
    [SerializeField] private Image _iconHolder;

    public void Init(MapObject mapObject) 
    {
        _nameLabel.text = mapObject.Name;
        _nameLabel.color = mapObject.nameColor;

        _loreLabel.text = mapObject.Lore;
        _iconHolder.sprite = mapObject.Icon;
        _iconHolder.color = mapObject.iconColor;
    }
}

