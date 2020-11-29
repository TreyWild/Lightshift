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
        public int Level;
        public int XP;
        public int TotalSessions;
        public string LastCheckPointId;
        public string LandedLocationId;
        public bool IsLanded;
        public DateTime CreationDate;
        public DateTime LastActivity;
        public string ActiveShip;

    }
}
