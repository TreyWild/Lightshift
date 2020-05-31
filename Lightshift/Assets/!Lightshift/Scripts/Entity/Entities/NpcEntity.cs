using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NpcEntity : Npc 
{
    private SpriteRenderer _renderer;
    private PolygonCollider2D _polyCollider;
    public void Awake()
    {
        base.Awake();

        onDataLoaded += OnInitialize;
    }

    private void OnInitialize()
    {
        _renderer = GetComponent<SpriteRenderer>();
        if (_renderer == null)
            _renderer = gameObject.AddComponent<SpriteRenderer>();
        _renderer.sortingOrder = npcData.renderOrder;
        _renderer.sprite = npcData.sprite;

        if (_polyCollider != null)
            Destroy(_polyCollider);

        _polyCollider = _renderer.gameObject.AddComponent<PolygonCollider2D>();
    }
}