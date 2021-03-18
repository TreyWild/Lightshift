using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ShipDesign : MonoBehaviour
{
    [SerializeField] private DesignObject _hull;
    [SerializeField] private DesignObject _leftWing;
    [SerializeField] private DesignObject _rightWing;

    public List<Collider2D> TriggerColliders = new List<Collider2D>();
    public List<Collider2D> SolidColliders = new List<Collider2D>();
    public List<Collider2D> GenerateTriggerColliders()
    {
        TriggerColliders.Clear();
        TriggerColliders.Add(_hull.GenerateCollider());
        TriggerColliders.Add(_leftWing.GenerateCollider());
        TriggerColliders.Add(_rightWing.GenerateCollider());

        foreach (var collider in TriggerColliders)
            collider.isTrigger = true;

        return TriggerColliders;
    }

    public List<Collider2D> GenerateSolidColliders()
    {
        SolidColliders.Clear();
        SolidColliders.Add(_hull.GenerateCollider());
        SolidColliders.Add(_leftWing.GenerateCollider());
        SolidColliders.Add(_rightWing.GenerateCollider());

        foreach (var collider in SolidColliders)
            collider.isTrigger = false;

        return SolidColliders;
    }
    public void SetHull(Sprite sprite)
    {
        _hull.Forground.sprite = sprite;
        _hull.Background.sprite = sprite;

        _hull.Forground.sortingOrder = SortingOrders.SHIP_HULL;
        _hull.Background.sortingOrder = SortingOrders.SHIP_HULL-1;
    }

    public void SetWings(Sprite sprite)
    {
        _leftWing.Forground.sprite = sprite;
        _leftWing.Background.sprite = sprite;
        _leftWing.Forground.sortingOrder = SortingOrders.SHIP_WING;
        _leftWing.Background.sortingOrder = SortingOrders.SHIP_WING-1;

        _rightWing.Forground.sprite = sprite;
        _rightWing.Background.sprite = sprite;
        _rightWing.Forground.sortingOrder = SortingOrders.SHIP_WING;
        _rightWing.Background.sortingOrder = SortingOrders.SHIP_WING-1;
    }

    public void SetTopMost()
    {
        _rightWing.Forground.sortingOrder++;
        _leftWing.Forground.sortingOrder++;
        _hull.Forground.sortingOrder++;
    }
}
