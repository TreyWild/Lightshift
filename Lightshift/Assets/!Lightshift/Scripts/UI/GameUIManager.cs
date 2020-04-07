using Lightshift;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private GameObject _settingsMenu { get; set; }
    private GameObject _respawnScreen { get; set; }
    private GameObject _weaponMenu { get; set; }
    private GameObject _chatBox { get; set; }
    private GameObject _playerMenu { get; set; }
    private GameObject _weaponListMenu { get; set; }
    private GameObject _shipListMenu { get; set; }
    private GameObject _weaponCreatorMenu { get; set; }
    private GameObject _shipCreatorMenu { get; set; }

    public ShipInterface ShipInterface;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        var shipInterface = Instantiate(_shipInterfacePrefab);
        ShipInterface = shipInterface.GetComponent<ShipInterface>();

        _chatBox = Instantiate(_chatBoxPrefab);

        if (ClientManager.Instance == null)
            SceneManager.LoadScene(0);
    }
    public void ToggleSettingsMenu()
    {
        if (_settingsMenu == null) 
            _settingsMenu = Instantiate(_settingsMenuPrefab);
        else Destroy(_settingsMenu);
    }

    public void ToggleInventory()
    {
        if (_inventoryCanvas.enabled) 
            _inventoryCanvas.enabled = false;
        else _inventoryCanvas.enabled = true;

        Settings.Instance.KeysLocked = _inventoryCanvas.enabled;
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

    void Update()
    {
        if (!Settings.Instance.KeysLocked) {
            if (Input.GetKeyDown(Settings.Instance.SettingsMenuKey))
                ToggleSettingsMenu();

            if (Input.GetKeyDown(Settings.Instance.InventoryKey))
                ToggleInventory();

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
}
