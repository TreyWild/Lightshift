using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Wing : NetworkBehaviour 
{
    [SyncVar]
    public float agility;

    private Kinematic _kinematic;
    private SpriteRenderer[] _wings;
    private Entity _entity;
    private PlayerController _input;
    private Engine _engine;

    private Sprite _baseImage;
    private void Awake()
    {
        _kinematic = GetComponent<Kinematic>();
        _input = GetComponent<PlayerController>();
        _engine = GetComponent<Engine>();

        var wings = Instantiate(PrefabManager.Instance.wingPrefab, transform);
        _wings = wings.GetComponentsInChildren<SpriteRenderer>();
        _entity = GetComponent<Entity>();

        _baseImage = _wings[0].sprite;
    }
    public void Turn(int axis)
    {
        if (hasAuthority)
        {
            float invSpeedPercent;
            if (_input.Up)
                invSpeedPercent = Mathf.Max(1 - (_kinematic.velocity.magnitude / _engine.maxSpeed) * 0.33f, 0); //75% turnspeed at max speed
            else
                invSpeedPercent = Mathf.Max(1 - (_kinematic.velocity.magnitude / _engine.maxSpeed) * 0.25f, 0); //50% turnspeed at max speed

            _kinematic.SetDirection(_kinematic.transform.eulerAngles.z + agility * axis * Time.fixedDeltaTime * invSpeedPercent);
        }
    }

    public void SetImage(Sprite sprite, Color color = default)
    {
        if (sprite == null)
            sprite = _baseImage;

        if (_wings == null)
            return;

        for (int i = 0; i < _wings.Length; i++)
        {
            var wing = _wings[i];
            if (wing == null)
                continue;

            wing.sprite = sprite;
            wing.color = color;

            var collider = wing.gameObject.GetComponent<PolygonCollider2D>();

            if (collider != null)
                Destroy(collider);
            
            collider = wing.gameObject.AddComponent<PolygonCollider2D>();
            collider.isTrigger = true;

            if (hasAuthority)
                wing.sortingOrder = SortingOrders.SHIP_WING + 1;
            else wing.sortingOrder = SortingOrders.SHIP_WING;
        }
    }
}