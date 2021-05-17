using Assets._Lightshift.Scripts.Network;
using Assets._Lightshift.Scripts.UI;
using Lightshift;
using Mirror;
using SharedModels.Models.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{

    public static GameUIManager Instance;

    [SerializeField] private List<UIObject> _uiObjects = new List<UIObject>();

    [HideInInspector]
    public ShipInterface ShipInterface;

    private TextMeshProUGUI _statsText;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        InitUI();
        ToggleLoadingScreen(true);
    }

    private void OnDestroy()
    {
        Destroy(Instance);
        Instance = null;
        Destroy(ShipInterface);
        ShipInterface = null;
        Destroy(_statsText);
        _statsText = null;

        for (int i = 0; i < _uiObjects.Count; i++)
        {
            Destroy(_uiObjects[i].MemoryStorage);
            _uiObjects[i] = null;
        }

        _uiObjects = null;
    }

    public void InitUI() 
    {
        var shipInterface = ShowUI("ShipInterface");
        ShipInterface = shipInterface.GetComponent<ShipInterface>();

        ShowUI("Chat");
    }

    public UIObject GetUI(string key) => _uiObjects.FirstOrDefault(u => u.Key.ToUpper() == key.ToUpper());

    public GameObject ShowUI(string key, bool useGameCanvas = false) 
    {
        var ui = GetUI(key);
        if (ui.MemoryStorage != null && ui.ReinstiantiateOnToggle)
        {
            Destroy(ui.MemoryStorage);

            ui.MemoryStorage = Instantiate(ui.Prefab);
        }
        else if (ui.MemoryStorage == null)
        {
            ui.MemoryStorage = Instantiate(ui.Prefab);
        }
        else ui.MemoryStorage.SetActive(true);

        return ui.MemoryStorage;
    }

    public void DestroyUI(string key) 
    {
        var ui = GetUI(key);
        if (ui != null)
        {
            Destroy(ui.MemoryStorage);
            ui.MemoryStorage = null;
        }
    }
    public GameObject ToggleUI(string key, bool active = true, bool useGameCanvas = false)
    {
        var ui = GetUI(key);

        ui.MemoryStorage = ShowUI(key, useGameCanvas);
        ui.MemoryStorage.SetActive(active);

        return ui.MemoryStorage;
    }

    private void Start()
    {
        //if (Server.Instance != null)
        //    ToggleAllUI(false);

        if (Settings.Instance != null)
            ShowScreenStats(Settings.Instance.ShowDebugStats);
    }

    public void ShowScreenStats(bool active) 
    {
        var ui = ToggleUI("PerformanceText", active);
        _statsText = ui.GetComponentInChildren<TextMeshProUGUI>();
    }



    public void ShowAnnouncementText(string message) 
    {
        var ui = ShowUI("AnnounceText", true);
        var text = ui.GetComponentInChildren<TextMeshProUGUI>();
        text.text = message;
    }

    public void ShowScreenText(string message)
    {
        var ui = ShowUI("LowerText", true);
        var text = ui.GetComponentInChildren<TextMeshProUGUI>();
        text.text = message;
    }

    private float _timeSinceLastStatUpdate;
    void Update()
    {
        _timeSinceLastStatUpdate += Time.deltaTime;
        if (_timeSinceLastStatUpdate > .5f)
        if (_statsText != null) 
        {
            _statsText.text = $"ping: {Communication.GetPing()}, fps: {(int)(1f / Time.unscaledDeltaTime)}";
                _timeSinceLastStatUpdate = 0;
        }

        if (!Settings.KeysLocked) {

            //if (Input.GetKeyDown(Settings.InventoryKey))
            //    ToggleInventoryUI(true);

            if (Input.GetKeyDown(Settings.PlayerMenuKey))
                TogglePlayerMenu();

            if (Input.GetKeyDown(Settings.CargoMenu))
                ToggleCargoMenu();

            if (Input.GetKeyDown(Settings.MapKey))
                ToggleSystemMap();
        }
    }

    public void ToggleAllUI(bool active) 
    {
        foreach (var ui in _uiObjects)
            ToggleUI(ui.Key, active);
    }

    //public void ToggleWeaponMenu(WeaponSystem weaponSystem)
    //{
    //    if (_weaponMenu != null)
    //    {
    //        Destroy(_weaponMenu);
    //        return;
    //    }

    //    _weaponMenu = Instantiate(_weaponMenuPrefab);
    //    var weaponSelector = _weaponMenu.GetComponent<WeaponSelector>();

    //    weaponSelector.Initialize(weaponSystem);
    //}

    private bool _playerMenuOpen = false;
    public void TogglePlayerMenu() 
    {
        _playerMenuOpen = !_playerMenuOpen;
        ToggleUI("PlayerList", _playerMenuOpen);
    }

    private bool _systemMapOpen = false;
    public void ToggleSystemMap()
    {
        _systemMapOpen = !_systemMapOpen;
        ToggleUI("SystemMap", _systemMapOpen);
    }

    private bool _cargoMenuOpen = false;
    public void ToggleCargoMenu()
    {
        _cargoMenuOpen = !_cargoMenuOpen;

        if (_cargoMenuOpen)
            ShowUI("CargoMenu");
        else DestroyUI("CargoMenu");
    }

    public void TryUpdatePlayerMenu() 
    {
        var ui = GetUI("PlayerList");
        if (ui.MemoryStorage != null)
        {
            var playerList = ui.MemoryStorage.GetComponent<PlayerList>();
            playerList.ShowOnlinePlayers();
        }
    }

    public void ToggleLoadingScreen(bool active) 
    {
        ToggleUI("LoadingScreen", active);
    }

    public void ShowLandable(LandableType landable) 
    {
        ToggleAllUI(false);
        ToggleUI(landable.ToString());
        Settings.KeysLocked = true;
    }

    public void LeaveLandable()
    {
        //ToggleAllUI(true);
        InitUI();
        foreach (LandableType landable in (LandableType[])Enum.GetValues(typeof(LandableType)))
            DestroyUI(landable.ToString());

        Settings.KeysLocked = false;
    }

    public void ShowDockingPrompt(Landable landable) 
    {
        var docker = ShowUI("DockPrompt");
        var control = docker.GetComponent<DockPromptControl>();
        control.landable = landable;
    }

    public void HideDockPromot() 
    {
        DestroyUI("DockPrompt");
    }
}
