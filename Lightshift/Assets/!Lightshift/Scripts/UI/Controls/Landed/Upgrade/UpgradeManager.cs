using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class UpgradeManager : MonoBehaviour
{
    [SerializeField] ItemView _itemView;

    private Player _player;
    

    private void Start()
    {
        _player = FindObjectsOfType<Player>().FirstOrDefault(p => p.isLocalPlayer);

        _itemView.onClick += (item) =>
        {
            var upgradeView = DialogManager.CreateUpgradeView(item);
        };

        _itemView.Init(_player.GetItems(), $"Upgrades");

    }

    public void LeaveStation()
    {
        _player.TakeOff();
    }
}

