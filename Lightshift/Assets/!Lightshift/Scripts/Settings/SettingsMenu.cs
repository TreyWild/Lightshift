﻿using Lightshift;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{

    public List<KeyItem> keybindings = new List<KeyItem>();
    public List<ToggleItem> toggleOptions = new List<ToggleItem>();
    public List<SliderItem> sliderOptions = new List<SliderItem>();
    public List<PickerItem> pickerItems = new List<PickerItem>();

    public GameObject contentPanel;
    public GameObject keyBindingPrefab;
    public GameObject toggleItemPrefab;
    public GameObject sliderItemPrefab;
    public GameObject dividerItemPrefab;
    public GameObject pickerItemPrefab;

    [SerializeField] private Button _saveButton;

    public enum SettingsType 
    {
        None,
        Controls,
        Sound,
        Graphics,
        Options
    }
    private bool _saved = false;
    private bool _fullScreen;

    private void OnDestroy()
    {
        keybindings = null;
        toggleOptions = null;
        sliderOptions = null;
        pickerItems = null;
        contentPanel = null;
        keyBindingPrefab = null;
        toggleItemPrefab = null;
        sliderItemPrefab = null;
        dividerItemPrefab = null;
        pickerItemPrefab = null;
        _saveButton = null;
    }

    private SettingsType _mode;
    public void Init(SettingsType mode = default)
    {
        _mode = mode;

        _saveButton.onClick.AddListener(delegate
        {
            Save();
        });
        Settings.KeysLocked = true;

        if (mode == SettingsType.Graphics)
        {
            CreateDivider("Graphics");
            CreateItemPicker("Quality", "gameQuality", new List<PickerItemObject>
        {
            new PickerItemObject
            {
                displayValue = "Incredible",
                value = "Fantastic",
            },
            new PickerItemObject
            {
                displayValue = "Beautiful",
                value = "Beautiful",
            },
            new PickerItemObject
            {
                displayValue = "Moderate",
                value = "Good",
            },
            new PickerItemObject
            {
                displayValue = "Lame",
                value = "Simple",
            },new PickerItemObject
            {
                displayValue = "Boring",
                value = "Fast",
            },
            new PickerItemObject
            {
                displayValue = "My eyes are bleeding",
                value = "Fastest",
            },
        });

            CreateItemPicker("Frame Rate", "frameRate", new List<PickerItemObject>
        {
            new PickerItemObject
            {
                displayValue = "30 FPS",
                value = "30",
            },
            new PickerItemObject
            {
                displayValue = "60 FPS",
                value = "60",
            },
            new PickerItemObject
            {
                displayValue = "120 FPS",
                value = "120",
            },new PickerItemObject
            {
                displayValue = "Unlimited",
                value = Int32.MaxValue.ToString(),
            },
        });

            CreateItemPicker("Window Mode", "screenMode", new List<PickerItemObject>
        {
            new PickerItemObject
            {
                displayValue = "Fullscreen",
                value = "FullScreenWindow",
            },
            new PickerItemObject
            {
                displayValue = "Windowed",
                value = "Windowed",
            }
        });

            var resolutions = Screen.resolutions;
            var resolutionItems = new List<PickerItemObject>();
            foreach (var item in resolutions)
            {
                resolutionItems.Add(new PickerItemObject
                {
                    displayValue = item.ToString(),
                    value = $"{item.width}:{item.height}"
                });
            }

            CreateItemPicker("Resolution", "resolution", resolutionItems);

            CreateToggleItem("Show Skybox", "showSkybox");
            CreateToggleItem("Background Objects", "showBackgroundElements");

            return;
        }

        if (mode == SettingsType.Options)
        {
            CreateDivider("Utility");
            CreateToggleItem("Show Debug Stats", "showDebugStats");
            CreateToggleItem("Show Damage Text", "showDamageText");
            return;
        }

        if (mode == SettingsType.Sound || mode == SettingsType.None)
        {
            CreateDivider("Volume Control");
            var master = CreateSliderItem("Master", "masterVolume");
            var music = CreateSliderItem("Music", "musicVolume");
            var sound = CreateSliderItem("Sound", "soundEffectVolume");
            music.onSliderChanged += (volume) => SoundManager.Instance.SetMusicVolume(volume * .01f);
            sound.onSliderChanged += (volume) => SoundManager.Instance.SetEffectsVolume(volume * .01f);
            master.onSliderChanged += (volume) => SoundManager.Instance.SetGlobalVolume(volume * .01f);
            return;
        }

        if (mode == SettingsType.Controls)
        {
            CreateDivider("Steering");
            CreateItemPicker("Steering Mode", "steeringMode", new List<PickerItemObject>
        {
            new PickerItemObject
            {
                displayValue = "Standard",
                value = "Standard",
            },
            new PickerItemObject
            {
                displayValue = "Mouse",
                value = "Mouse",
            },
            new PickerItemObject
            {
                displayValue = "Axis",
                value = "Axis",
            }
        });
            CreateKeyBinding("Left", "leftKey");
            CreateKeyBinding("Right", "rightKey");
            CreateKeyBinding("Drift", "driftKey");
            CreateDivider("Thruster");
            CreateKeyBinding("Forward", "upKey");
            CreateKeyBinding("Brake", "downKey");
            CreateDivider("Ability");
            CreateKeyBinding("LightLance", "lightLanceKey");
            CreateKeyBinding("Overdrive", "overdriveKey");
            //CreateKeyBinding("Mining Drill", "miningDrillKey");
            //CreateDivider("Targetting");
            //CreateKeyBinding("Manual Target Key", "targetKey");
            //CreateToggleItem("Show Target Marker", "useTargetMarker");
            //CreateToggleItem("Use Auto Targetting", "useAutoTarget");
            CreateDivider("Weapon");
            CreateToggleItem("Fire with Weapon Hotkeys", "useWeaponHotKeys");
            CreateKeyBinding("Fire Weapon", "fireKey");
            CreateKeyBinding("Weapon 1", "weapon1Key");
            CreateKeyBinding("Weapon 2", "weapon2Key");
            CreateKeyBinding("Weapon 3", "weapon3Key");
            CreateKeyBinding("Weapon 4", "weapon4Key");
            CreateKeyBinding("Weapon 5", "weapon5Key");
            CreateDivider("Hotkeys");
            CreateKeyBinding("Respawn", "respawnKey");
            CreateKeyBinding("Dock", "dockKey");
            //CreateKeyBinding("Toggle Inventory", "inventoryKey");
            CreateKeyBinding("Menu", "menuKey");
            CreateKeyBinding("Player Menu", "playerMenuKey");
            CreateKeyBinding("System Map", "mapKey");
            CreateKeyBinding("Cargo Menu", "cargoKey");
            CreateKeyBinding("Self Destruct", "selfDestructKey");
            return;
        }
    }

    public void CreateToggleItem(string desc, string saveCode)
    {
        var item = Instantiate(toggleItemPrefab, contentPanel.transform);
        var script = item.GetComponent<ToggleItem>();
        script.label.text = desc;
        script.saveCode = saveCode;
        var val = bool.Parse(PlayerPrefs.GetString(saveCode, "False"));
        script.toggle.isOn = val;

        toggleOptions.Add(script);
    }

    public SliderItem CreateSliderItem(string desc, string saveCode)
    {
        var item = Instantiate(sliderItemPrefab, contentPanel.transform);
        var script = item.GetComponent<SliderItem>();
        script.label.text = desc;
        script.saveCode = saveCode;
        var val = float.Parse(PlayerPrefs.GetString(saveCode, "100"));
        script.slider.value = val;

        sliderOptions.Add(script);

        return script;
    }

    public void CreateDivider(string desc)
    {
        var item = Instantiate(dividerItemPrefab, contentPanel.transform);
        var script = item.GetComponent<DividerItem>();
        script.SetDisplay(desc);
    }

    public void CreateItemPicker(string description, string saveCode, List<PickerItemObject> items)
    {
        var item = Instantiate(pickerItemPrefab, contentPanel.transform);
        var script = item.GetComponent<PickerItem>();
        script.Initialize(description, saveCode, items);

        pickerItems.Add(script);
    }

    public void CreateKeyBinding(string desc, string saveCode)
    {
        var item = Instantiate(keyBindingPrefab, contentPanel.transform);
        var script = item.GetComponent<KeyItem>();
        script.label.text = desc;
        script.saveCode = saveCode;
        var btnText = PlayerPrefs.GetString(saveCode, "---");
        script.btnText.text = btnText;
        script.keyCode = btnText;

        keybindings.Add(script);
    }

    private bool HasChanges() 
    {
        foreach (var item in keybindings)
            if (item.keyCode == "---")
                continue;
            else
            {
                var value = PlayerPrefs.GetString(item.saveCode);
                if (value != item.keyCode)
                    return true;
            }


        foreach (var item in toggleOptions)
            if (item.Result == "---")
                continue;
            else
            {
                var value = PlayerPrefs.GetString(item.saveCode);
                if (value != item.Result)
                    return true;
            }

        foreach (var item in sliderOptions)
            if (item.Result == "---")
                continue;
            else
            {
                var value = PlayerPrefs.GetString(item.saveCode);
                if (value != item.Result)
                    return true;
            }

        foreach (var item in pickerItems)
            if (item.GetCurrentValue() == "---")
                continue;
            else
            {
                var value = PlayerPrefs.GetString(item.saveCode);
                if (value != item.GetCurrentValue())
                    return true;
            }

        return false;
    }
    public void Save()
    {

        foreach (var item in keybindings)
            if (item.keyCode == "---")
                continue;
            else
                PlayerPrefs.SetString(item.saveCode, item.keyCode);


        foreach (var item in toggleOptions)
            if (item.Result == "---")
                continue;
            else
                PlayerPrefs.SetString(item.saveCode, item.Result);

        foreach (var item in sliderOptions)
            if (item.Result == "---")
                continue;
            else
                PlayerPrefs.SetString(item.saveCode, item.Result);

        foreach (var item in pickerItems)
            if (item.GetCurrentValue() == "---")
                continue;
            else
                PlayerPrefs.SetString(item.saveCode, item.GetCurrentValue());

        PlayerPrefs.Save();

        if (_mode == SettingsType.Controls)
            Settings.Instance.RefreshControls();
        if (_mode == SettingsType.Sound)
            Settings.Instance.RefreshSound();
        if (_mode == SettingsType.Graphics)
            Settings.Instance.RefreshScreen();
        if (_mode == SettingsType.Options)
            Settings.Instance.RefreshOptions();

        _saved = true;

        Exit();
    }

    private bool _canExit = true;
    public void Exit()
    {
        if (_canExit)
        {

            _canExit = false;

            if (!_saved && HasChanges())
            {
                DialogManager.ShowDialog("Are you sure you want to close without saving?", delegate (bool confirmed)
                {
                    _canExit = true;
                    if (confirmed)
                    {
                        Settings.KeysLocked = false;
                        Destroy(transform.gameObject);
                    }
                });
            }
            else
            {
                _canExit = true;
                Settings.KeysLocked = false;
                Destroy(transform.gameObject);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(Settings.MenuKey))
            Exit();
    }
}
