using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Models.Game
{
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
    }

    public enum ModuleLocation
    {
        Hull,
        PrimaryWings,
        SecondaryWings,
        Engine,
        Weapon1,
        Weapon2,
        Weapon3,
        Weapon4,
        Weapon5,
        Utility1,
        Utility2
    }
}
