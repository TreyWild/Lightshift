using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private List<Entity> _entities = new List<Entity>();

    public string Id { get; set; } = Guid.NewGuid().ToString();

    private ForceField _forceField;

    private void OnDestroy()
    {
        Id = null;
        _forceField = null;
        _entities.Clear();
        _entities = null;
    }

    private void Start()
    {
        _forceField = gameObject.AddComponent<ForceField>();
        CircleCollider2D collider = gameObject.GetComponent<CircleCollider2D>();
        collider.radius = 7f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var entity = collision.GetComponentInParent<Entity>();
        if (entity)
        {
            if (!_entities.Contains(entity))
            {
                _entities.Add(entity);
                entity.EnterCheckpoint(this);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        var entity = collision.GetComponentInParent<Entity>();
        if (entity)
        {
            if (_entities.Contains(entity))
            {
                _entities.Remove(entity);
                entity.LeaveCheckpoint(this);
            }
        }
    }
}
