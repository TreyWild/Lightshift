using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class LSObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private GameObject _effectPrefab;

    private static LSObjectPool Instance;

    private List<Projectile> _projectiles = new List<Projectile>();
    private Transform _projectileHolder;
    private Transform _effectsHolder;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(gameObject);

        _projectileHolder = transform;
    }
    public static Projectile GetUsableProjectile()
    {
        var projectile = Instance._projectiles.FirstOrDefault(p => !p.isAlive);
        if (projectile == null)
            projectile = Instantiate(Instance._bulletPrefab, Instance._projectileHolder).GetComponent<Projectile>();

        projectile.gameObject.SetActive(true);
        return projectile;
    }

    public static GameObject GetUsableGlowEffect() => Instantiate(Instance._effectPrefab);
}
