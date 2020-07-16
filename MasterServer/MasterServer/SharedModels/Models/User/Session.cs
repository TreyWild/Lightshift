using System;
using System.Collections.Generic;
using System.Text;

namespace SharedModels.Models.User
{
    [Serializable]
    public class Session
    {
        public string Id;
        public string userId;
        public TimeSpan sessionDuration;


        public DateTime loginDate;
        public DateTime logoffDate;
    }
}
