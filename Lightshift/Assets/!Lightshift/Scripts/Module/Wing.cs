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

    private Sprite _baseImage;
    private void Awake()
    {
        _kinematic = GetComponent<Kinematic>();
        _input = GetComponent<PlayerController>();

        var wings = Instantiate(PrefabManager.Instance.wingPrefab, transform);
        _wings = wings.GetComponentsInChildren<SpriteRenderer>();
        _entity = GetComponent<Entity>();

        _baseImage = _wings[0].sprite;
    }
    public void Turn(int axis)
    {
        if (hasAuthority)
        {

        }
    }

    public bool AxisAlignedAim()
    {
        return true;
        //if (hasAuthority)
        //{
        //    float aimAngle = _kinematic.rotation;
        //    bool aiming = true;
        //    if (_input.Up)
        //    {
        //        if (_input.Left)
        //            aimAngle = 45;
        //        else if (_input.Right)
        //            aimAngle = -45;
        //        else
        //            aimAngle = 0;
        //    }
        //    else if (_input.Down)
        //    {
        //        if (_input.Left)
        //            aimAngle = 135;
        //        else if (_input.Right)
        //            aimAngle = -135;
        //        else
        //            aimAngle = 180;
        //    }
        //    else if (_input.Left)
        //        aimAngle = 90;
        //    else if (_input.Right)
        //        aimAngle = -90;
        //    else
        //        aiming = false;

        //    float invSpeedPercent;
        //    if (aiming)
        //        invSpeedPercent = Mathf.Max(1 - (_kinematic.velocity.magnitude / _engine.maxSpeed) * 0.33f, 0); //66% turnspeed at max flight speed, if a movement key is down
        //    else
        //        invSpeedPercent = Mathf.Max(1 - (_kinematic.velocity.magnitude / _engine.maxSpeed) * 0.25f, 0); //75% turnspeed at max flight speed
        //    invSpeedPercent *= agility * Time.fixedDeltaTime;

        //    float angleDiff = (_kinematic.rotation - aimAngle) % 360;
        //    if (angleDiff > 180)
        //        angleDiff -= 360;
        //    else if (angleDiff < -180)
        //        angleDiff += 360;

        //    Debug.LogError(angleDiff);

        //    if (angleDiff > invSpeedPercent)
        //    {
        //        _kinematic.SetDirection(_kinematic.transform.eulerAngles.z - invSpeedPercent);
        //    }
        //    else if (angleDiff < -invSpeedPercent)
        //    {
        //        _kinematic.SetDirection(_kinematic.transform.eulerAngles.z + invSpeedPercent);
        //    }
        //    else
        //    {
        //        _kinematic.SetDirection(aimAngle);
        //    }

        //    return aiming /*&& Mathf.Abs(angleDiff) < 45*/;
        //}
        //return false;
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

    public void SetSortingOrder(int i)
    {
        foreach (var wing in _wings) 
        {
            wing.sortingOrder = i - 1;
        }
    }
}