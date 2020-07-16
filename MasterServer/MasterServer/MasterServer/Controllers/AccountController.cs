using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SharedModels.Models.User;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MasterServer.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class AccountController : ControllerBase
    {


        [HttpGet("{id}"), Route("[controller]/get")]
        public Account Get(string id)
        {
            return Database.Accounts.Get(id);
        }

        [HttpGet("{email}"), Route("[controller]/getbyemail")]
        public Account GetByEmail(string email)
        {
            return Database.Accounts.GetByEmail(email);
        }

        [HttpGet("{email}"), Route("[controller]/checkemailavailability")]
        public bool CheckEmailAvailability(string email)
        {
            return Database.Accounts.EmailAvailable(email);
        }

        [HttpGet("{email}"), Route("[controller]/getusernametag")]
        public bool CheckUsernameAvailability(string username)
        {
            return Database.Accounts.UsernameAvailable(username);
        }

        [HttpPost]
        public ActionResult<bool> Save(Account data)
        {
            Database.Accounts.Save(data);
            return true;
        }
    }
}
