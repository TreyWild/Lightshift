using SharedModels.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Models.User
{
    [Serializable]
    public struct Profile
    {
        public string Username;
        public int Credits;
        public int BankCredits;
        public int Level;
        public int XP;
        public string LastCheckPointId;
        public string LandedLocationId;
        public bool IsLanded;
        public string ActiveLoadout;

        public List<ResourceObject> Resources;

        public List<ResourceObject> Bank;

        public static bool operator ==(Profile obj, Profile obj2)
        {
            return obj.Equals(obj2);
        }
        public static bool operator !=(Profile obj, Profile obj2)
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
