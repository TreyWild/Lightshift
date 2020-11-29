using SharedModels.Models;
using SharedModels.Models.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterServer.Database.Context
{
    public class CredentialsDBContext : DatabaseContext
    { 
        public UserCredentials GetUserCredentials(string userId) 
        {
            return documentStore.OpenSession().Query<UserCredentials>().FirstOrDefault(u => u.UserId == userId);
        }

        public void CreateUserCredentials(UserCredentials credentials) 
        {
            credentials.LastPasswordUpdate = DateTime.Now;
            credentials.Id = Guid.NewGuid().ToString();
            SaveDocument(credentials);
        }

        public void UpdateUserCredentials(UserCredentials credentials)
        {
            credentials.LastPasswordUpdate = DateTime.Now;
            SaveDocument(credentials);
        }

        public GameServerCredentials GetGameServerCredentials(string secretKey) 
        {
            return NewSession().Query<GameServerCredentials>().FirstOrDefault(g => g.SecretKey == secretKey);
        }

        public void CreateGameServerCredentials(string secretKey)
        {
            var credentials = new GameServerCredentials
            {
                Id = Guid.NewGuid().ToString(),
                SecretKey = secretKey
            };
            SaveDocument(credentials);
        }
    }
}
