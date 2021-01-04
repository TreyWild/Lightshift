using SharedModels.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Lightshift.Scripts.Data
{
    [CreateAssetMenu(fileName = "CargoItem", menuName = "Lightshift/New Cargo Item", order = 2)]

    public class CargoItem : ScriptableObject
    {
        public CargoType Type;
        public string DisplayName;
        public string Lore;
        public Sprite Sprite;
    }


}
