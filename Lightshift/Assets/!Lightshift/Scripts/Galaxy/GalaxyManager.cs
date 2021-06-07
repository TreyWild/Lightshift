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

        _parallaxManager = GetComponentInChildren<ParallaxManager>();
        _galaxy = ResourceHelper.GetDefaultGalaxy();

        Initialize(_galaxy.Name);
    }

    public static Galaxy GetGalaxy() => Instance._galaxy;

    private ParallaxManager _parallaxManager { get; set; }

    private Galaxy _galaxy { get; set; }

    private string _name;

    public void Initialize(string name)
    {
        _name = name;
        //Color parsedColor = (Color)System.Enum.Parse(typeof(Color), color);

        //_parallaxManager.Initialize(parsedColor);
        SoundManager.Instance.PlayMusic();
    }
}
