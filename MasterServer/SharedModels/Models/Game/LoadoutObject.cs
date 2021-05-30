using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Models.Game
{
    [Serializable]
    public struct LoadoutObject
    {
        public string Name;
        public string Id;
        public string UserId;
        public EquipObject[] EquippedModules;

        public static bool operator ==(LoadoutObject obj, LoadoutObject obj2)
        {
            return obj.Equals(obj2);
        }
        public static bool operator !=(LoadoutObject obj, LoadoutObject obj2)
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

        public static LoadoutObject Empty() => new LoadoutObject();
    }

    [Serializable]
    public struct EquipObject 
    {
        public ModuleLocation location;
        public string itemId;
    }
}
