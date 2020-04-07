using System;

[Serializable]
public struct ModuleData
{
    public float maxShield;
    public float maxHealth;
    public float maxPower;
    public float powerRegen;
    public float shieldRegen;
    public float healthRegen;
    public float maxSpeed;
    public float acceleration;
    public float agility;
    public float weight;
    public float brakeForce;
    public float lightLancePowerCost;
    public float lightLancePullForce;
    public float lightLanceRange;
    public float overDrivePowerCost;
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