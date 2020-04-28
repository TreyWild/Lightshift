using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ItemManager : MonoBehaviour
{
    private static ItemManager Instance;
    private List<Item> _items = new List<Item>();
    private void Awake()
    {
        if (Instance == null)
            Instance = this;  
        else { 
            Destroy(gameObject);
            return;
        }

        _items = Resources.LoadAll<Item>("").ToList();
    }

    public static Item GetItem(string key) => Instance._items.FirstOrDefault(i => i.key == key);
    public static Starship GetStarship(string key) => Instance._items.FirstOrDefault(i => i.key == key) as Starship;
    public static Weapon GetWeapon(string key) => Instance._items.FirstOrDefault(i => i.key == key) as Weapon;
}
