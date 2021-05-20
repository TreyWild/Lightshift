using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ColorDialog : BaseDialog
{

    public Action<Color> OnConfirm;
    [SerializeField] private FlexibleColorPicker _colorPicker;

    public void SetStartColor(Color color) 
    {
        _colorPicker.startingColor = color;
    }
    public void Confirm()
    {
        OnConfirm?.Invoke(_colorPicker.color);
        Destroy(gameObject);
    }
}

