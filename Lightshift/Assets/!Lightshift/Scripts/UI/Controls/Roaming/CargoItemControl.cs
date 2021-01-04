using SharedModels.Models.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CargoItemControl : MonoBehaviour
{
    [SerializeField] Image _sprite;
    [SerializeField] TextMeshProUGUI _nameLabel;
    [SerializeField] TextMeshProUGUI _amountLabel;

    public CargoType type;
    public int amount;

    public Action<CargoItemControl> onEject;
    public void Eject() 
    {
        onEject?.Invoke(this);
    }

    public void Init(CargoType type, int amount) 
    {
        this.type = type;
        _nameLabel.text = $"{type}";
        this.amount = amount;

        _amountLabel.text = $"{amount}";

        var item = ItemService.GetCargoItem(type);
        if (item != null)
            _sprite.sprite = item.Sprite;
        else Destroy(gameObject);
    }
}
