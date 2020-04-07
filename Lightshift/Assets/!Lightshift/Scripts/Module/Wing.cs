using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Wing : NetworkBehaviour 
{
    private Rigidbody2D _rigidBody;
    private SpriteRenderer[] _wings;
    private Entity _entity;

    [SyncVar]
    public float agility;
    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        var wings = Instantiate(PrefabManager.Instance.wingPrefab, transform);
        _wings = wings.GetComponentsInChildren<SpriteRenderer>();
        _entity = GetComponent<Entity>();

        SetImage(0);
    }

    public void Turn(int axis)
    {
        if (isServer)
            _rigidBody.rotation += -axis * agility * Time.deltaTime;
    }

    public void SetImage(int id, Color color = default)
    {
        if (_wings == null)
            return;

        for (int i = 0; i < _wings.Length; i++)
        {
            var wing = _wings[i];
            if (wing == null)
                continue;

            var wingSprite = PrefabManager.Instance.Wings[id];
            if (wingSprite == null)
                continue;

            wing.sprite = wingSprite;
            wing.color = color;

            var collider = wing.gameObject.GetComponent<PolygonCollider2D>();
            if (collider != null)
            {
                if (_entity.colliders.Contains(collider))
                    _entity.colliders.Remove(collider);
                Destroy(collider);
            }
            collider = wing.gameObject.AddComponent<PolygonCollider2D>();

            _entity.colliders.Add(collider);

            if (hasAuthority)
                wing.sortingOrder = SortingOrders.SHIP_WING + 1;
            else wing.sortingOrder = SortingOrders.SHIP_WING;
        }
    }
}