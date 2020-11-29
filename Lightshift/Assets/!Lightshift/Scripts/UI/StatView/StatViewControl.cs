using Assets._Lightshift.Scripts.Utilities;
using SharedModels.Models.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatViewControl : MonoBehaviour
{
    private GameModifier _modifier;

    [SerializeField] TextMeshProUGUI _display;
    [SerializeField] TextMeshProUGUI _value;
    [SerializeField] Image _image;
    public GameModifier GetModifier() => _modifier;
    public void Setup(GameModifier modifier) 
    {
        _modifier = modifier;

        _display.text = $"{modifier.Type}";
        _value.text = $"{Math.Round(modifier.Value, 2)}";

        _image.color = ColorHelper.FromModifier(modifier.Type);
    }
}
