using SharedModels.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BankItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private TextMeshProUGUI _balance;
    [SerializeField] private ItemGraphicDisplay _icon;

    public ResourceType type;

    public Action<BankItem> onWithdraw;
    public Action<BankItem> onDeposit;

    public void Withdraw() 
    {
        onWithdraw?.Invoke(this);
    }

    public void Deposit() 
    {
        onDeposit.Invoke(this);
    }

    public void SetBalance(float balance) 
    {
        _balance.text = balance.ToString();
    }

    public void SetLabel(string label) 
    {
        _label.text = label;
    }

    public void SetDisplay(Sprite sprite)
    {
        _icon.InitializeGraphic(sprite);
    }
}
