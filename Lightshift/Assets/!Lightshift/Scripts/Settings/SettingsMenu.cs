using Lightshift;
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

    private bool _saved = false;
    private bool _fullScreen;
    void Start()
    {
        _saveButton.onClick.AddListener(delegate
        {
            Save();
        });
        Settings.Instance.KeysLocked = true;
        _fullScreen = Settings.Instance.IsFullscreen;

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

        CreateItemPicker("Fullscreen Mode", "fullscreenMode", new List<PickerItemObject>
        {
            new PickerItemObject
            {
                displayValue = "Fullscreen",
                value = "ExclusiveFullScreen",
            },
            new PickerItemObject
            {
                displayValue = "Fullscreen Window",
                value = "FullScreenWindow",
            },
            new PickerItemObject
            {
                displayValue = "MaximizedWindow",
                value = "MaximizedWindow",
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

        CreateItemPicker("Resolution", "gameResulotion", resolutionItems);

 
        CreateToggleItem("Use fullscreen", "isFullscreen");
        CreateToggleItem("Show Skybox", "showSkybox");
        CreateToggleItem("Background Objects", "showBackgroundElements");

        CreateDivider("Utility");
        CreateToggleItem("Show Debug Stats", "showDebugStats");
        CreateItemPicker("Camera Mode", "immersiveCamera", new List<PickerItemObject>
        {
            new PickerItemObject
            {
                displayValue = "Normal",
                value = "False",
            },
            new PickerItemObject
            {
                displayValue = "Immersive",
                value = "True",
            }
        });

        CreateDivider("Volume Control");
        CreateSliderItem("Music Volume", "musicVolume");
        CreateSliderItem("Sound Volume", "soundEffectVolume");
        CreateDivider("Rotation");
        CreateToggleItem("Use Mouse Aim", "useMouseAim");
        CreateKeyBinding("Left", "leftKey");
        CreateKeyBinding("Right", "rightKey");
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
        CreateKeyBinding("Toggle Inventory", "inventoryKey");
        CreateKeyBinding("Settings Menu", "settingsMenuKey");
        CreateKeyBinding("System Map", "mapKey");
        CreateKeyBinding("Zoom Out", "zoomOutKey");
        CreateKeyBinding("Self Destruct", "selfDestructKey");

        Settings.Instance.KeysLocked = true;
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

    public void CreateSliderItem(string desc, string saveCode)
    {
        var item = Instantiate(sliderItemPrefab, contentPanel.transform);
        var script = item.GetComponent<SliderItem>();
        script.label.text = desc;
        script.saveCode = saveCode;
        var val = float.Parse(PlayerPrefs.GetString(saveCode, "100"));
        script.slider.value = val;

        sliderOptions.Add(script);
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

        Settings.Instance.RefreshControls();
        Settings.Instance.RefreshBackgrounds();
        Settings.Instance.RefreshScreen();
        Settings.Instance.RefreshCameraMode();

        _saved = true;
        Exit();
    }

    public void Exit()
    {
        if (!_saved && HasChanges())
        {
            DialogManager.ShowDialog("Are you sure you want to close without saving?", delegate (bool confirmed)
            {
                if (confirmed)
                {
                    Settings.Instance.KeysLocked = false;
                    Destroy(transform.gameObject);
                }
            });
        }
        else
        {
            Settings.Instance.KeysLocked = false;
            Destroy(transform.gameObject);
        }
    }
}
