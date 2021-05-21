using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Lightshift.Scripts.SolarSystem
{
    public class LandableManager : MonoBehaviour
    {
        public static LandableManager Instance;
        private List<Landable> _landables = new List<Landable>();
        private void Awake()
        {
            Instance = this;
            _landables = FindObjectsOfType<Landable>().ToList();
        }

        public static Landable GetLandableById(string id) 
        {
            var station = Instance._landables.FirstOrDefault(i => i.Id == id);
            if (station == null)
                station = Instance._landables.FirstOrDefault();
            return station;
        }

        public static void RequestLand(string id) 
        {
            var player = FindObjectsOfType<Player>().FirstOrDefault(p => p.isLocalPlayer);
            player.Land(id);
        }

        public static void Land(string id) 
        {
            var landable = GetLandableById(id);
            if (landable == null)
                return;

            GameUIManager.Instance.ShowLandable(landable.Type);
        }
    }
}
