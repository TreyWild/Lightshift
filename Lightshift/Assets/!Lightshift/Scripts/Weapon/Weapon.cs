using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class Weapon
{
    public string weaponKey;
    public int slotId;
    public float coolDown = 0f;
    public bool isDisabled;

    public Entity entity;
    public WeaponData weaponData;
    public float gunSpreadInitial;
    public float gunSpacingInitial;
    public float gunSpreadIncrement;
    public float gunSpacingIncrement;
    public float aimAngle;
    public float cooldown;

    private bool _isActive;

    public virtual void Update()
    {
        if (!isDisabled)
        {
            aimAngle = entity.transform.eulerAngles.z;

            coolDown -= Time.deltaTime;
        }
    }

    public virtual void Fire()
    {
        if (_isActive)
            return;

        //_isActive = true;

        if (coolDown <= 0f)
        {
            OnWeaponFire();
            coolDown = weaponData.rateOfFire;
        }

    }

    public virtual void OnWeaponFire()
    {

    }
}

