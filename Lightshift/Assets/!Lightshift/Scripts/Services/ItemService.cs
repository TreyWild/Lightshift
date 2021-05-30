using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets._Lightshift.Scripts.Data;
using SharedModels.Models.Game;

public class ItemService : MonoBehaviour
{
    private static List<ModuleItem> _items;
    private static List<ResourceItem> _resourceItems;

    private static bool _inititalized = false;
    private static void Initialize() 
    {
        _items = Resources.LoadAll<ModuleItem>("").ToList();
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
        return Resources.LoadAll<PlayerDefaults>("").ToList().FirstOrDefault();
    }
}
