using System;
using System.Collections.Generic;
using System.Text;

namespace SharedModels
{
    [Serializable]
    public class Session
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public TimeSpan SessionDuration { get; set; }
        public DateTime LoginDate { get; set; }
        public DateTime LogoffDate { get; set; }
    }
}
