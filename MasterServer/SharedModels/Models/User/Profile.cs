using SharedModels.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Models.User
{
    [Serializable]
    public class Profile
    {
        public string Username { get; set; }
        public int Credits { get; set; }
        public int BankCredits { get; set; }
        public int Level { get; set; }
        public int XP { get; set; }
        public string LastCheckPointId { get; set; }
        public string LandedLocationId { get; set; }
        public bool IsLanded { get; set; }
        public string ActiveShip { get; set; }

    }
}
