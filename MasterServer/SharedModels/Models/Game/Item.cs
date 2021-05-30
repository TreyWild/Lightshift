using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Models.Game
{
    [Serializable]
    public struct Item
    {
        public string UserId;
        public string Id;
        public string ModuleId;

        public string Color;
        public Upgrade[] Upgrades;
        public ResourceObject[] SpentResources;
        public int MaxUpgrades;

        public static bool operator ==(Item obj, Item obj2)
        {
            return obj.Equals(obj2);
        }
        public static bool operator !=(Item obj, Item obj2)
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

        public static Item Empty() => new Item();
    }
}
