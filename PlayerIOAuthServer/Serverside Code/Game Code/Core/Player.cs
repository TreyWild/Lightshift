using PlayerIO.GameLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersideGameCode
{
    public class Player : BasePlayer
    {
        public string GetSpecialAuthKey() 
        {
            string key = Guid.NewGuid().ToString();
            PlayerObject.Set("authKey", key);
            PlayerObject.Save();
            return key;
        }
        public string Username
        {
            //Get username from database object
            get => PlayerObject.GetString("username", "");
            set
            {
                //Save new username to database object
                PlayerObject.Set("username", value);

                //We'll use this uppercase varient when checking against existing usernames.
                PlayerObject.Set("accountInfo.uppercaseUsername", value.ToUpperInvariant());
                PlayerObject.Save();
            }
        }

        public bool CanChangeUsername
        {
            //Is this user allowed to change their username?
            get => PlayerObject.GetBool("accountInfo.canChangeUsername", false);
            set
            {
                //Save new information
                PlayerObject.Set("accountInfo.canChangeUsername", value);
                PlayerObject.Save();
            }
        }
    }
}
