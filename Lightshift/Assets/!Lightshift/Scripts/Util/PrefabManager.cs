using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public const int RESPAWN_PREFAB_ID = 0;
    public const int INVENTORY_PREFAB_ID = 1;
    public const int PLAYER_SHIP_PREFAB_ID = 2;
    public const int NPC_SHIP_PREFAB_ID = 3;
    public const int SPAWNER_PREFAB_ID = 4;

    public GameObject shieldPrefab;
    public GameObject shipPrefab;
    public GameObject stationPrefab;
    public GameObject entityUIPrefab;
    public GameObject lightLancePrefab;
    public GameObject enginePrefab;
    public GameObject wingPrefab;
    public GameObject hullPrefab;
    public GameObject forceFieldPrefab;
    public GameObject deathEffectPrefab;
    public GameObject spawnEffectPrefab;

    public static PrefabManager Instance;

    public List<Sprite> Wings;
    public List<Sprite> Hulls;
    public List<Sprite> Stations;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(gameObject);

        
    }
}
