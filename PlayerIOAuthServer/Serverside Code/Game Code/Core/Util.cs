using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersideGameCode
{
    public class Util
    {
        public static bool IsAllowedUsername(string username) 
        {
            if (username == null || username.Length > 20 || username.Length < 3 || username == "")
                return false;
            return true;
        }
    }
}
