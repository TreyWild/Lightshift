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
    public struct Upgrade
    {
        public string Id;
        public int Level;

        public static bool operator ==(Upgrade obj, Upgrade obj2)
        {
            return obj.Equals(obj2);
        }
        public static bool operator !=(Upgrade obj, Upgrade obj2)
        {
            return !obj.Equals(obj2);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
