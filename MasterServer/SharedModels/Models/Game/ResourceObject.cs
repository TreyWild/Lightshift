using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Models.Game
{
    public enum ResourceType
    {
        Lite,
        Virium,
        Tondrite,
        Glyphic,
        Rock,
        Talcium,
        Sand,
        Armorium,
        VoidShard,
        QuantamCrystal,
        CarbonSteel,
        NickelAlloy,
        Titanium,
        Talc,
        Gypsum,
        Calcite,
        Fluorite,
        Apatite,
        Feldspar,
        Quartz,
        Topaz,
        Diamond,
        Lonsdaleite,
        Sapphire,
        Gold,
    }

    public class ResourceCollection 
    {
        public string Id;

        public List<ResourceObject> Collection;
    }

    [Serializable]
    public struct ResourceObject
    {
        public int Amount;
        public ResourceType Type;
    }
}
