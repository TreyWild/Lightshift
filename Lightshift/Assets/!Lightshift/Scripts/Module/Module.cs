using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum ModuleType 
{
    Lightlance,
    Engine,
    Hull,
    Wing,
    Weapon,
}

[Serializable]
public class Module 
{
    public string key;
    public string displayName;
    public ModuleType type;
    public ModuleData data;

    public Image Icon;
    public Sprite Sprite;

    public Color color;
}