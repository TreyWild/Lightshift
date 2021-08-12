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
        [HttpPost("getloadout")]
        public ActionResult<LoadoutObject> GetLoadout(JsonString json)
        {
            if (!AuthenticationService.ValidateGameServerFromRequest(Request))
                return null;

            return DB.Context.NewSession().Query<LoadoutObject>().FirstOrDefault(s => s.Id == json.Value);
        }

        [HttpPost("getloadouts")]
        public ActionResult<List<LoadoutObject>> GetLoadouts(JsonString json)
        {
            if (!AuthenticationService.ValidateGameServerFromRequest(Request))
                return null;

            return DB.Context.NewSession().Query<LoadoutObject>().Where(s => s.UserId == json.Value).ToList();
        }

        [HttpPost("getitems")]
        public ActionResult<List<Item>> GetItems(JsonString json)
        {
            if (!AuthenticationService.ValidateGameServerFromRequest(Request))
                return null;

            return DB.Context.NewSession().Query<Item>().Where(s => s.UserId == json.Value).ToList();
        }

        [HttpPost("getitem")]
        public ActionResult<Item> GetItem(JsonString json)
        {
            if (!AuthenticationService.ValidateGameServerFromRequest(Request))
                return null;

            return DB.Context.NewSession().Query<Item>().FirstOrDefault(s => s.UserId == json.Value);
        }

        [HttpPost("saveitem")]
        public ActionResult<bool> SaveItem(Item item)
        {
            if (!AuthenticationService.ValidateGameServerFromRequest(Request))
                return false;

            DB.Context.SaveDocument(item);
            return true;
        }

        [HttpPost("saveitems")]
        public ActionResult<bool> SaveItems(List<Item> items)
        {
            if (!AuthenticationService.ValidateGameServerFromRequest(Request))
                return false;

            DB.Context.SaveDocuments(items);
            return true;
        }


        [HttpPost("saveloadout")]
        public ActionResult<bool> SaveLoadout(LoadoutObject loadout)
        {
            if (!AuthenticationService.ValidateGameServerFromRequest(Request))
                return false;

            DB.Context.SaveDocument(loadout);
            return true;
        }
    }
}
