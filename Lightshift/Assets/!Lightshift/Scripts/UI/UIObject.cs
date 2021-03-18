using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Lightshift.Scripts.UI
{
    [Serializable]
    public class UIObject
    {
        public GameObject Prefab;

        [HideInInspector]
        public GameObject MemoryStorage;

        public bool ReinstiantiateOnToggle = true;
        public string Key;
    }
}
