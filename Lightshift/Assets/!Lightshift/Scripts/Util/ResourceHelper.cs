using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ResourceHelper : MonoBehaviour
{
    private static List<SolarSystem> _solarSystems;
    private static bool _inititalized = false;
    private static void Initialize()
    {
        _solarSystems = Resources.LoadAll<SolarSystem>("").ToList();
        _inititalized = true;
    }

    public static SolarSystem GetSolarSystem(string Id)
    {
        if (!_inititalized)
            Initialize();

        return _solarSystems.FirstOrDefault(i => i.Id.ToLower() == Id.ToLower());
    }

    public static SolarSystem GetDefaultSolarSystem()
    {
        if (!_inititalized)
            Initialize();

        return _solarSystems.FirstOrDefault(i => i.Default);
    }
}