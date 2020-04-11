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
    private List<Ship> _safeZonedShips = new List<Ship>();
    private ForceField _forceField;
    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.sortingOrder = SortingOrders.STATION;

        _forceField = GetComponent<ForceField>();
        _forceField.OnTriggerEnter += ForceField_OnTriggerEnter;
        _forceField.OnTriggerLeave += ForceField_OnTriggerLeave;
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

    private void ForceField_OnTriggerEnter(GameObject obj)
    {
        if (!HasSafeZone)
            return;

        var ship = obj.GetComponent<Ship>();
        if (ship)
        {
            if (!_safeZonedShips.Contains(ship))
            {
                _safeZonedShips.Add(ship);
                ship.OnEnterSafezone();
            }
        }
    }
    private void ForceField_OnTriggerLeave(GameObject obj)
    {
        if (!HasSafeZone)
            return;

        var ship = obj.GetComponent<Ship>();
        if (ship)
        {
            if (_safeZonedShips.Contains(ship))
            {
                _safeZonedShips.Remove(ship);
                ship.OnLeaveSafezone();
            }
        }
    }


}
