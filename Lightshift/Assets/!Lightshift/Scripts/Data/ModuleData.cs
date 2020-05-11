using System;
using UnityEngine;

[Serializable]
public struct ModuleData
{
    [Range(0, 1000)]
    public float maxShield;
    [Range(0, 1000)]
    public float maxHealth;
    [Range(0, 1000)]
    public float maxPower;
    [Range(0, 100)]
    public float powerRegen;
    [Range(0, 1000)]
    public float shieldRegen;
    [Range(0, 1000)]
    public float healthRegen;
    [Range(0, 50)]
    public float maxSpeed;
    [Range(0, 50)]
    public float acceleration;
    [Range(0, 360)]
    public float agility;
    [Range(0, 10)]
    public float weight;
    [Range(0, 10.0f)]
    public float brakeForce;
    [Range(0.02f, 3)]
    public float lightLancePowerCost;
    [Range(0, 10)]
    public float lightLancePullForce;
    [Range(5, 30)]
    public float lightLanceRange;
    [Range(0, 100)]
    public float overDrivePowerCost;
    [Range(0, 10)]
    public float overDriveBoostMultiplier;


    public static ModuleData operator +(ModuleData x, ModuleData y) 
    {
        return new ModuleData
        {
            acceleration = x.acceleration + y.acceleration,
            agility = x.agility + y.agility,
            brakeForce = x.brakeForce + y.brakeForce,
            weight = x.weight + y.weight,
            healthRegen = x.healthRegen + y.healthRegen,
            shieldRegen = x.shieldRegen + y.shieldRegen,
            powerRegen = x.powerRegen + y.powerRegen,
            maxSpeed = x.maxSpeed + y.maxSpeed,
            maxPower = x.maxPower + y.maxPower,
            maxHealth = x.maxHealth + y.maxHealth,
            maxShield = x.maxShield + y.maxShield,
            lightLancePowerCost = x.lightLancePowerCost + y.lightLancePowerCost,
            lightLancePullForce = x.lightLancePullForce + y.lightLancePullForce,
            lightLanceRange = x.lightLanceRange + y.lightLanceRange,
            overDriveBoostMultiplier = x.overDriveBoostMultiplier + y.overDriveBoostMultiplier,
            overDrivePowerCost = x.overDrivePowerCost + y.overDrivePowerCost,
        };
    }

    public static ModuleData operator -(ModuleData x, ModuleData y)
    {
        return new ModuleData
        {
            acceleration = x.acceleration - y.acceleration,
            agility = x.agility - y.agility,
            brakeForce = x.brakeForce - y.brakeForce,
            weight = x.weight - y.weight,
            healthRegen = x.healthRegen - y.healthRegen,
            shieldRegen = x.shieldRegen - y.shieldRegen,
            powerRegen = x.powerRegen - y.powerRegen,
            maxSpeed = x.maxSpeed - y.maxSpeed,
            maxPower = x.maxPower - y.maxPower,
            maxHealth = x.maxHealth - y.maxHealth,
            maxShield = x.maxShield - y.maxShield,
            lightLancePowerCost = x.lightLancePowerCost - y.lightLancePowerCost,
            lightLancePullForce = x.lightLancePullForce - y.lightLancePullForce,
            lightLanceRange = x.lightLanceRange - y.lightLanceRange,
            overDriveBoostMultiplier = x.overDriveBoostMultiplier - y.overDriveBoostMultiplier,
            overDrivePowerCost = x.overDrivePowerCost - y.overDrivePowerCost,
        };
    }

    public static ModuleData operator *(ModuleData x, ModuleData y)
    {
        return new ModuleData
        {
            acceleration = x.acceleration * y.acceleration,
            agility = x.agility * y.agility,
            brakeForce = x.brakeForce * y.brakeForce,
            weight = x.weight * y.weight,
            healthRegen = x.healthRegen * y.healthRegen,
            shieldRegen = x.shieldRegen * y.shieldRegen,
            powerRegen = x.powerRegen * y.powerRegen,
            maxSpeed = x.maxSpeed * y.maxSpeed,
            maxPower = x.maxPower * y.maxPower,
            maxHealth = x.maxHealth * y.maxHealth,
            maxShield = x.maxShield * y.maxShield,
            lightLancePowerCost = x.lightLancePowerCost * y.lightLancePowerCost,
            lightLancePullForce = x.lightLancePullForce * y.lightLancePullForce,
            lightLanceRange = x.lightLanceRange * y.lightLanceRange,
            overDriveBoostMultiplier = x.overDriveBoostMultiplier * y.overDriveBoostMultiplier,
            overDrivePowerCost = x.overDrivePowerCost * y.overDrivePowerCost,
        };
    }

    public static ModuleData operator /(ModuleData x, ModuleData y)
    {
        return new ModuleData
        {
            acceleration = x.acceleration / y.acceleration,
            agility = x.agility / y.agility,
            brakeForce = x.brakeForce / y.brakeForce,
            weight = x.weight / y.weight,
            healthRegen = x.healthRegen / y.healthRegen,
            shieldRegen = x.shieldRegen / y.shieldRegen,
            powerRegen = x.powerRegen / y.powerRegen,
            maxSpeed = x.maxSpeed / y.maxSpeed,
            maxPower = x.maxPower / y.maxPower,
            maxHealth = x.maxHealth / y.maxHealth,
            maxShield = x.maxShield / y.maxShield,
            lightLancePowerCost = x.lightLancePowerCost * y.lightLancePowerCost,
            lightLancePullForce = x.lightLancePullForce * y.lightLancePullForce,
            lightLanceRange = x.lightLanceRange * y.lightLanceRange,
            overDriveBoostMultiplier = x.overDriveBoostMultiplier * y.overDriveBoostMultiplier,
            overDrivePowerCost = x.overDrivePowerCost * y.overDrivePowerCost,
        };
    }
}