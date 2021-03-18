using Assets._Lightshift.Scripts.Util;
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
        [Header("ID REQUIRED")]
        public string Id;

        [Header("RESOURCE COST")]
        public List<ResourceObject> ResourceCost;

        [Header("Maximum Upgrade Value")]
        public float Value;

        [Header("Upgrade Type")]
        public Modifier Type;
    }
}
