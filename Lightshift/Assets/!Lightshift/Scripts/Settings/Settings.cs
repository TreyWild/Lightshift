using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightshift
{
    public class Settings : MonoBehaviour
    {

        public static Settings Instance { get; set; }

        public KeyCode DownKey = KeyCode.S;
        public KeyCode UpKey = KeyCode.W;
        public KeyCode LeftKey = KeyCode.A;
        public KeyCode RightKey = KeyCode.D;
        public KeyCode FireKey = KeyCode.Mouse0;
        public KeyCode LightLanceKey = KeyCode.Mouse1;
        public KeyCode TargetKey = KeyCode.Mouse1;
        public KeyCode MiningDrillKey = KeyCode.X;
        public KeyCode OverdriveKey = KeyCode.C;
        public KeyCode MapKey = KeyCode.M;
        public KeyCode SettingsMenuKey = KeyCode.H;
        public KeyCode PlayerMenuKey = KeyCode.P;
        public KeyCode InventoryKey = KeyCode.Tab;
        public KeyCode ZoomOutKey = KeyCode.LeftControl;
        public KeyCode Weapon1 = KeyCode.Alpha1;
        public KeyCode Weapon2 = KeyCode.Alpha2;
        public KeyCode Weapon3 = KeyCode.Alpha3;
        public KeyCode Weapon4 = KeyCode.Alpha4;
        public KeyCode Weapon5 = KeyCode.Alpha5;
        public KeyCode RespawnKey = KeyCode.Space;
        public KeyCode WeaponMenuKey = KeyCode.X;
        public KeyCode ChatKey = KeyCode.KeypadEnter;
        public KeyCode ChatKeyAlt = KeyCode.Return;
        public KeyCode SelfDestruct = KeyCode.R;
        public KeyCode DeveloperWeaponListKey = KeyCode.K;
        public KeyCode DeveloperShipListKey = KeyCode.J;
        public bool KeysLocked;
        public bool FireWithWeaponHotkeys = false;
        public bool UseMouseAim = false;
        public bool AutoTarget = false;
        public bool ShowTargetMarker = false;
        public float soundEffectVolume = .5f;
        public float musicVolume = .5f;
        public bool IsFullscreen = false;
        public bool ShowSkybox = true;
        public bool ShowBackgroundElements;
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            else Instance = this;
        }

        void Start()
        {
            AddDefaultKey("upKey", KeyCode.UpArrow);
            AddDefaultKey("downKey", KeyCode.DownArrow);
            AddDefaultKey("leftKey", KeyCode.LeftArrow);
            AddDefaultKey("rightKey", KeyCode.RightArrow);
            //---
            AddDefaultKey("fireKey", KeyCode.Space);
            AddDefaultKey("lightLanceKey", KeyCode.F);
            AddDefaultKey("overdriveKey", KeyCode.C);
            AddDefaultKey("settingsMenuKey", KeyCode.H);
            AddDefaultKey("weaponMenuKey", KeyCode.X);
            AddDefaultKey("mapKey", KeyCode.M);
            AddDefaultKey("inventoryKey", KeyCode.Tab);
            AddDefaultKey("targetKey", KeyCode.Mouse1);
            AddDefaultKey("zoomOutKey", KeyCode.LeftControl);
            AddDefaultKey("respawnKey", KeyCode.Space);
            AddDefaultKey("weapon1Key", KeyCode.Alpha1);
            AddDefaultKey("weapon2Key", KeyCode.Alpha2);
            AddDefaultKey("weapon3Key", KeyCode.Alpha3);
            AddDefaultKey("weapon4Key", KeyCode.Alpha4);
            AddDefaultKey("weapon5Key", KeyCode.Alpha5);
            AddDefaultKey("selfDestructKey", KeyCode.R);

            if (!PlayerPrefs.HasKey("soundEffectVolume"))
                PlayerPrefs.SetFloat("soundEffectVolume", 50);

            if (!PlayerPrefs.HasKey("musicVolume"))
                PlayerPrefs.SetFloat("musicVolume", 50);

            if (!PlayerPrefs.HasKey("showSkybox"))
                PlayerPrefs.SetString("showSkybox", "True");

            if (!PlayerPrefs.HasKey("showBackgroundElements"))
                PlayerPrefs.SetString("showBackgroundElements", "True");

            RefreshControls();
        }

        private void AddDefaultKey(string key, KeyCode keyCode)
        {
            if (!PlayerPrefs.HasKey(key))
                PlayerPrefs.SetString(key, keyCode.ToString());
        }
        public void RefreshControls()
        {
            DownKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("downKey", "DownArrow"));
            UpKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("upKey", "UpArrow"));
            LeftKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("leftKey", "LeftArrow"));
            RightKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("rightKey", "RightArrow"));
            FireKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("fireKey", "Mouse0"));
            LightLanceKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("lightLanceKey", "Mouse1"));
            MiningDrillKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("miningDrillKey", "X"));
            OverdriveKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("overdriveKey", "C"));
            MapKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("mapKey", "M"));
            SettingsMenuKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("settingsMenuKey", SettingsMenuKey.ToString()));
            WeaponMenuKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("weaponMenuKey", WeaponMenuKey.ToString()));
            PlayerMenuKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("playerMenuKey", PlayerMenuKey.ToString()));
            InventoryKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("inventoryKey", InventoryKey.ToString()));
            TargetKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("targetKey", TargetKey.ToString()));
            soundEffectVolume = float.Parse(PlayerPrefs.GetString("soundEffectVolume", "50")) * .01f;
            musicVolume = float.Parse(PlayerPrefs.GetString("musicVolume", "50")) * .01f;
            ZoomOutKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("zoomOutKey", "LeftControl"));
            RespawnKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("respawnKey", "Space"));
            Weapon1 = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("weapon1Key", KeyCode.Alpha1.ToString()));
            Weapon2 = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("weapon2Key", KeyCode.Alpha2.ToString()));
            Weapon3 = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("weapon3Key", KeyCode.Alpha3.ToString()));
            Weapon4 = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("weapon4Key", KeyCode.Alpha4.ToString()));
            Weapon5 = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("weapon5Key", KeyCode.Alpha5.ToString()));
            SelfDestruct = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("selfDestructKey", SelfDestruct.ToString()));
            FireWithWeaponHotkeys = bool.Parse(PlayerPrefs.GetString("useWeaponHotKeys", "False"));
            UseMouseAim = bool.Parse(PlayerPrefs.GetString("useMouseAim", "False"));
            AutoTarget = bool.Parse(PlayerPrefs.GetString("useAutoTarget", "False"));
            ShowTargetMarker = bool.Parse(PlayerPrefs.GetString("useTargetMarker", "True"));
            IsFullscreen = bool.Parse(PlayerPrefs.GetString("isFullscreen", "True"));
            ShowSkybox = bool.Parse(PlayerPrefs.GetString("showSkybox", "True"));
            ShowBackgroundElements = bool.Parse(PlayerPrefs.GetString("showBackgroundElements", "True"));

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.MusicSource.volume = musicVolume;
                SoundManager.Instance.EffectsSource.volume = soundEffectVolume;
            }

            if (ParallaxManager.Instance != null)
            {
                ParallaxManager.Instance.ShowBackgroundObjects(ShowBackgroundElements);
                ParallaxManager.Instance.ShowSkybox(ShowSkybox);
            }
            if (!Application.isEditor)
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, IsFullscreen);
        }
    }
}