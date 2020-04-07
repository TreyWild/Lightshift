using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystem : MonoBehaviour
{
    private ParallaxManager _parallaxManager { get; set; }

    private string _name;

    private void Start()
    {
        _parallaxManager = GetComponentInChildren<ParallaxManager>();
    }
    public void Initialize(string name, string color)
    {
        _name = name;
        Color parsedColor = (Color)System.Enum.Parse(typeof(Color), color);

        _parallaxManager.Initialize(parsedColor);
    }
}
