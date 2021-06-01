using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets._Lightshift.Scripts.Data;
using SharedModels.Models.Game;

public class ItemService : MonoBehaviour
{
    private static PlayerDefaults _defaultPlayer;
    private static List<ModuleItem> _items;
    private static List<ResourceItem> _resourceItems;

    private static bool _inititalized = false;
    private static void Initialize() 
    {
        _items = Resources.LoadAll<ModuleItem>("").ToList();
        _defaultPlayer = Resources.LoadAll<PlayerDefaults>("").ToList().FirstOrDefault();
        foreach (var item in _items) 
        {
            switch (item.Type)
            {
                case ItemType.Wing:
                    item.InstallLocations = new List<ModuleLocation>();
                    item.InstallLocations.Add(ModuleLocation.PrimaryWings);
                    item.InstallLocations.Add(ModuleLocation.SecondaryWings);
                    break;
                case ItemType.Weapon:
                    item.InstallLocations = new List<ModuleLocation>();
                    item.InstallLocations.Add(ModuleLocation.Weapon1);
                    item.InstallLocations.Add(ModuleLocation.Weapon2);
                    item.InstallLocations.Add(ModuleLocation.Weapon3);
                    item.InstallLocations.Add(ModuleLocation.Weapon4);
                    item.InstallLocations.Add(ModuleLocation.Weapon5);
                    break;
                case ItemType.Hull:
                    item.InstallLocations = new List<ModuleLocation>();
                    item.InstallLocations.Add(ModuleLocation.Hull);
                    break;

                case ItemType.Engine:
                    item.InstallLocations = new List<ModuleLocation>();
                    item.InstallLocations.Add(ModuleLocation.Engine);
                    break;
            }
        }
        _resourceItems = Resources.LoadAll<ResourceItem>("").ToList();
        _inititalized = true;
    }

    public static ModuleItem GetItem(string id) 
    {
        if (!_inititalized)
            Initialize();

        return _items.FirstOrDefault(i => i.Id == id);
    }

    public static ResourceItem GetResourceItem(ResourceType type)
    {
        if (!_inititalized)
            Initialize();

        return _resourceItems.FirstOrDefault(i => i.Type == type);
    }

    public static List<ResourceItem> GetAllResourceItems()
    {
        if (!_inititalized)
            Initialize();

        return _resourceItems;
    }

    public static PlayerDefaults GetPlayerDefaults() 
    {
        if (!_inititalized)
            Initialize();

        return _defaultPlayer;
    }
}
