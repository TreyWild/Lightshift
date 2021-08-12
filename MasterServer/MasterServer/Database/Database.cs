using MasterServer.Database.Context;
using Microsoft.AspNetCore.Http;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterServer
{
    public static class DB
    {
        public static AccountDBContext Accounts = new AccountDBContext();
        public static CredentialsDBContext Credentials = new CredentialsDBContext();
        public static SessionDBContext Sessions = new SessionDBContext();
        public static DatabaseContext General = new DatabaseContext();
        public static DatabaseContext Context = new DatabaseContext();
    }

    public class DatabaseContext
    {
        public DocumentStore documentStore;
        private string _certificate = $@"{AppContext.BaseDirectory}certificate.pfx";
        private string _databaseName = "Lightshift";

        public DatabaseContext()
        {
            documentStore = new DocumentStore
            {
                Urls = new[]
{
                    "https://a.free.ls.ravendb.cloud"
                },
                Database = _databaseName,
                Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(_certificate),
            };
            documentStore.Initialize();
        }
        public virtual T SaveDocument<T>(T document)
        {
            if (document == null)
                return document;

            var session = documentStore.OpenSession();
            session.Store(document);
            session.SaveChanges();

            return document;
        }

        public virtual List<T> SaveDocuments<T>(List<T> document)
        {
            if (document == null)
                return document;

            var session = documentStore.OpenSession();
            foreach (var item in document)
                session.Store(item);
            session.SaveChanges();

            return document;
        }

        public virtual void DeleteDocument<T>(string id)
        {
            var session = documentStore.OpenSession();
            var document = session.Load<T>(id);

            if (document != null)
            {
                session.Delete(document);
                session.SaveChanges();
            }
        }

        public virtual List<T> GetAllDocuments<T>()
        {
            return documentStore.OpenSession().Query<T>().ToList();
        }

        public IDocumentSession NewSession() => documentStore.OpenSession();
    }
}
