using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyManager : MonoBehaviour
{
    public static GalaxyManager Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }

        Instance = this;

    }

    public static Galaxy GetGalaxy() => Instance._galaxy;

    private ParallaxManager _parallaxManager { get; set; }

    private Galaxy _galaxy { get; set; }

    private string _name;

    private void Start()
    {
        _parallaxManager = GetComponentInChildren<ParallaxManager>();
        _galaxy = ResourceHelper.GetDefaultGalaxy();
    }
    public void Initialize(string name, string color)
    {
        _name = name;
        Color parsedColor = (Color)System.Enum.Parse(typeof(Color), color);

        _parallaxManager.Initialize(parsedColor);
    }
}
