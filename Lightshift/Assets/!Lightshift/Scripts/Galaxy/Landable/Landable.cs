using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landable : MonoBehaviour
{
    public string Id = Guid.NewGuid().ToString();

    public string DisplayName;

    private List<PlayerShip> _entities = new List<PlayerShip>();

    public LandableType Type;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var entity = collision.GetComponentInParent<PlayerShip>();
        if (entity != null)
        {
            if (!_entities.Contains(entity))
            {
                _entities.Add(entity);
                entity.EnterStationDock(this);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        var entity = collision.GetComponentInParent<PlayerShip>();
        if (entity != null)
        {
            if (_entities.Contains(entity))
            {
                _entities.Remove(entity);
                entity.LeaveStationDock();
            }
        }
    }

}
