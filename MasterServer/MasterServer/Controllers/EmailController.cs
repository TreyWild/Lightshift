using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MasterServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        // POST api/<EmailController>
        [HttpPost]
        public void Post(EmailShell shell)
        {
            EmailService.Send(shell.emailAddress, shell.username, shell.subject, shell.message);
        }
    }
}
