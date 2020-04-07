using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ProgressBar 
{
    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private Slider _slider;

    public void SetLabel(string label) => _label.text = label;

    public void SetProgress(float current, float max) 
    {
        var value = (current / max) * 1.0f;
        _slider.value = value;
        _label.text = $"{Mathf.Round(current)} | {Mathf.Round(max)}";
    }
}
