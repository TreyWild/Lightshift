using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : Landable
{
    [Header("Image")]
    [SerializeField] private Sprite _stationSprite;
    [Header("Blink image")]
    [SerializeField] private Sprite _blinkSprite;

    public bool defaultStation;

    private SpriteRenderer _renderer;
    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.sprite = _stationSprite;
        _renderer.sortingOrder = SortingOrders.STATION;
    }

    private float _nextBlinkTime = 0;
    private float _timeSinceLastBlink = 0;
    private void FixedUpdate()
    {
        if (_blinkSprite == null)
            return;

        _timeSinceLastBlink += Time.deltaTime;
        if (_timeSinceLastBlink > _nextBlinkTime)
        {
            _nextBlinkTime = Random.Range(0.5f, 10f);
            _timeSinceLastBlink = 0;

            if (_renderer.sprite == _blinkSprite)
                _renderer.sprite = _stationSprite;
            else _renderer.sprite = _blinkSprite;
        }
    }

}
