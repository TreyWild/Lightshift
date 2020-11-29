using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Models
{
    public class UserCredentials
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ValidationToken { get; set; }
        public string HashedPassword { get; set; }
        public DateTime LastPasswordUpdate { get; set; }
    }
}
