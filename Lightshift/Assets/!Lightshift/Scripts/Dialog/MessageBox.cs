using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageBox : BaseDialog {

    public void Confirm()
    {
        Destroy(gameObject);
    }
}
