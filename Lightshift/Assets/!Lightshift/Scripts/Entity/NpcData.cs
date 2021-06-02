using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
public enum NpcFaction 
{
    Bacon,
    Mufenz
}

public enum NpcType 
{
    Ship,
    Entity
}

[Serializable]
public struct NpcSettings
{
    public float TrackingRange;
    public string Name;
    public NpcType Type;
    public NpcFaction Faction;
    public List<Weapon> Weapons;
    public List<GameModifier> Stats;
}

public abstract class GenericCollection<T> : ScriptableObject where T : UnityEngine.Object 
{
    public List<T> Items = new List<T>();
}
