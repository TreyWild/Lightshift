using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Models.Game
{
    [Serializable]
    public class Item
    {
        public string Id { get; set; }
        public string ModuleId { get; set; }
        public string Color { get; set; } = "FFFFFF";
        public List<Upgrade> Upgrades { get; set; }
        public ModuleType ModuleLocation { get; set; }
        public int SpentCredits { get; set; }
    }
}
