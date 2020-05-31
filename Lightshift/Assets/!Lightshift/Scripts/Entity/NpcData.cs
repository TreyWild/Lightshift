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
[CreateAssetMenu(fileName = "Npc", menuName = "Lightshift/Create Npc", order = 4)]
public class NpcData : ScriptableObject
{
    public string key = Guid.NewGuid().ToString();
    public string Name;
    public NpcType type;
    public NpcFaction faction;

    [Header("Npc Behaviors")]
    public List<UnityEngine.Object> Behavior;

    public Sprite mapIcon;
    public float trackingRange;
    public ModuleData data;
    public Vector2 scale = new Vector2(1,1);
    public Sprite sprite;
    public int renderOrder = 45;
    [Header("Add Wings (Optional)")]
    public Sprite wingImage;
    [Header("Override Weapons (Optional)")]
    public Weapon[] weapons;
    [Header("Override Prefab (Optional)")]
    public GameObject prefab;
}
public abstract class GenericCollection<T> : ScriptableObject where T : UnityEngine.Object 
{
    public List<T> Items = new List<T>();
}

[Serializable]
public class NpcDataShell 
{
    public NpcData data;
}