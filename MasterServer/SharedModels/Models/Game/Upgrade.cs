using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Models.Game
{
    public enum Modifier
    {
        MaxHealth,
        Health,
        Shield,
        MaxShield,
        Weight,
        Acceleration,
        Range,
        Speed,
        Regen,
        Agility,
        Armor,
        Refire,
        Power,
        MaxPower,
        BrakeForce,
        OverdriveBoost,
        CargoCapacity,
    }

    [Serializable]
    public class Upgrade
    {
        public string Id { get; set; }
        public int Level { get; set; }
    }
}
