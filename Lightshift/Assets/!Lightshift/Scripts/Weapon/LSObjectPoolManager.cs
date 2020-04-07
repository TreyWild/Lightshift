using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LSObjectPoolManager : MonoBehaviour
{
    public static LSObjectPoolManager Instance;

    [Header("Bullet")]
    public List<Sprite> bulletImages;
    public GameObject bulletPrefab;

    [Header("Effect")]
    public List<GameObject> effectPrefabs;

    public int initialPoolSize = 50;

    public List<GameObject> pooledBullets = new List<GameObject>();
    public List<GameObject> pooledEffects = new List<GameObject>();
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < initialPoolSize; i++)
            SpawnPooledBullet(0);
    }

    #region Effect
    public GameObject SpawnPooledEffect(int effectId)
    {
        var effectObj = Instantiate(effectPrefabs[effectId]);

        var particleSystem = effectObj.GetComponent<ParticleSystem>();
        var renderer = particleSystem.GetComponent<Renderer>();

        renderer.sortingOrder = SortingOrders.EFFECT;

        effectObj.name = $"effect-[{effectId}]";

        effectObj.AddComponent<ParticleSystemAutoDestruct>();

        effectObj.SetActive(false);

        pooledEffects.Add(effectObj);

        return effectObj;
    }

    public GameObject GetUsableEffect(int id)
    {
        var obj = (from item in pooledEffects
                   where item.activeSelf == false && item.name == $"effect-[{id}]"
                   select item).FirstOrDefault();

        if (obj != null)
            return obj;

        return SpawnPooledEffect(id);
    }

    #endregion

    #region Bullet
    public GameObject SpawnPooledBullet(int imageId)
    {
        var bulletObj = Instantiate(bulletPrefab);
        var spriteRenderer = bulletObj.GetComponent<SpriteRenderer>();

        var sprite = bulletImages[imageId];
        if (sprite == null)
            return null;

        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = SortingOrders.BULLET;

        bulletObj.name = $"bullet-[{imageId}]";
        bulletObj.SetActive(false);

        var bulletScript = bulletObj.GetComponent<Bullet>();
        if (bulletScript == null)
            bulletScript = bulletObj.AddComponent<Bullet>();
        bulletScript.AddRigidBody(bulletScript.GetComponent<Rigidbody2D>());
        pooledBullets.Add(bulletObj);

        return bulletObj;
    }

    public GameObject GetUsableBullet(int id) 
    {
        var obj = (from item in pooledBullets
                   where item.activeSelf == false && item.name == $"bullet-[{id}]"
                   select item).FirstOrDefault();

        if (obj == null) {
            obj = (from item in pooledBullets
                   where item.activeSelf == false select item).FirstOrDefault();

            if (obj != null)
            {
                var spriteRenderer = obj.GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                    spriteRenderer = obj.AddComponent<SpriteRenderer>();

                var sprite = bulletImages[id];
                if (sprite == null)
                    return null;

                spriteRenderer.sprite = sprite;

                Destroy(obj.GetComponent<EdgeCollider2D>());
                obj.AddComponent<EdgeCollider2D>();

                pooledBullets.Remove(obj);
                pooledBullets.Add(obj);

                obj.gameObject.name = $"bullet-[{id}]";
            }
        }

        if (obj != null)
            return obj;

        return SpawnPooledBullet(id);
    }

    #endregion
}

