using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ShipInterface : MonoBehaviour
{
    [SerializeField] private ProgressBar _healthBar;
    [SerializeField] private ProgressBar _shieldBar;
    [SerializeField] private ProgressBar _powerBar;
    [SerializeField] private Image _panicIndicator;

    public void SetHealth(float value, float maxValue) 
    {
        float percentile = (1 - value / (maxValue / 4)) * .2f;

        _panicIndicator.SetTransparency(percentile);

        _healthBar.SetProgress(value, maxValue);
    }
    public void SetShield(float value, float maxValue)
    {
        _shieldBar.SetProgress(value, maxValue);
    }

    public void SetPower(float value, float maxValue)
    {
        _powerBar.SetProgress(value, maxValue);
    }

}
