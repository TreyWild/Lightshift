using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamToTarget : MonoBehaviour
{
    private Transform _target;

    private LineRenderer _lineRenderer;

    public bool active = false;

    public float maxDistance;

    public Action<Transform, float> OnFocus;
    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.sortingOrder = SortingOrders.LIGHTLANCE;
    }

    public void TryFocusTarget(Transform target) 
    {
        if (target != null)
        {
            if (!active)
            {
                _target = target;
                active = true;
            }

            if (active)
            {
                var distance = Vector2.Distance(_target.transform.position, transform.position);

                if (distance > maxDistance)
                {
                    _lineRenderer.enabled = false;
                    return;
                }

                OnFocus?.Invoke(_target, distance);

            }
            else _lineRenderer.enabled = false;
        }      
    }

    public void CancelFocus() 
    {
        active = false;
        _lineRenderer.enabled = false;
    }

    public void TryDrawBeam() 
    {
        _lineRenderer.enabled = true;
        _lineRenderer.SetPositions(new Vector3[2] { transform.position, _target.position });
    }

    public void SetColor(Color color) 
    {
        _lineRenderer.material.color = color;
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
    }
}
