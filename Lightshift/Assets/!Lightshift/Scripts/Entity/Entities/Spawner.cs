using Mirror;
using System;
using System.Linq;
using UnityEngine;

[Serializable]
public struct SpawnData
{
    public NpcData npcData;
    public int maxSpawns;
    public float spawnRate;
    public float timeSinceLastSpawn { get; set; }
    public float currentSpawnCount { get; set; }
}
public class Spawner : NpcEntity
{
    public SpawnData[] spawnData;
    private new void FixedUpdate()
    {
        base.FixedUpdate();

        //if (!HasTarget || TargetDistance > 50)
        //    return;

        //if (isServer)
        //{

        //    for (int i = 0; i < spawnData.Length; i++)
        //    {
        //        if (!(spawnData[i].currentSpawnCount >= spawnData[i].maxSpawns))
        //            spawnData[i].timeSinceLastSpawn -= Time.fixedDeltaTime;

        //        TrySpawnNpcs(i);
        //    }
        //}
    }

    private void TrySpawnNpcs(int i) 
    {
        if (spawnData[i].currentSpawnCount < spawnData[i].maxSpawns && spawnData[i].timeSinceLastSpawn <= 0)
            SpawnNpc(i);
    }

    private Npc SpawnNpc(int i) 
    {
        var entity = EntityManager.GetAllNpcs().FirstOrDefault(e => e.npcData != null && e.npcData.key == spawnData[i].npcData.key && e != null && !e.alive);
        if (entity == null)
        {
            var prefab = spawnData[i].npcData.prefab;

            if (prefab == null)
                switch (spawnData[i].npcData.type) 
                {
                    case NpcType.Entity:
                        prefab = LightshiftNetworkManager.GetPrefab<NpcEntity>();
                        break;
                    case NpcType.Ship:
                        prefab = LightshiftNetworkManager.GetPrefab<NpcShip>();
                        break;
                    default:
                        prefab = LightshiftNetworkManager.GetPrefab<Npc>();
                        break;
                }
                

            var npc = Instantiate(prefab);
            npc.transform.position = transform.position;

            NetworkServer.Spawn(npc);

            //entity = npc.GetComponent<Npc>();
            //entity.mapIcon = spawnData[i].npcData.mapIcon;
        }

        entity.LoadNpcData(spawnData[i].npcData.key);
        entity.teamId = teamId;
        entity.SetSpawnLocation(transform.position);
        entity.SetPosition(transform.position);
        spawnData[i].currentSpawnCount++;
        spawnData[i].timeSinceLastSpawn = spawnData[i].spawnRate;
        entity.Respawn();
        entity.onDeath += Entity_onDeath;

        return entity;
    }

    private void Entity_onDeath(Npc npc)
    {
        npc.onDeath -= Entity_onDeath;

        // Enable Respawn.
        for (int i = 0; i < spawnData.Length; i++) 
        {
            if (spawnData[i].npcData.key == npc.npcData.key)
                spawnData[i].currentSpawnCount--;
        }
    }
}

