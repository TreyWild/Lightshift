using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ItemService : MonoBehaviour
{
    private static List<ModuleItem> _items;

    private static bool _inititalized = false;
    private static void Initialize() 
    {
        _items = Resources.LoadAll<ModuleItem>("").ToList();
        _inititalized = true;
    }

    public static ModuleItem GetItem(string id) 
    {
        if (!_inititalized)
            Initialize();

        return _items.FirstOrDefault(i => i.Id == id);
    }
}
