using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NpcShip : Npc
{
    public new void Awake()
    {
        base.Awake();
        onDataLoaded += InitializeData;
    }

    private void InitializeData()
    {
        //if (hull != null)
        //    hull.SetImage(npcData.sprite, Color.white);
        //if (wing != null && npcData.wingImage != null)
        //    wing.SetImage(npcData.wingImage, Color.white);

        SetSortingOrder(npcData.renderOrder);
    }

    public void SetSortingOrder(int sortingOrder)
    {
        //if (hull != null)
        //    hull.SetSortingOrder(sortingOrder);

        //if (wing != null)
        //    wing.SetSortingOrder(sortingOrder);
    }
}