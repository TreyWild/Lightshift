using System;
using System.Collections.Generic;
using System.Text;

namespace SharedModels.Models.User
{
    [Serializable]
    public class Account
    {
        public string Id { get; set; }
        public string password { get; set; }
        public string emailAddress { get; set; }
        public string username { get; set; }

        public DateTime creationDate { get; set; }

        public DateTime lastActivity { get; set; }
    }
}
