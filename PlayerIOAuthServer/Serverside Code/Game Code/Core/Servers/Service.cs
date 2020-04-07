using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using PlayerIO.GameLibrary;
using System.Drawing;
using System.Linq;

namespace ServersideGameCode {

	[RoomType("Service")]
	public class Service : Game<Player> {

		public override void GameStarted()
		{
			//This will load a users player object as they are connecting. Very useful for saving player information.
			PreloadPlayerObjects = true;

			//Add Message Handlers
			AddMessageHandler("username", OnUsernameMessage);
            AddMessageHandler("init", OnInitMessage);

		}

        private void OnInitMessage(Player player, Message message)
        {
            //Ensure user has a username and that their existing username complies with username specifications.
            if (!Util.IsAllowedUsername(player.Username))
            {
                //Username undefined or not allowed. Force them to set a new one.
                player.Username = "";
                player.Send("username", "set");
            }

            //User has valid username. Initialize them.
            else player.Send("init", player.GetSpecialAuthKey());
        }

        private void OnUsernameMessage(Player player, Message message)
		{
			switch (message.GetString(0))
			{
                //Change username
                case "set":
                    string username = message.GetString(1);

                    //Ensure username is available
                    PlayerIO.BigDB.LoadSingle("PlayerObjects", "byName", new object[1] { username.ToUpperInvariant() }, delegate (DatabaseObject o)
                    {
                        if (o != null && o.ExistsInDatabase)
                        {
                            //Username is taken.
                            player.Send("username", "taken");
                            return;
                        }

                        if (username.Length >= 20)
                        {
                            //Username is too long.
                            player.Send("username", "long");
                            return;
                        }

                        if (username.Length <= 3)
                        {
                            //Username is too short.
                            player.Send("username", "short");
                            return;
                        }

                        //Ensure user is allowed to change it's name.
                        if (player.Username == "" || player.Username == null || player.CanChangeUsername)
                        {
                            //Assign new username
                            player.Username = username;
                            player.Send("username", "success");
                            return;
                        }
                    });
                    break;
            }
		}

        //public override void GotMessage(Player player, Message message)
        //{
        //    Console.WriteLine(message);
        //    base.GotMessage(player, message);
        //}
    }
}
