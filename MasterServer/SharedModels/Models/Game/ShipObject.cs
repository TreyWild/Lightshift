using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Models.Game
{
    [Serializable]
    public class ShipObject
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string HullId { get; set; }
        public List<string> EquippedModules { get; set; }
        public List<Item> OwnedItems { get; set; }
    }
}
