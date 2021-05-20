using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class InputDialog : BaseDialog
{
    [SerializeField] private TMP_InputField _inputField;

    public Action<string> OnClick;

    public void SetInput(string input) => _inputField.text = input;
    public void Confirm()
    {
        OnClick?.Invoke(_inputField.text);
        Destroy(gameObject);
    }
}
