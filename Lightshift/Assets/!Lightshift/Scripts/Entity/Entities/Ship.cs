using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(EntityUI), typeof(Heart), typeof(Shield))]
public class Ship : Entity
{
    [HideInInspector]
    public ModuleData stats;

    [HideInInspector]
    public LightLance lightLance;
    [HideInInspector]
    public Engine engine;
    [HideInInspector]
    public Wing wing;
    [HideInInspector]
    public Hull hull;
    [HideInInspector]
    public Heart heart;
    [HideInInspector]
    public Shield shield;
    [HideInInspector]
    public Generator generator;
    [HideInInspector]
    public WeaponSystem weaponSystem;
    public void Awake()
    {
        base.Awake();

        lightLance = GetComponent<LightLance>();
        engine = GetComponent<Engine>();
        wing = GetComponent<Wing>();
        hull = GetComponent<Hull>();
        shield = GetComponent<Shield>();
        heart = GetComponent<Heart>();
        generator = GetComponent<Generator>();
        weaponSystem = GetComponent<WeaponSystem>();
        kinematic = GetComponent<Kinematic>();
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


    public void UpdateStats(bool refill = false)
    {
        if (engine != null)
        {
            engine.maxSpeed = stats.maxSpeed;
            engine.acceleration = stats.acceleration;
            engine.brakeForce = stats.brakeForce;
            engine.overDriveMultiplier = stats.overDriveBoostMultiplier;
            engine.overDrivePowerCost = stats.overDrivePowerCost;
        }

        if (hull != null)
            hull.weight = stats.weight;

        if (wing != null)
            wing.agility = stats.agility;

        heart.SetMaxHealth(stats.maxHealth);
        heart.healthRegen = stats.healthRegen;

        shield.SetMaxShield(stats.maxShield);
        shield.shieldRegen = stats.shieldRegen;

        if (generator != null)
        {
            generator.maxPower = stats.maxPower;
            generator.powerRegen = stats.powerRegen;
        }
        if (lightLance != null)
        {
            lightLance.SetRange(stats.lightLanceRange);
            lightLance.pullForce = stats.lightLancePullForce;
            lightLance.powerCost = stats.lightLancePowerCost;
        }

        if (refill)
        {
            heart.health = stats.maxHealth - 1;
            shield.shield = stats.maxShield - 1;

            if (generator != null)
            generator.power = stats.maxPower - 1;
        }
    }

}

