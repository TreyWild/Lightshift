using Assets._Lightshift.Scripts.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    None,
    Scrap,
    Engine,
    Wing,
    Armor,
    Weapon,
    Generator,
    LightLance,
    Shield,
    MiningDrill,
    Hull,
    Ship
}

[CreateAssetMenu(fileName = "Module", menuName = "Lightshift/Create Module", order = 2)]

public class ModuleItem : ScriptableObject
{
    public string Id;
    public string DisplayName;
    public string Lore;
    public int MaxUpgrades = 20;
    public ItemType Type;
    public Sprite Sprite;
    public List<GameModifier> Stats;
    public List<UpgradeInfo> Upgrades;
}

