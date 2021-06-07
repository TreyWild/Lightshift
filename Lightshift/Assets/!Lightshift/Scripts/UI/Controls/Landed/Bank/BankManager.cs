using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;
using TMPro;
using System;
using System.Linq;
using Mirror;
using SharedModels.Models.Game;

public class BankManager : LandedState
{
    [SerializeField] private GameObject _bankDialog;

    [Header("Resources")]
    [SerializeField] private Transform _resourceTransform;
    [SerializeField] private GameObject _resourceItemControlPrefab;

    [Header("Content")]
    [SerializeField] private Transform _contentTransform;
    [SerializeField] private GameObject _bankItemPrefab;

    private void Start()
    {
        if (player != null)
        {
            player.Resources.Callback += OnResourceChanged;
            player.BankResources.Callback += OnBankChanged;
            LoadResources();
            LoadBankResources();
        }
    }

    private void OnResourceChanged(SyncIDictionary<ResourceType, int>.Operation op, ResourceType key, int item)
    {
        LoadResources();
    }
    private void OnBankChanged(SyncIDictionary<ResourceType, int>.Operation op, ResourceType key, int item)
    {
        LoadBankResources();
    }

    private List<ResourceItemControl> _resourceList = new List<ResourceItemControl>();
    private void LoadResources()
    {
        if (_resourceList == null)
            _resourceList = new List<ResourceItemControl>();

        foreach (var resource in _resourceList)
        {
            if (resource == null)
                continue;
            if (resource.gameObject != null)
            {
                Destroy(resource.gameObject);
            }
        }

        _resourceList.Clear();
        _resourceList = new List<ResourceItemControl>();

        var resources = player.GetResources();
        foreach (var resource in resources)
        {
            var obj = Instantiate(_resourceItemControlPrefab, _resourceTransform);
            var script = obj.GetComponent<ResourceItemControl>();
            script.Init(resource.Type, resource.Amount);
            _resourceList.Add(script);
        }
    }

    private List<BankItem> _bankResourceList = new List<BankItem>();
    private void LoadBankResources()
    {
        if (_bankResourceList == null)
            _bankResourceList = new List<BankItem>();

        foreach (var resource in _bankResourceList)
        {
            if (resource == null)
                continue;
            if (resource.gameObject != null)
            {
                Destroy(resource.gameObject);
            }
        }

        _bankResourceList.Clear();
        _bankResourceList = new List<BankItem>();

        //Ensure bank can see any playerr resources for deposit/withdrawal
        var resources = player.GetBankResources().Where(b => b.Amount > 0).ToList();
            foreach (var resource in player.GetResources().Where(a => a.Amount > 0))
            if (!resources.Any(a => a.Type == resource.Type))
                    resources.Add(new ResourceObject { Type = resource.Type, Amount = 0 });

        foreach (var resource in resources)
        {
            var resourceObject = ItemService.GetResourceItem(resource.Type);
            if (resourceObject != null) {
                var obj = Instantiate(_bankItemPrefab, _contentTransform);
                var script = obj.GetComponent<BankItem>();
                script.SetBalance(resource.Amount);
                script.SetDisplay(resourceObject.Sprite);
                script.SetLabel(resourceObject.DisplayName);

                script.onWithdraw += (s) => 
                {
                    ShowBankDialog(resource.Type,BankAction.Withdraw);
                };
                script.onDeposit += (s) => 
                {
                    ShowBankDialog(resource.Type, BankAction.Deposit);
                };

                _bankResourceList.Add(script);
            }
        }
    }

    private GameObject _dialogObject;

    private void ShowBankDialog(ResourceType type, BankAction action) 
    {
        var balance = player.GetBankResource(type);
        var cargo = player.GetResource(type);

        var dialog = Instantiate(_bankDialog, transform);
        var control = dialog.GetComponent<BankDialog>();
        control.Init(cargo.Amount, balance.Amount, type, action);

        control.onConfirm += (balance) =>
        {
            player.BankTransaction(action, type, balance);
        };

        _dialogObject = dialog.gameObject;
    }


    public void LeaveBank()
    {
        player.TakeOff();
        if (_dialogObject != null)
            Destroy(_dialogObject);
    }

    public void DepositAll() 
    {
        DialogManager.ShowDialog($"Are you sure you want to Deposit all of your Cargo?", delegate (bool result) 
        {
            if (result)
            {
                player.DepositAllCargo();
            }
        });
    }

    public void WithdrawAll() 
    {
        DialogManager.ShowDialog($"Are you sure you want to Withdraw all of your Cargo?", delegate (bool result)
        {
            if (result)
            {
                player.WithdrawAllCargo();
            }
        });
    }
}
