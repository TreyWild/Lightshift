using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class UpgradeManager : LandedState
{
    [SerializeField] ItemView _itemView;


    private void Start()
    {
        _itemView.onClick += (item) =>
        {
            var upgradeView = DialogManager.CreateUpgradeView(item);
        };

        _itemView.Init(player.GetItems(), $"Upgrades");

    }
}

