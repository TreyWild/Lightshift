using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageBox : BaseDialog {

    public Action onConfirm;
    public void Confirm()
    {
        onConfirm?.Invoke();
        onConfirm = null;
        Destroy(gameObject);
    }
}
