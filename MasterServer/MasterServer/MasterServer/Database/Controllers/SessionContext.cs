using SharedModels.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterServer
{
    public class SessionContext : DatabaseContext
    {
        public SessionContext(string table) : base(table)
        {

        }

        public List<Session> GetSessions(string userId)
        {
            return documentStore.OpenSession().Query<Session>().Where(s => s.userId == userId).ToList();
        }

        public Session GetSession(string sessionId)
        {
            return documentStore.OpenSession().Query<Session>().FirstOrDefault(s => s.Id == sessionId);
        }
        public void Save(Session document)
        {
            if (document == null)
                return;

            var session = documentStore.OpenSession();
            session.Store(document);
            session.SaveChanges();
        }
    }
}
