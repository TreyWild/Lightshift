using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Models.Game
{
    public enum Modifier
    {
        Health,
        Shield,
        Weight,
        Acceleration,
        Range,
        Speed,
        Regen,
        Agility,
        Armor,
        Refire,
    }

    [Serializable]
    public class Upgrade
    {
        public Modifier Type { get; set; }

        public int Level { get; set; }
    }
}
