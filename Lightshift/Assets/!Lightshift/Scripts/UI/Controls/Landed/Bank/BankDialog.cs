using Michsky.UI.ModernUIPack;
using SharedModels.Models.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum BankAction 
{
    Deposit, Withdraw
}
public class BankDialog : MonoBehaviour
{
    [SerializeField] private ItemGraphicDisplay _icon;
    [SerializeField] private TextMeshProUGUI _displayName;
    [SerializeField] private TextMeshProUGUI _bankBalance;
    [SerializeField] private TextMeshProUGUI _cargoBalance;
    [SerializeField] private TMP_InputField _inputBalance;
    [SerializeField] private ButtonManagerBasic _buttonConfirm;
    [SerializeField] private ButtonManagerBasic _buttonCancel;

    public Action<int> onConfirm;

    public ResourceType type;

    private BankAction _action;

    private int bank, wallet;
    public void Init(int wallet, int bank, ResourceType type, BankAction action)
    {
        this.bank = bank;
        this.wallet = wallet;
        _action = action;

        if (action == BankAction.Deposit)
            _inputBalance.text = $"{wallet}";
        else if (action == BankAction.Withdraw)
            _inputBalance.text = $"{bank}";

        _bankBalance.text = $"{bank:D}";
        _cargoBalance.text = $"{wallet:D}";

        _buttonConfirm.buttonText = action.ToString();;
        _buttonConfirm.UpdateUI();

        var resource = ItemService.GetResourceItem(type);
        if (resource != null) 
        {
            _displayName.text = resource.Type.ToString().ToSentence();
            _icon.InitializeGraphic(resource.Sprite);
        }

        _inputBalance.onValueChanged.AddListener(delegate
        {
            try
            {
                var credits = int.Parse(_cargoBalance.text);

                switch (_action)
                {
                    case BankAction.Deposit:
                        if (credits > wallet)
                        {
                            _cargoBalance.text = $"{wallet}";
                            return;
                        }
                        break;
                    case BankAction.Withdraw:
                        if (credits > bank)
                        {
                            _cargoBalance.text = $"{bank}";
                            return;
                        }
                        break;
                }
            }
            catch
            {

            }
        });
    }

    public void Confirm()
    {
        try
        {
            var amount = int.Parse(_inputBalance.text);

            switch (_action)
            {
                case BankAction.Deposit:
                    if (amount > wallet)
                    {
                        DialogManager.ShowMessage($"You do not have enough {type}!"); 
                        return;
                    }
                    break;
                case BankAction.Withdraw:
                    if (amount > bank)
                    {
                        DialogManager.ShowMessage($"You do not have enough {type}!");
                        return;
                    }
                    break;
            }
            onConfirm?.Invoke(amount);
            Destroy(gameObject);
        }
        catch
        {
            DialogManager.ShowMessage("Please enter a valid number of credits!");
            if (_action == BankAction.Deposit)
                _inputBalance.text = $"{wallet}";
            else if (_action == BankAction.Withdraw)
                _inputBalance.text = $"{bank}";
        }
    }

    public void Cancel() 
    {
        Destroy(gameObject);
    }
}
