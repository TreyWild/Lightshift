using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SharedModels.Models.Game;
using Assets._Lightshift.Scripts.Utilities;

public class ModuleStatControl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameLabel;
    [SerializeField] private TextMeshProUGUI _valueLabel;

    [SerializeField] private Image _background;
    [SerializeField] private Image _forground;

    public void Init(GameModifier resource, float potential) 
    {
        _background.color = ColorHelper.FromModifier(resource.Type);
        _forground.color = ColorHelper.FromModifier(resource.Type);

        _nameLabel.text = resource.Type.ToString().ToSentence();
        if (resource.Value != 0)
        {
            if (potential > 0)
                _valueLabel.text = $"{resource.Value} (+{potential} potential)";
            else if (potential < 0)
                _valueLabel.text = $"{resource.Value} ({potential} potential)";
            else _valueLabel.text = $"{resource.Value}";
        }
        else 
        {
            if (potential > 0)
                _valueLabel.text = $"+{potential} potential";
            else if (potential < 0)
                _valueLabel.text = $"{potential} potential";
        }
    }

}
