using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Models.Game
{
    public enum ResourceType 
    {
        ToxicWaste,
        MetalScrap,
        LargeScrap,
        DarkShard,
        VoidShard,
        LightShard
    }

    public class ResourceCollection 
    {
        public string Id { get; set; }

        public List<ResourceObject> Collection { get; set; }
    }

    [Serializable]
    public class ResourceObject
    {
        public int Amount;
        public ResourceType Type;
    }
}
