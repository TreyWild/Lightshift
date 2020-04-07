using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmDialog : BaseDialog {

    public Action<bool> OnClick;

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
