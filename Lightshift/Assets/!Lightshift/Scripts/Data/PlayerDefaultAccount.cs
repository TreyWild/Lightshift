using SharedModels.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Default Player Data", menuName = "Lightshift/Create Default Player Data", order = 4)]

public class PlayerDefaultAccount : ScriptableObject
{
    public int Level;
    public int Xp;
    public int Credits;
    public List<ResourceObject> Resources;
    public List<ResourceObject> BankResources;
    public List<ModuleItem> Items;
    public LoadoutObject Loadout;
}
