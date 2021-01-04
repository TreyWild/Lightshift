using SharedModels.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Lightshift.Scripts.Utilities
{
    public class ColorHelper
    {
        public static Color FromHex(string hex) 
        {
            Color color = Color.green;
            ColorUtility.TryParseHtmlString(hex, out color);
                
            return color;
        }

        public static Color FromModifier(Modifier modifier) 
        {
            Color color = FromHex("#8f4d17");
            switch (modifier)
            {
                case Modifier.Acceleration:
                    color = FromHex($"#884AFF");
                    break;
                case Modifier.Agility:
                    color = FromHex($"#D64AFF");
                    break;
                case Modifier.Armor:
                    color = FromHex($"#FF974A");
                    break;
                case Modifier.Health:
                    color = FromHex($"#49FF85");
                    break;
                case Modifier.Shield:
                    color = FromHex($"#4AB6FF");
                    break;
                case Modifier.Regen:
                    color = FromHex($"#4A81FF");
                    break;
                case Modifier.Speed:
                    color = FromHex($"#FF4ABE");
                    break;
                case Modifier.Weight:
                    color = FromHex($"#FF4A6C");
                    break;
                case Modifier.BrakeForce:
                    color = FromHex($"#db9430");
                    break;
            }

            return color;
        }
    }
}
