
using SharedModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterServer
{
    public class AccountDBContext : DatabaseContext
    {
        public Account Get(string id)
        {
            return documentStore.OpenSession().Query<Account>().FirstOrDefault(u => u.Id == id);
        }

        public Account GetByEmail(string email)
        {
            return documentStore.OpenSession().Query<Account>().FirstOrDefault(u => u.EmailAddress == email.ToLowerInvariant());
        }

        public Account GetById(string id)
        {
            return documentStore.OpenSession().Query<Account>().FirstOrDefault(u => u.Id == id);
        }

        public void Save(Account document)
        {
            if (document == null)
                return;

            var session = documentStore.OpenSession();
            session.Store(document);
            session.SaveChanges();
        }

        public bool UsernameAvailable(string userName)
        {
            return documentStore.OpenSession().Query<Account>().FirstOrDefault(u => u.CaseSensitiveUsername == userName.ToLowerInvariant()) == null;
        }

        public bool EmailAvailable(string email)
        {
            return documentStore.OpenSession().Query<Account>().FirstOrDefault(u => u.EmailAddress == email.ToLowerInvariant()) == null;
        }
    }
}
