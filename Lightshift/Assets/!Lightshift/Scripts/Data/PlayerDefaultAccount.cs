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
        public static List<ResourceObject> GetTestResources() 
        {
            var list = new List<ResourceObject>();

            foreach (ResourceType type in (ResourceType[])Enum.GetValues(typeof(ResourceType)))
                list.Add(new ResourceObject { Type = type, Amount = 99999 });

            return list;
        }
        public static List<Item> GetDefaultItems()
        {
            var list = new List<Item>()
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
            };

            return list;
        }

        public static string GetDefaultStation() => "1093c819-a7a5-41ea-8a60-f4f1bae1fc19";
    }
}
