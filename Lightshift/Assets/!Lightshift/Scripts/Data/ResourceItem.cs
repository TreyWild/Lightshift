using SharedModels.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Lightshift.Scripts.Data
{
    [CreateAssetMenu(fileName = "ResourceItem", menuName = "Lightshift/Create ResourceItem", order = 2)]

    public class ResourceItem : ScriptableObject
    {
        public ResourceType Type;
        public string DisplayName;
        public string Lore;
        public Sprite Sprite;
        public List<GameModifier> Modifiers;
        public List<ResourceObject> RecycleValue;
    }


}
