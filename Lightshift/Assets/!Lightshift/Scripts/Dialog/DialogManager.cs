using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{

    [SerializeField] GameObject _messageBox;
    [SerializeField] GameObject _dialogBox;
    [SerializeField] GameObject _colorDialog;

    public static DialogManager Instance { get; set; }

    public void Awake()
    {
        if (Instance != null)
            Destroy(this);
        else
            Instance = this;
    }
    public static void ShowDialog(string message, Action<bool> callback = null)
    {
        var obj = Instantiate(Instance._dialogBox);
        var dialog = obj.GetComponent<ConfirmDialog>();
        dialog.SetDisplay(message);

        dialog.OnClick += (result) => callback?.Invoke(result);
    }

    public static void ShowColorDialog(Action<Color> callback = null)
    {
        var obj = Instantiate(Instance._colorDialog);
        var dialog = obj.GetComponent<ColorDialog>();
        dialog.OnConfirm += (color) => callback?.Invoke(color);
    }

    public static void ShowMessage(string message)
    {
        var obj = Instantiate(Instance._messageBox);
        var messageBox = obj.GetComponent<MessageBox>();
        messageBox.SetDisplay(message);
    }
}
