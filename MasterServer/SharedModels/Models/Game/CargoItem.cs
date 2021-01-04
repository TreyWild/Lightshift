using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Models.Game
{
    public enum CargoType 
    {
        ToxicWaste,
        MetalScrap,
        LargeScrap,
    }
    public class CargoObject
    {
        public int Amount { get; set; }
        public CargoType Type { get; set; }
    }
}
