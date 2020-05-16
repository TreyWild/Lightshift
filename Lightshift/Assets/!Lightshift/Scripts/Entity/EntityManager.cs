using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class EntityManager : MonoBehaviour
{
    private static EntityManager Instance;
    private List<Entity> _entities = new List<Entity>();
    public static int EntityCount 
    {
        get 
        {
            if (Instance == null || Instance._entities == null)
                return 0;
            else return Instance._entities.Count;
        }
    }
    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);     
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public static void AddEntity(Entity entity)
    {
        if (Server.Instance != null)
            entity.Id = Guid.NewGuid().ToString();

        if (!Instance._entities.Contains(entity))
            Instance._entities.Add(entity);
    }

    public static void RemoveEntity(Entity entity)
    {
        if (Instance._entities.Contains(entity))
            Instance._entities.Remove(entity);
    }

    public static Entity GetEntity(string id) 
    {
        return Instance._entities.FirstOrDefault(e => e.Id == id);
    }

    public static List<Entity> GetAllEntities() => Instance._entities;

    public void OnLevelWasLoaded(int level)
    {
        _entities.Clear();
    }
}

