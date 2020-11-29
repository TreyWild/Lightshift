using SharedModels.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._Lightshift.Scripts.Data
{
    public class PlayerDefaults
    {
        public static ShipObject GetDefaultShip() 
        {
            var ship = new ShipObject 
            {
                Id = Guid.NewGuid().ToString(),
                HullId = "infernoHull",
                OwnedItems = new List<Item>
                {
                    // Hull
                    new Item
                    {
                        Id = Guid.NewGuid().ToString(),
                        ModuleId = "infernoHull",
                        ModuleLocation = ModuleType.Hull,
                    },

                    // Wings
                    new Item
                    {
                        Id = Guid.NewGuid().ToString(),
                        ModuleId = "aeroWings",
                        ModuleLocation = ModuleType.PrimaryWings,
                    },

                    // Engine
                    new Item
                    {
                        Id = Guid.NewGuid().ToString(),
                        ModuleId = "combustionEngine",
                        ModuleLocation = ModuleType.Engine
                    },
                }
            };

            ship.EquippedModules = new List<string>();
            foreach (var item in ship.OwnedItems)
                ship.EquippedModules.Add(item.Id);

            return ship;
        }

        public static string GetDefaultStation() => "1093c819-a7a5-41ea-8a60-f4f1bae1fc19";
    }
}
