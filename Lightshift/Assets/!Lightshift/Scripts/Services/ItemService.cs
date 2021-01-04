using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets._Lightshift.Scripts.Data;
using SharedModels.Models.Game;

public class ItemService : MonoBehaviour
{
    private static List<ModuleItem> _items;
    private static List<CargoItem> _cargo;

    private static bool _inititalized = false;
    private static void Initialize() 
    {
        _items = Resources.LoadAll<ModuleItem>("").ToList();
        _cargo = Resources.LoadAll<CargoItem>("").ToList();
        _inititalized = true;
    }

    public static ModuleItem GetItem(string id) 
    {
        if (!_inititalized)
            Initialize();

        return _items.FirstOrDefault(i => i.Id == id);
    }

    public static CargoItem GetCargoItem(CargoType type)
    {
        if (!_inititalized)
            Initialize();

        return _cargo.FirstOrDefault(i => i.Type == type);
    }
}
