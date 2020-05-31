using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class EntityManager : MonoBehaviour
{
    private static EntityManager Instance;
    private List<Entity> _entities = new List<Entity>();
    private List<NpcData> _npcData = new List<NpcData>();
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
        {
            Destroy(gameObject);
            return;
        } 
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        _npcData.Clear();
        _npcData = Resources.LoadAll<NpcData>("").ToList();
    }

    public static void AddEntity(Entity entity)
    {
        if (Server.Instance != null)
            entity.Id = (short)Instance._entities.Count;

        if (!Instance._entities.Contains(entity))
            Instance._entities.Add(entity);
    }

    public static void RemoveEntity(Entity entity)
    {
        if (Instance._entities.Contains(entity))
            Instance._entities.Remove(entity);
    }

    public static Entity GetEntity(short id) 
    {
        return Instance._entities.FirstOrDefault(e => e.Id == id);
    }

    public static List<Npc> GetAllNpcs() => Instance._entities.OfType<Npc>().ToList();

    public static List<Entity> GetAllEntities() => Instance._entities;

    public static NpcData GetEntityData(string key) => Instance._npcData.FirstOrDefault(i => i.key == key);
    public void OnLevelWasLoaded(int level)
    {
        _entities.Clear();
    }
}

