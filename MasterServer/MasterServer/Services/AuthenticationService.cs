using Microsoft.AspNetCore.Http;
using SharedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MasterServer.Services
{
    public static class AuthenticationService
    {
        private static List<Session> _sessions = new List<Session>();

        public static Account GetAccountFromSessionKey(string key)
        {
            var session = _sessions.FirstOrDefault(s => s.SessionId == key);
            if (session == null || (DateTime.Now - session.CreationDate).TotalHours > 6)
                return null;

            return DB.Accounts.GetById(session.UserId);
        }
        public static Account GetAccountFromRequest(HttpRequest request)
        {
            if (!request.Headers.ContainsKey("auth"))
                return null;

            var sessionId = request.Headers["auth"];
            var session = _sessions.FirstOrDefault(s => s.SessionId == sessionId);
            if (session == null || (DateTime.Now - session.CreationDate).TotalHours > 6)
                return null;

            return DB.Accounts.GetById(session.UserId);
        }

        public static string CreateSession(string userId, HttpRequest request)
        {
            var session = _sessions.FirstOrDefault(s => s.UserId == userId);
            if (session != null)
                _sessions.Remove(session);

            session = new Session
            {
                CreationDate = DateTime.Now,
                SessionId = Guid.NewGuid().ToString(),
                UserId = userId,
                IP = request.HttpContext.Connection.RemoteIpAddress,
            };

            _sessions.Add(session);

            return session.SessionId;
        }

        public static bool ValidateGameServerFromRequest(HttpRequest request)
        {
            if (!request.Headers.ContainsKey("auth"))
                return false;

            var authKey = request.Headers["auth"];

            var credentials = DB.Credentials.GetGameServerCredentials(authKey);

            if (credentials == null)
                return false;

            return true;
        }

        private class Session
        {
            public string SessionId;
            public string UserId;
            public DateTime CreationDate;
            public IPAddress IP;
        }
    }
}
