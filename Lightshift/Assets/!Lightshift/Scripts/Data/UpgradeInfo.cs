using SharedModels.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Lightshift.Scripts.Data
{
    [Serializable]
    public class UpgradeInfo
    {
        public int Cost;

        public float Value;

        public Modifier Type;
    }
}
