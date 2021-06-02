using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatBarSlider : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private Slider _slider;

    public void SetLabel(string label) => _label.text = label;

    public void SetProgress(float current, float max, bool customLabel = false)
    {
        if (current <= 0)
            _slider.value = 0;
        var value = (current / max) * 1.0f;
        _slider.value = value;

        if (customLabel)
            return;

        _label.text = $"{Mathf.Round(current)} | {Mathf.Round(max)}";
    }
}
