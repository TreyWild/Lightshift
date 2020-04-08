using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Hull : NetworkBehaviour 
{
    [SyncVar]
    public float healthRegen;
    [SyncVar(hook = nameof(SetUIHealthMax))]
    public float maxHealth;
    [SyncVar(hook = nameof(SetUIHealth))]
    public float health;


    private SpriteRenderer _hull;
    private Entity _entity;
    private EntityUI _ui;
    private void Awake()
    {
        _ui = GetComponent<EntityUI>();
        _hull = Instantiate(PrefabManager.Instance.hullPrefab, transform).GetComponent<SpriteRenderer>();
        _entity = GetComponent<Entity>();
    }

    public void SetImage(int id, Color color = default)
    {
        var hullSprite = PrefabManager.Instance.Hulls[id];
        if (hullSprite == null)
            return;

        if (_hull != null)
        {
            _hull.sprite = hullSprite;
            _hull.color = color;

            var collider = _hull.gameObject.GetComponent<PolygonCollider2D>();
            if (collider != null)
            {
                if (_entity.colliders.Contains(collider))
                    _entity.colliders.Remove(collider);
                Destroy(collider);
            }
            collider = _hull.gameObject.AddComponent<PolygonCollider2D>();

            _entity.colliders.Add(collider);

            if (hasAuthority)
                _hull.sortingOrder = SortingOrders.SHIP_HULL + 1;
            else _hull.sortingOrder = SortingOrders.SHIP_HULL;
        }
        else Debug.LogError("Hull not assigned.");
    }

    public void Update()
    {
        if (isServer)
        {
            health += healthRegen * Time.deltaTime;

            if (health >= maxHealth)
                health = maxHealth;
        }
    }

    private void SetUIHealthMax(float old, float newValue)
    {
        _ui.SetHealth(health, newValue);
    }

    private void SetUIHealth(float old, float newValue)
    {
        _ui.SetHealth(newValue, maxHealth);
    }

    
}