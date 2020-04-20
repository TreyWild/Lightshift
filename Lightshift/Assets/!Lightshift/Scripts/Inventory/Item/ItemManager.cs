using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ItemManager : MonoBehaviour
{
    private static ItemManager Instance;
    [SerializeField] private List<Item> _items = new List<Item>();
    private void Awake()
    {
        if (Instance == null)
            Instance = this;  
        else { 
            Destroy(gameObject);
            return;
        }
    }

    public static Item GetItem(string key) => Instance._items.FirstOrDefault(i => i.key == key);
}
