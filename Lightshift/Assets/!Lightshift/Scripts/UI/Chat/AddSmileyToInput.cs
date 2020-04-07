using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AddSmileyToInput : MonoBehaviour {

    public TMP_InputField input;
    public TextMeshProUGUI smiley;

    public void OnSmileyClick()
    {
        if (input.text.Length < 200)
        input.text = input.text + $" {smiley.text}";

        input.ActivateInputField();
    }
}
