using SharedModels.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class Ship : Entity
{
    public Thruster thruster;
    public ShipDesign design;
    public WeaponSystem weaponSystem;
    public Generator generator;

    [Header("Stats")]
    public float acceleration;
    public float brakeForce;
    public float speed;
    public float overDriveBoost;
    public float agility;

    public void Awake()
    {
        base.Awake();

        onModifierChanged += OnModifierChanged;

        if (generator == null)
            generator = GetComponent<Generator>();

        if (weaponSystem == null)
            weaponSystem = gameObject.AddComponent<WeaponSystem>();

        onCleanup += () =>
        {
            thruster = null;
            design = null;
            weaponSystem = null;
            generator = null;
        };
    }

    public void Brake()
    {

        float engineStr = acceleration * Time.fixedDeltaTime;

        engineStr *= brakeForce / kinematic.mass; //this uses mass
        if (kinematic.velocity.sqrMagnitude > engineStr * engineStr)
            kinematic.velocity -= engineStr * kinematic.velocity.normalized;
        else kinematic.velocity = Vector2.zero;

    }

    public void Thrust(bool overDrive = false)
    {

        float engineStr = acceleration * Time.fixedDeltaTime;
        if (overDrive)
            engineStr *= overDriveBoost;

        var maxSpeed = speed;
        if (overDrive)
            maxSpeed *= overDriveBoost;

        if (overDrive && kinematic.velocity.sqrMagnitude > maxSpeed * maxSpeed) //over max speed but boosting
        {
            float speed = kinematic.velocity.magnitude;
            kinematic.AddForce(transform.up * engineStr / 2);
            kinematic.velocity *= Mathf.Max(speed - engineStr / kinematic.mass / 2, 0) / speed;
        }
        else if (kinematic.velocity.sqrMagnitude > maxSpeed * maxSpeed) //over max speed, not boosting
        {
            float speed = kinematic.velocity.magnitude;
            kinematic.AddForce(transform.up * engineStr / 2);
            kinematic.velocity *= Mathf.Max(speed - engineStr / kinematic.mass / 2, 0) / speed;
        }
        else //under max speed
        {
            kinematic.AddForce(transform.up * engineStr);
        }


        if (!overDrive)
            kinematic.drag = 0.99f;
        else
            kinematic.drag = 0.999f;

    }

    public void Turn(int axis, bool thrusting = false) 
    {
        float invSpeedPercent;
        if (thrusting)
            invSpeedPercent = Mathf.Max(1 - (kinematic.velocity.magnitude / speed) * 0.33f, 0); //75% turnspeed at max speed
        else
            invSpeedPercent = Mathf.Max(1 - (kinematic.velocity.magnitude / speed) * 0.4f, 0); //50% turnspeed at max speed

        kinematic.SetDirection(kinematic.transform.eulerAngles.z + agility * axis * Time.fixedDeltaTime * invSpeedPercent);
    }

    private void OnModifierChanged(Modifier type, float value)
    {
        //if (isServer)
        //    Modifiers[type] = value;

        switch (type)
        {
            case Modifier.Weight:
                kinematic.mass = value;
                break;
            case Modifier.Acceleration:
                acceleration = value;
                break;
            case Modifier.Speed:
                speed = value;
                break;
            case Modifier.OverdriveBoost:
                overDriveBoost = value;
                break;
            case Modifier.BrakeForce:
                brakeForce = value;
                break;
            case Modifier.Agility:
                agility = value;
                break;
        }
    }

    public void SetWings(Sprite wings) 
    {
        design.SetWings(wings);
    }

    public void SetHull(Sprite hull)
    {
        design.SetHull(hull);
    }
    public override void OnDeath()
    {
        base.OnDeath();

        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        // Disable children
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).gameObject;
            if (child != null)
            {          
                child.SetActive(false);
            }
        }
    }

    public override void OnRespawn()
    {
        base.OnRespawn();

        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).gameObject;
            if (child != null)
                child.SetActive(true);
        }
    }

    public void SetThrusterColor(Color color)
    {
        thruster.SetColor(color);
    }
}

