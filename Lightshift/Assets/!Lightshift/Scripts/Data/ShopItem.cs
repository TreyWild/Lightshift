using SharedModels.Models.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem", menuName = "Lightshift/Create Shop Item", order = 4)]

public class ShopItem : ScriptableObject
{
    public string Id = Guid.NewGuid().ToString();
    public string Lore;
    public ModuleItem module;
    public int creditsCost;
    public List<ResourceObject> resourceCost;
}
