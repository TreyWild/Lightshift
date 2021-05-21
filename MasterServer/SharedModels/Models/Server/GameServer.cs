using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Models.Server
{
    public class GameServer
    {
        public string Id { get; set; }
        public string IPAddress { get; set; }
        public string Port { get; set; }
        public string MaxPlayers { get; set; }
        public string SecretAuthenticationKey { get; set; }
    }
}
