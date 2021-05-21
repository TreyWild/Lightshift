using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeaderControl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _value;

    public void SetDisplay(string title, object value)
    {
        _title.text = title;
        _value.text = value.ToString();
    }
}