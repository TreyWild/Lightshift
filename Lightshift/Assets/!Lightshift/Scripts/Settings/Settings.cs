using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightshift
{
    public class Settings : MonoBehaviour
    {
        public const int SETTINGS_VERSION = 5;
        public static Settings Instance { get; set; }

        public static KeyCode DownKey = KeyCode.DownArrow;
        public static KeyCode UpKey = KeyCode.UpArrow;
        public static KeyCode LeftKey = KeyCode.LeftArrow;
        public static KeyCode RightKey = KeyCode.RightArrow;
        public static KeyCode FireKey = KeyCode.Space;
        public static KeyCode DockKey = KeyCode.Space;
        public static KeyCode LightLanceKey = KeyCode.F;
        public static KeyCode MiningDrillKey = KeyCode.X;
        public static KeyCode OverdriveKey = KeyCode.C;
        public static KeyCode DriftKey = KeyCode.D;
        public static KeyCode MapKey = KeyCode.M;
        public static KeyCode MenuKey = KeyCode.Escape;
        public static KeyCode PlayerMenuKey = KeyCode.P;
        public static KeyCode InventoryKey = KeyCode.Tab;
        public static KeyCode ZoomOutKey = KeyCode.LeftControl;
        public static KeyCode Weapon1 = KeyCode.Alpha1;
        public static KeyCode Weapon2 = KeyCode.Alpha2;
        public static KeyCode Weapon3 = KeyCode.Alpha3;
        public static KeyCode Weapon4 = KeyCode.Alpha4;
        public static KeyCode Weapon5 = KeyCode.Alpha5;
        public static KeyCode RespawnKey = KeyCode.Space;
        public static KeyCode ChatKey = KeyCode.KeypadEnter;
        public static KeyCode ChatKeyAlt = KeyCode.Return;
        public static KeyCode SelfDestruct = KeyCode.R;
        public static KeyCode CargoMenu = KeyCode.V;
        public static KeyCode DeveloperWeaponListKey = KeyCode.K;
        public static KeyCode DeveloperShipListKey = KeyCode.J;
        public static bool KeysLocked;
        public static bool FireWithWeaponHotkeys = true;
        public static bool AutoTarget = false;
        public static bool ShowTargetMarker = false;
        public static float soundEffectVolume = .5f;
        public static float musicVolume = .5f;
        //public bool IsFullscreen = false;
        public bool ShowSkybox = true;
        public bool ShowBackgroundElements;
        public bool ShowDebugStats;
        public FullScreenMode fullScreenMode;
        public Vector2 resolution;
        public int MaxFrameRate = 60;
        public static bool ShowDamageText;

        public static SteeringMode Steering;
        public enum SteeringMode { Standard, Mouse, Axis }
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            else Instance = this;

            DontDestroyOnLoad(gameObject);

            if (PlayerPrefs.GetInt("settingsVersion", 0) != SETTINGS_VERSION)
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.SetInt("settingsVersion", SETTINGS_VERSION);
            }

            if (!PlayerPrefs.HasKey("soundEffectVolume"))
                PlayerPrefs.SetString("soundEffectVolume", "50");

            if (!PlayerPrefs.HasKey("musicVolume"))
                PlayerPrefs.SetString("musicVolume", "50");

            if (!PlayerPrefs.HasKey("showSkybox"))
                PlayerPrefs.SetString("showSkybox", "True");

            if (!PlayerPrefs.HasKey("showBackgroundElements"))
                PlayerPrefs.SetString("showBackgroundElements", "True");

            if (!PlayerPrefs.HasKey("showDebugStats"))
                PlayerPrefs.SetString("showDebugStats", "False");

            if (!PlayerPrefs.HasKey("useWeaponHotKeys"))
                PlayerPrefs.SetString("useWeaponHotKeys", "False");

            if (PlayerPrefs.HasKey("showDamageText"))
                PlayerPrefs.SetString("showDamageText", "True");

            PlayerPrefs.Save();

            RefreshControls();
            RefreshScreen();
            RefreshSound();
            RefreshOptions();
        }

        private KeyCode GetControlValue(string key, KeyCode defaultValue = default)
        {
            if (!PlayerPrefs.HasKey(key))
                PlayerPrefs.SetString(key, defaultValue.ToString());
            
            return (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(key));
        }
        public void RefreshControls()
        {
            DownKey = GetControlValue("downKey", DownKey);
            UpKey = GetControlValue("upKey", UpKey);
            LeftKey = GetControlValue("leftKey", LeftKey);
            RightKey = GetControlValue("rightKey", RightKey);
            FireKey = GetControlValue("fireKey", FireKey);
            LightLanceKey = GetControlValue("lightLanceKey", LightLanceKey);
            MiningDrillKey = GetControlValue("miningDrillKey", MiningDrillKey);
            OverdriveKey = GetControlValue("overdriveKey", OverdriveKey);
            MapKey = GetControlValue("mapKey", MapKey);
            DriftKey = GetControlValue("driftKey", DriftKey);
            MenuKey = GetControlValue("menuKey", MenuKey);
            PlayerMenuKey = GetControlValue("playerMenuKey", PlayerMenuKey);
            InventoryKey = GetControlValue("inventoryKey", InventoryKey);
            ZoomOutKey = GetControlValue("zoomOutKey", ZoomOutKey);
            RespawnKey = GetControlValue("respawnKey", RespawnKey);
            Weapon1 = GetControlValue("weapon1Key", Weapon1);
            Weapon2 = GetControlValue("weapon2Key", Weapon2);
            Weapon3 = GetControlValue("weapon3Key", Weapon3);
            Weapon4 = GetControlValue("weapon4Key", Weapon4);
            Weapon5 = GetControlValue("weapon5Key", Weapon5);
            CargoMenu = GetControlValue("cargoMenu", CargoMenu);
            DockKey = GetControlValue("dockKey", DockKey);
            SelfDestruct = GetControlValue("selfDestructKey", SelfDestruct);
            FireWithWeaponHotkeys = bool.Parse(PlayerPrefs.GetString("useWeaponHotKeys", "True"));
            AutoTarget = bool.Parse(PlayerPrefs.GetString("useAutoTarget", "False"));
            ShowTargetMarker = bool.Parse(PlayerPrefs.GetString("useTargetMarker", "True"));
            ShowSkybox = bool.Parse(PlayerPrefs.GetString("showSkybox", "True"));
            ShowBackgroundElements = bool.Parse(PlayerPrefs.GetString("showBackgroundElements", "True"));
            Steering = (SteeringMode)Enum.Parse(typeof(SteeringMode), PlayerPrefs.GetString("steeringMode", "Standard"));
        }

        public void RefreshScreen()
        {
            var oldRes = resolution;
            var oldFsM = fullScreenMode;

            for (int i = 0; i < QualitySettings.names.Length; i++)
            {
                var name = QualitySettings.names[i];
                var quality = PlayerPrefs.GetString("gameQuality", "Fantastic");
                if (name.ToLower() == quality.ToLower())
                {
                    QualitySettings.SetQualityLevel(i);
                    break;
                }

            }

            fullScreenMode = (FullScreenMode)Enum.Parse(typeof(FullScreenMode), PlayerPrefs.GetString("screenMode", "Windowed"));

            var oldFrameRate = MaxFrameRate;
            MaxFrameRate = int.Parse(PlayerPrefs.GetString("frameRate", "60"));

            string[] split = PlayerPrefs.GetString("resolution", $"{Screen.currentResolution.width}:{Screen.currentResolution.height}").Split(':');
            int width = int.Parse(split[0]);
            int height = int.Parse(split[1]);

            resolution = new Vector2(width, height);
            if (oldRes == resolution)
            {
                resolution.x = Screen.currentResolution.width;
                resolution.y = Screen.currentResolution.height;
            }

            if (resolution.x < 840)
                resolution.x = 840;
            if (resolution.y < 680)
                resolution.y = 680;

            if (oldRes != resolution || oldFsM != fullScreenMode || MaxFrameRate != oldFrameRate) 
            {
                Screen.SetResolution((int)resolution.x, (int)resolution.y, fullScreenMode, MaxFrameRate);
            }


            var backgrounds = (ShowBackgroundElements) || ShowSkybox;
            
            if (backgrounds)
                RefreshBackgrounds();
        }

        public void RefreshBackgrounds() 
        {
            if (ParallaxManager.Instance == null)
                return;

            ParallaxManager.Instance.ShowBackgroundObjects(ShowBackgroundElements);
            ParallaxManager.Instance.ShowSkybox(ShowSkybox);
        }

        public void RefreshOptions() 
        {
            ShowDamageText = bool.Parse(PlayerPrefs.GetString("showDamageText", "True"));

            ShowDebugStats = bool.Parse(PlayerPrefs.GetString("showDebugStats", "False"));

            if (GameUIManager.Instance != null)
                GameUIManager.Instance.ShowScreenStats(ShowDebugStats);
        }

        public void RefreshSound() 
        {
            soundEffectVolume = float.Parse(PlayerPrefs.GetString("soundEffectVolume", "50")) * .01f;
            musicVolume = float.Parse(PlayerPrefs.GetString("musicVolume", "30")) * .01f;

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.UpdateVolume();
            }
        }
    }
}