using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : Entity
{
    [Header("Requires ForceField Componant")]
    public bool HasSafeZone = true;

    [Header("Normal Image")]
    public Sprite stationImage;

    [Header("Blink Image")]
    public Sprite stationBlinkImage;


    public StationType stationType;
    private SpriteRenderer _renderer;
    private List<PlayerShip> _safeZonedShips = new List<PlayerShip>();
    private ForceField _forceField;
    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.sortingOrder = SortingOrders.STATION;

        _forceField = GetComponent<ForceField>();
    }

    private float _nextBlinkTime = 0;
    private float _timeSinceLastBlink = 0;
    private void FixedUpdate()
    {
        _timeSinceLastBlink += Time.deltaTime;
        if (_timeSinceLastBlink > _nextBlinkTime) 
        {
            _nextBlinkTime = Random.Range(0.5f, 10f);
            _timeSinceLastBlink = 0;

            if (_renderer.sprite == stationBlinkImage)
                _renderer.sprite = stationImage;
            else _renderer.sprite = stationBlinkImage;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!HasSafeZone)
            return;

        var ship = collision.GetComponentInParent<PlayerShip>();
        if (ship)
        {
            if (!_safeZonedShips.Contains(ship))
            {
                _safeZonedShips.Add(ship);
                ship.OnEnterSafezone(this);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!HasSafeZone)
            return;

        var ship = collision.GetComponentInParent<PlayerShip>();
        if (ship)
        {
            if (_safeZonedShips.Contains(ship))
            {
                _safeZonedShips.Remove(ship);
                ship.OnLeaveSafezone(this);
            }
        }
    }



}
