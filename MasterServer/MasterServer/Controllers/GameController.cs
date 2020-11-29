using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MasterServer.Services;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Models.Game;
using SharedModels.WebRequestObjects;
using Raven.Client.Documents.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MasterServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        [HttpPost("getship")]
        public ActionResult<ShipObject> GetShip(JsonString json)
        {
            if (!AuthenticationService.ValidateGameServerFromRequest(Request))
                return null;

            return DB.Context.NewSession().Query<ShipObject>().FirstOrDefault(s => s.Id == json.Value);
        }

        [HttpPost("getships")]
        public ActionResult<List<ShipObject>> GetShips(JsonString json)
        {
            if (!AuthenticationService.ValidateGameServerFromRequest(Request))
                return null;

            return DB.Context.NewSession().Query<ShipObject>().Where(s => s.UserId == json.Value).ToList();
        }

        [HttpPost("saveship")]
        public ActionResult<bool> SaveShip(ShipObject ship)
        {
            if (!AuthenticationService.ValidateGameServerFromRequest(Request))
                return false;

            DB.Context.SaveDocument(ship);
            return true;
        }
    }
}
