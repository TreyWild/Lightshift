using Assets._Lightshift.Scripts.Data;
using SharedModels.Models.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "Module", menuName = "Lightshift/Create Module", order = 2)]

public class ModuleItem : ScriptableObject
{
    [Header("Misc")]
    public string Id;
    public string DisplayName;
    public string Lore;
    public int MaxUpgrades = 20;
    public ItemType Type;
    public Sprite Sprite;
    public Color DisplayColor = Color.white;


    [Header("Game Data")]
    public List<ModuleLocation> InstallLocations;
    public List<GameModifier> Stats;
    public List<UpgradeInfo> Upgrades;

    [Header("ShopInfo")]
    public int creditsCost;
    public List<ResourceObject> resourceCost;
}