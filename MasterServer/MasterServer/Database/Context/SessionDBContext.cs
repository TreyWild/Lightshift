using SharedModels;
using System.Collections.Generic;
using System.Linq;

namespace MasterServer
{
    public class SessionDBContext : DatabaseContext
    {
        public List<Session> GetSessions(string userId)
        {
            return documentStore.OpenSession().Query<Session>().Where(s => s.Id == userId).ToList();
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
