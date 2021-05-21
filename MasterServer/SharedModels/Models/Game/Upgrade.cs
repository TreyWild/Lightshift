using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Models.Game
{
    public enum Modifier
    {
        None,
        MaxHealth,
        Health,
        Shield,
        MaxShield,
        Weight,
        Accel,
        Range,
        Speed,
        Regen,
        Agility,
        Armor,
        Refire,
        Power,
        MaxPower,
        Brakes,
        Overdrive,
        Storage,
    }

    [Serializable]
    public class Upgrade
    {
        public string Id { get; set; }
        public int Level { get; set; }
    }
}
