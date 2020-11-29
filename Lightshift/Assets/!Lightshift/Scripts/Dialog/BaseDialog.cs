using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BaseDialog : MonoBehaviour {

    [SerializeField] TextMeshProUGUI displayText;

    public void SetDisplay(string message)
    {
        displayText.text = message;
    }

    [SerializeField] TextMeshProUGUI _buttonText;
    public void SetButtonText(string text) => _buttonText.text = text;
}
