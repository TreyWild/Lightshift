using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Models.User;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MasterServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {

        [HttpGet("{id}"), Route("[controller]/get")]
        public Session Get(string id)
        {
            return Database.Sessions.GetSession(id);
        }

        [HttpGet("{userId}"), Route("[controller]/getall")]
        public List<Session> GetSessions(string userId)
        {
            return Database.Sessions.GetSessions(userId);
        }

        [HttpPost]
        public ActionResult<bool> Save(Session data)
        {
            Database.Sessions.Save(data);
            return true;
        }
    }
}
