using Lightshift;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{

    public static GameUIManager Instance;

    [SerializeField] GameObject _settingsMenuPrefab;
    [SerializeField] GameObject _weaponMenuPrefab;
    [SerializeField] GameObject _respawnScreenPrefab;
    [SerializeField] Canvas _inventoryCanvas;
    [SerializeField] GameObject _announceTextPrefab;
    [SerializeField] GameObject _lowerTextPrefab;
    [SerializeField] Canvas _gameCanvas;
    [SerializeField] GameObject _chatBoxPrefab;
    [SerializeField] GameObject _shipInterfacePrefab;
    [SerializeField] GameObject _playerListPrefab;
    [SerializeField] GameObject _devCreatorMenuPrefab;
    [SerializeField] GameObject _devListPrefab;
    [SerializeField] GameObject _performanceStatsPrefab;

    private GameObject _settingsMenu { get; set; }
    private GameObject _respawnScreen { get; set; }
    private GameObject _weaponMenu { get; set; }
    private GameObject _chatBox { get; set; }
    private GameObject _playerMenu { get; set; }
    private GameObject _weaponListMenu { get; set; }
    private GameObject _shipListMenu { get; set; }
    private GameObject _weaponCreatorMenu { get; set; }
    private GameObject _shipCreatorMenu { get; set; }

    private GameObject _performanceStats { get; set; }


    public ShipInterface ShipInterface;

    private TextMeshProUGUI _statsText;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        var shipInterface = Instantiate(_shipInterfacePrefab);
        ShipInterface = shipInterface.GetComponent<ShipInterface>();

        _chatBox = Instantiate(_chatBoxPrefab);
    }

    private void Start()
    {
        if (Server.Instance != null)
            ToggleAllUI(false);

        if (Settings.Instance != null)
            ShowScreenStats(Settings.Instance.ShowDebugStats);
    }

    public void ShowScreenStats(bool active) 
    {
        if (active && _performanceStats == null)
        {
            _performanceStats = Instantiate(_performanceStatsPrefab);
            _statsText = _performanceStats.GetComponentInChildren<TextMeshProUGUI>();
        }
        else if (_performanceStats != null)
        {
            _performanceStats.SetActive(active);

            _statsText = _performanceStats.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    public void ToggleSettingsMenu()
    {
        if (_settingsMenu == null) 
            _settingsMenu = Instantiate(_settingsMenuPrefab);
        else Destroy(_settingsMenu);
    }

    public void ShowAnnouncementText(string message) 
    {
        var obj = Instantiate(_announceTextPrefab, _gameCanvas.transform);
        var text = obj.GetComponent<TextMeshProUGUI>();
        text.text = message;
    }

    public void ShowScreenText(string message)
    {
        var obj = Instantiate(_lowerTextPrefab, _gameCanvas.transform);
        var text = obj.GetComponent<TextMeshProUGUI>();
        text.text = message;
    }

    private float _timeSinceLastStatUpdate;
    void Update()
    {
        _timeSinceLastStatUpdate += Time.deltaTime;
        if (_timeSinceLastStatUpdate > .5f)
        if (_statsText != null) 
        {
            _statsText.text = $"ping: {(int)((NetworkTime.rtt / 2) * 1000)}, fps: {(int)(1f / Time.unscaledDeltaTime)}";
                _timeSinceLastStatUpdate = 0;
        }

        if (!Settings.Instance.KeysLocked) {
            if (Input.GetKeyDown(Settings.Instance.SettingsMenuKey))
                ToggleSettingsMenu();

            if (Input.GetKeyDown(Settings.Instance.InventoryKey))
                ToggleInventoryUI(true);

            if (Input.GetKeyDown(Settings.Instance.PlayerMenuKey))
                TogglePlayerMenu();
        }
    }

    public void HandleRespawnScreen(bool showRespawn = true, string attackerName = "") 
    {
        if (showRespawn)
        {
            ToggleAllUI(false);
            _respawnScreen = Instantiate(_respawnScreenPrefab);
            var respawnUI = _respawnScreen.GetComponent<RespawnUI>();
            respawnUI.Initialize(respawnTime:5.3f, attackerName);

            Settings.Instance.KeysLocked = true;
            //On Respawn
            //respawnUI.onRespawn += () => ClientManager.Instance.Send("g", "respawn");
        }
        else 
        {
            Destroy(_respawnScreen);
            ToggleAllUI(true);
            Settings.Instance.KeysLocked = false;
        }
    }

    public void ToggleAllUI(bool active) 
    {
        if (_respawnScreen != null)
            _respawnScreen?.SetActive(active);

        if (_settingsMenu != null)
            _settingsMenu.SetActive(active);

        if (ShipInterface != null)
            ShipInterface?.gameObject.SetActive(active);

        if (_weaponMenu != null)
        _weaponMenu?.gameObject.SetActive(active);

        if (_chatBox != null)
            _chatBox.gameObject.SetActive(active);

        if (_weaponCreatorMenu != null)
            _weaponCreatorMenu.SetActive(active);

        if (_shipCreatorMenu != null)
            _shipCreatorMenu.SetActive(active);

        if (_weaponListMenu != null)
            _weaponListMenu.SetActive(active);

        if (_shipListMenu != null)
            _shipListMenu.SetActive(active);

        if (_inventoryUI != null)
            ToggleInventoryUI(active);
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

    public void TogglePlayerMenu() 
    {
        if (_playerMenu == null)
            _playerMenu = Instantiate(_playerListPrefab);
        else 
        {
            var playerList = _playerMenu.GetComponent<PlayerList>();
            playerList.Exit();
        }
    }

    public void TryUpdatePlayerMenu() 
    {
        if (_playerMenu != null)
        {
            var playerList = _playerMenu.GetComponent<PlayerList>();
            playerList.ShowOnlinePlayers();
        }
    }

    private InventoryUI _inventoryUI;
    public void HookInventoryUI(InventoryUI inventory) 
    {
        if (_inventoryUI != null)
            Destroy(_inventoryUI.gameObject);

        _inventoryUI = inventory;
    }

    private void ToggleInventoryUI(bool toggle)
    {
        if (_inventoryUI == null)
            return;

        _inventoryUI?.gameObject.SetActive(toggle);
        Settings.Instance.KeysLocked = toggle;
    }
}
