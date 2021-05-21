using Lightshift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MainMenu : MonoBehaviour 
{
    [SerializeField] private GameObject _settingsMenuPrefab;
    [SerializeField] private GameObject _menuPrefab;
    private static MainMenu Instance { get; set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (!Settings.KeysLocked)
            if (Input.GetKeyDown(Settings.MenuKey))
                SetActive(true);
    }

    public void HideMainMenu() 
    {
        SetActive(false);
    }

    private GameObject _settingsGameobject;
    private GameObject _menuGameobject;
    public static void ShowSettings(SettingsMenu.SettingsType type = default) 
    {
        if (Instance._settingsGameobject != null)
        {
            Destroy(Instance._settingsGameobject);
        }
        else Instance._settingsGameobject = Instantiate(Instance._settingsMenuPrefab);

        var settings = Instance._settingsGameobject.GetComponent<SettingsMenu>();

        settings.Init(type);
    }

    public static void SetActive(bool active) 
    {
        if (Instance != null && Instance._menuGameobject != null)
            Instance._menuGameobject.SetActive(active);
        else if (active)
            Instance._menuGameobject = Instantiate(Instance._menuPrefab);
    }


}
