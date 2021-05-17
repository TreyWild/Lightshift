using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ResourceHelper : MonoBehaviour
{
    private static List<Galaxy> _galaxies;
    private static bool _inititalized = false;
    private static void Initialize()
    {
        _galaxies = Resources.LoadAll<Galaxy>("").ToList();
        _inititalized = true;
    }

    public static Galaxy GetGalaxyById(string Id)
    {
        if (!_inititalized)
            Initialize();

        return _galaxies.FirstOrDefault(i => i.Id.ToLower() == Id.ToLower());
    }

    public static Galaxy GetDefaultGalaxy()
    {
        if (!_inititalized)
            Initialize();

        return _galaxies.FirstOrDefault(i => i.Default);
    }
}