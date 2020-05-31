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

    [SerializeField] GameObject _announceTextPrefab;
    [SerializeField] GameObject _lowerTextPrefab;
    [SerializeField] Canvas _gameCanvas;
    [SerializeField] GameObject _chatBoxPrefab;
    [SerializeField] GameObject _shipInterfacePrefab;
    [SerializeField] GameObject _playerListPrefab;
    [SerializeField] GameObject _performanceStatsPrefab;
    private GameObject _chatBox { get; set; }
    private GameObject _playerMenu { get; set; }
    private GameObject _shipListMenu { get; set; }
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
            _statsText.text = $"ping: {Game.GetPing()}, fps: {(int)(1f / Time.unscaledDeltaTime)}";
                _timeSinceLastStatUpdate = 0;
        }

        if (!Settings.KeysLocked) {

            if (Input.GetKeyDown(Settings.InventoryKey))
                ToggleInventoryUI(true);

            if (Input.GetKeyDown(Settings.PlayerMenuKey))
                TogglePlayerMenu();
        }
    }

    public void HandleRespawnScreen(bool hideAllUI) 
    {
        ToggleAllUI(!hideAllUI);
        Settings.KeysLocked = hideAllUI;
    }

    public void ToggleAllUI(bool active) 
    {

        if (!active)
            MainMenu.SetActive(active);

        if (ShipInterface != null)
            ShipInterface?.gameObject.SetActive(active);

        //if (_chatBox != null)
        //    _chatBox.gameObject.SetActive(active);

        if (_shipListMenu != null)
            _shipListMenu.SetActive(active);

        if (_inventoryUI != null)
            _inventoryUI.gameObject.SetActive(false);
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

        if (toggle)
            _inventoryUI?.ShowInventoryHeldItemSlot();
        _inventoryUI?.gameObject.SetActive(toggle);
        Settings.KeysLocked = toggle;
    }
}
