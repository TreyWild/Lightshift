using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Hull : NetworkBehaviour 
{
    [SyncVar(hook = nameof(UpdateMass))]
    public float weight;

    private Kinematic _kinematic;
    private SpriteRenderer _hull;
    private Entity _entity;
    private EntityUI _ui;
    private void Awake()
    {
        _kinematic = GetComponent<Kinematic>();
        _ui = GetComponent<EntityUI>();
        _hull = Instantiate(PrefabManager.Instance.hullPrefab, transform).GetComponent<SpriteRenderer>();
        _entity = GetComponent<Entity>();
    }

    public void SetImage(Sprite sprite, Color color = default)
    {
        if (_hull != null)
        {
            _hull.sprite = sprite;
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

    private void UpdateMass(float oldValue, float newValue)
    {
        _kinematic.mass = newValue;
    }
}