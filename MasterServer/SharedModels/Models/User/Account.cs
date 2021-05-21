using SharedModels.Models.Game;
using SharedModels.Models.User;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SharedModels
{
    public class Account
    {
        public string Id { get; set; }
        public bool AccountConfirmed { get; set; }
        public string EmailAddress { get; set; }
        public string CaseSensitiveUsername { get; set; }
        public Profile Profile { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastActivity { get; set; }
        public int TotalSessions { get; set; }
    }
}
