using Michsky.UI.ModernUIPack;
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
    [SerializeField] private TextMeshProUGUI _bankBalance;
    [SerializeField] private TextMeshProUGUI _walletBalance;
    [SerializeField] private TMP_InputField _inputCredits;
    [SerializeField] private ButtonManagerBasic _buttonConfirm;
    [SerializeField] private ButtonManagerBasic _buttonCancel;

    public Action<int> onConfirm;

    private BankAction _action;

    private int bank, wallet;
    public void Init(int wallet, int bank, BankAction action)
    {
        this.bank = bank;
        this.wallet = wallet;
        _action = action;

        _inputCredits.text = $"1000";
        _bankBalance.text = $"{bank:D}";
        _walletBalance.text = $"{wallet:D}";

        _buttonConfirm.buttonText = action.ToString();;
        _buttonConfirm.UpdateUI();

        _inputCredits.onValueChanged.AddListener(delegate
        {
            try
            {
                var credits = int.Parse(_inputCredits.text);

                switch (_action)
                {
                    case BankAction.Deposit:
                        if (credits > wallet)
                        {
                            _inputCredits.text = $"{wallet}";
                            return;
                        }
                        break;
                    case BankAction.Withdraw:
                        if (credits > bank)
                        {
                            _inputCredits.text = $"{bank}";
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
            var credits = int.Parse(_inputCredits.text);

            switch (_action)
            {
                case BankAction.Deposit:
                    if (credits > wallet)
                    {
                        DialogManager.ShowMessage("You do not have enough credits!");
                        return;
                    }
                    break;
                case BankAction.Withdraw:
                    if (credits > bank)
                    {
                        DialogManager.ShowMessage("You do not have enough credits!");
                        return;
                    }
                    break;
            }
            onConfirm?.Invoke(credits);
            Destroy(gameObject);
        }
        catch
        {
            DialogManager.ShowMessage("Please enter a valid number of credits!");
            _inputCredits.text = $"1000";
        }
    }

    public void Cancel() 
    {
        Destroy(gameObject);
    }
}
