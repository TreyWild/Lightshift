using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private List<Entity> _entities = new List<Entity>();

    public string Id { get; set; } = Guid.NewGuid().ToString();

    private ForceField _forceField;
    private void Start()
    {
        _forceField = gameObject.AddComponent<ForceField>();
        CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
        collider.radius = 12.5f;
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
