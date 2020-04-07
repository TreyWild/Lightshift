using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DividerItem : MonoBehaviour
{
    public TextMeshProUGUI label;

    public void SetDisplay(string display)
    {
        label.text = display;
    }
}
