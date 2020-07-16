using Raven.Client.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterServer
{
    public static class Database
    {
        public static AccountContext Accounts = new AccountContext("Accounts");
        public static SessionContext Sessions = new SessionContext("Sessions");
    }

    public class DatabaseContext
    {
        public DocumentStore documentStore;
        private string _certificate = @"C:\Users\bryly\Documents\GitHub\Lightshift\MasterServer\MasterServer\MasterServer\bin\Debug\netcoreapp3.1\certificate.pfx";
        private string _databaseName = "Lightshift";
        public DatabaseContext(string table)
        {
            documentStore = new DocumentStore
            {
                Urls = new[]
{
                    "https://a.free.bryly.ravendb.cloud"
                },
                Database = _databaseName,
                Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(_certificate),
                Identifier = table
            };
            documentStore.Initialize();
        }
    }
}
