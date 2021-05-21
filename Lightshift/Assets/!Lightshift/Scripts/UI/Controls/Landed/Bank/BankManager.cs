using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;
using TMPro;
using System;
using System.Linq;

public class BankManager : MonoBehaviour
{
    [SerializeField] private GameObject _bankDialog;
    [SerializeField] private TextMeshProUGUI _bankBalance;
    [SerializeField] private TextMeshProUGUI _walletBalance;

    private Player _player;
    private void Start()
    {
        _player = FindObjectsOfType<Player>().FirstOrDefault(p => p.isLocalPlayer);

        _player.onBankCreditsChanged += (credits) =>
        {
            RefreshBalances();
        };

        _player.onCreditsChanged += (credits) =>
        {
            RefreshBalances();
        };

        RefreshBalances();
    }

    public void LeaveBank()
    {
        _player.TakeOff();
    }

    public void RefreshBalances() 
    {
        _bankBalance.text = _player.BankCredits.ToString("D");
        _walletBalance.text = _player.Credits.ToString("D");
    }

    public void Deposit() 
    {
        var dialog = Instantiate(_bankDialog);
        var control = dialog.GetComponent<BankDialog>();
        control.Init(_player.Credits, _player.BankCredits, BankAction.Deposit);

        control.onConfirm += (value) =>
        {
            _player.BankTransaction(BankAction.Deposit, value);
        };
    }

    public void Withdraw() 
    {
        var dialog = Instantiate(_bankDialog);
        var control = dialog.GetComponent<BankDialog>();
        control.Init(_player.Credits, _player.BankCredits, BankAction.Withdraw);

        control.onConfirm += (value) =>
        {
            _player.BankTransaction(BankAction.Withdraw, value);
        };
    }
}
