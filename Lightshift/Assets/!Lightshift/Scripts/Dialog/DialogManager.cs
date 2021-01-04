using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharedModels.Models.Game;
public class DialogManager : MonoBehaviour
{
    [Header("Dialogs")]
    [SerializeField] GameObject _messageBox;
    [SerializeField] GameObject _dialogBox;
    [SerializeField] GameObject _dialogEjectBox;
    [SerializeField] GameObject _colorDialog;
    [SerializeField] GameObject _listView;
    [SerializeField] GameObject _upgradeView;

    [Header("Controls")]
    [SerializeField] GameObject _itemViewControl;
    [SerializeField] GameObject _itemViewStatControl;
    [SerializeField] GameObject _itemViewShipControl;
    [SerializeField] GameObject _itemViewModuleControl;
    public static DialogManager Instance { get; set; }

    public void Awake()
    {
        if (Instance != null)
            Destroy(this);
        else
            Instance = this;
    }
    public static void ShowDialog(string message, Action<bool> callback = null, string buttonText = null, string button2Text = null)
    {
        var obj = Instantiate(Instance._dialogBox);
        var dialog = obj.GetComponent<ConfirmDialog>();
        dialog.SetDisplay(message);

        dialog.OnClick += (result) => callback?.Invoke(result);
    }


    public static void ShowEjectDialog(string message, int amount, Action<bool, int> callback = null, string buttonText = null, string button2Text = null)
    {
        var obj = Instantiate(Instance._dialogEjectBox);
        var dialog = obj.GetComponent<ConfirmEjectDialog>();
        dialog.MaxValue = amount;
        dialog.SetDisplay(message);

        dialog.OnClick += (result, amt) => callback?.Invoke(result, amt);
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

    public static ListView CreateListView(string title) 
    {
        var obj = Instantiate(Instance._listView);
        var listView = obj.GetComponent<ListView>();
        listView.SetTitle(title);
        return listView;
    }

    public static UpgradeView CreateUpgradeView(Item item = null)
    {
        var obj = Instantiate(Instance._upgradeView);
        var view = obj.GetComponent<UpgradeView>();
        if (item != null)
            view.InitializeUpgrades(item);
        return view;
    }

    public static GameObject GetDefaultItemViewControl() => Instance._itemViewControl;
    public static GameObject GetItemViewShipControl() => Instance._itemViewShipControl;
    public static GameObject GetItemViewModuleControl() => Instance._itemViewModuleControl;
}
