using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Item", menuName = "Inventory/Create Item", order = 1)]

public class Item : ScriptableObject
{
    public string key = Guid.NewGuid().ToString();
    public string displayName;
    public ItemType type;
    public ModuleData data;

    public Sprite Icon;
    public Sprite Sprite;

    public Color color = Color.white;

    public int maxStack = 999;
}

public enum ItemType 
{
    Any,
    Engine,
    Wing,
    Hull,
    Weapon,
    Generator,
    LightLance,
    Shield,
    MiningDrill,
}