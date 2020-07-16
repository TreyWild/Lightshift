using SharedModels.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterServer
{
    public class AccountContext : DatabaseContext
    {
        public AccountContext(string table) : base(table) 
        {

        }

        private List<Account> _cashedAccounts = new List<Account>();
        public Account Get(string id)
        {
            var Account = _cashedAccounts.FirstOrDefault(u => u.Id == id);
            if (Account == null)
                Account = documentStore.OpenSession().Query<Account>().FirstOrDefault(u => u.Id == id);
            return Account;
        }

        public Account GetByEmail(string email)
        {
            var Account = _cashedAccounts.FirstOrDefault(u => u.emailAddress == email);
            if (Account == null)
                Account = documentStore.OpenSession().Query<Account>().FirstOrDefault(u => u.emailAddress == email);
            return Account;
        }

        public void Save(Account document)
        {
            if (document == null)
                return;

            _cashedAccounts.Add(document);
            var session = documentStore.OpenSession();
            session.Store(document);
            session.SaveChanges();
        }

        public bool UsernameAvailable(string userName)
        {
            return Database.Accounts.documentStore.OpenSession().Query<Account>().FirstOrDefault(u => u.username.ToUpper() == userName.ToUpper()) == null;
        }

        public bool EmailAvailable(string email)
        {
            return Database.Accounts.documentStore.OpenSession().Query<Account>().FirstOrDefault(u => u.emailAddress.ToUpper() == email.ToUpper()) == null;
        }
    }
}
