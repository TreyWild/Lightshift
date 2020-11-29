using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConfirmDialog : BaseDialog {

    public Action<bool> OnClick;

    [SerializeField] TextMeshProUGUI _button2Text;
    public void SetButton2Text(string text) => _button2Text.text = text;

    public void Confirm()
    {
        OnClick?.Invoke(true);
        Destroy(gameObject);
    }

    public void Cancel()
    {
        OnClick?.Invoke(false);
        Destroy(gameObject);
    }
}
