using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ConfirmEjectDialog : BaseDialog
{
    private TMP_InputField _inputField;

    public Action<bool, int> OnClick;

    [SerializeField] TextMeshProUGUI _button2Text;
    public void SetButton2Text(string text) => _button2Text.text = text;

    public void Confirm()
    {
        OnClick?.Invoke(true, GetValue()) ;
        Destroy(gameObject);
    }

    public void Cancel()
    {
        OnClick?.Invoke(false, GetValue());
        Destroy(gameObject);
    }

    public int MaxValue;
    private int _currentValue;

    public int GetValue() => _currentValue;

    public void Start()
    {
        _inputField.onValueChanged.AddListener(TextChanged);
        _inputField.text = MaxValue.ToString();
    }

    private void TextChanged(string text)
    {
        try
        {
            if (_inputField.text != "")
                _currentValue = int.Parse(text);

            if (_currentValue > MaxValue)
                _currentValue = MaxValue;
            else if (_currentValue < 0)
                _currentValue = 0;
        }
        catch 
        {
            _inputField.text = MaxValue.ToString();
        };
    }
}
