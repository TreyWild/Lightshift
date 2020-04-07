
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityManager : MonoBehaviour 
{
    public static EntityManager Instance { get; set; }

    private List<Entity> _entities = new List<Entity>();
    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(gameObject);
    }

    public static void RegisterEntity(Entity entity) 
    {
        if (!Instance._entities.Contains(entity))
            Instance._entities.Add(entity);
    }

    public static void UnregisterEntity(Entity entity)
    {
        if (Instance._entities.Contains(entity))
            Instance._entities.Remove(entity);
    }

    public static List<Entity> GetAllEntities() => Instance._entities;

    public static Entity GetEntity(int id) 
    {
        return Instance._entities.FirstOrDefault(e => e.netId == id);
    }
}
