using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    [SerializeField] GameObject _parallax;
    [SerializeField] GameObject _backGroundObjects;

    private List<MeshRenderer> _renderers = new List<MeshRenderer>();

    public static ParallaxManager Instance;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else Instance = this;
    }
    private void Start()
    {
        var renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in renderers)
            _renderers.Add(renderer);

        Initialize(Color.white);
    }

    public void Initialize(Color color)
    {

        foreach (var renderer in _renderers)
        {
            foreach (var material in renderer.materials)
            {
                material.SetColor("_TintColor", color);
                material.SetColor("_SpecColor", color);
            }
        }
    }
    public void Initialize(string color)
    {
        Color newColor;
        if (ColorUtility.TryParseHtmlString(color, out newColor))
            Initialize(newColor);
        else Debug.LogError("Parallax color couldn't be assigned");
    }

    public void ShowSkybox(bool show = true) 
    {
        _parallax.SetActive(show);
        if (show)
            Camera.main.clearFlags = CameraClearFlags.Depth;
        else Camera.main.clearFlags = CameraClearFlags.SolidColor;
    }

    public void ShowBackgroundObjects(bool show = true)
    {
        _backGroundObjects.SetActive(show);
    }
}
