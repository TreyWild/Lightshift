using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    public static SystemManager Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }

        Instance = this;

    }

    public static SolarSystem GetSolarSystem() => Instance.SolarSystem;

    private ParallaxManager _parallaxManager { get; set; }

    public SolarSystem SolarSystem { get; set; }

    private string _name;

    private void Start()
    {
        _parallaxManager = GetComponentInChildren<ParallaxManager>();
        SolarSystem = ResourceHelper.GetDefaultSolarSystem();
    }
    public void Initialize(string name, string color)
    {
        _name = name;
        Color parsedColor = (Color)System.Enum.Parse(typeof(Color), color);

        _parallaxManager.Initialize(parsedColor);
    }
}
