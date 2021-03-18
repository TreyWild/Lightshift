using Assets._Lightshift.Scripts.SolarSystem;
using Lightshift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DockPromptControl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _display;
    private Canvas _canvas;
    public Landable landable;

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
        _display.text = $"Press [{Settings.DockKey}] to Dock";
    }
    public void Dock() 
    {
        LandableManager.RequestLand(landable.Id);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (Settings.KeysLocked)
        {
            _canvas.enabled = false;
            return;
        }

        _canvas.enabled = true;
        if (Input.GetKeyDown(Settings.DockKey))
        {
            Dock();
        }
    }
}

