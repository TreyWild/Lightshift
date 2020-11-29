using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Linq;
using Mirror;
using System;

public class WeaponSystem : NetworkBehaviour
{
    public WeaponObject[] weapons = new WeaponObject[5];

    public WeaponObject activeWeapon { get; private set; }
    public int activeWeaponSlot { get; private set; }
    private Kinematic _kinematic;
    private Entity _entity;
    private void Awake()
    {
        _kinematic = GetComponent<Kinematic>();
        _entity = GetComponent<Entity>();
    }
    public void AddWeapon(Weapon weapon, int slot) 
    {
        if (slot < weapons.Length)
            weapons[slot] = new WeaponObject { item = weapon};
        else
            weapons = weapons.Append(new WeaponObject { item = weapon });
    }

    public void RemoveWeapon(int slot)
    {
        if (weapons[slot] != null)
            weapons[slot] = null;
    }

    private void FixedUpdate()
    {
        if (activeWeapon == null)
            return;

        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
                weapons[i].timeSinceLastShot += Time.fixedDeltaTime;
        }
    }

    public void TryFireWeapon(int weapon) 
    {
        if (activeWeapon != weapons[weapon])
        {
            activeWeapon = weapons[weapon];
            if (isServer)
            {
                RpcSyncActiveWeapon(activeWeapon.timeSinceLastShot);
                var oldWeapon = weapons[activeWeaponSlot];
                if (oldWeapon == null)
                    return;

                RpcSyncLastWeapon(activeWeaponSlot, oldWeapon.timeSinceLastShot);
            }
        }

        activeWeaponSlot = weapon;

        if (_entity.IsInSafezone)
            return;

        if (activeWeapon != null && activeWeapon.item != null &&  activeWeapon.timeSinceLastShot > activeWeapon.item.weaponData.refire)
            FireWeapon(activeWeapon.item);
    }

    [ClientRpc]
    public void RpcSyncLastWeapon(int weaponId, float syncTime) 
    {
        var weapon = weapons[weaponId];
        if (weapon != null)
            weapon.timeSinceLastShot = syncTime;
    }

    [ClientRpc]
    public void RpcSyncActiveWeapon(float syncTime)
    {
        var weapon = activeWeapon;
        if (weapon != null)
            weapon.timeSinceLastShot = syncTime;
    }

    private void FireWeapon(Weapon weapon)
    {
        if (weapon.ShootSound != null)
            SoundManager.Play(weapon.ShootSound, transform.position);

        for (int i = 0; i < weapon.weaponData.projectileCount; i++)
        {
            Vector2 newProjVel = _kinematic.velocity * weapon.weaponData.bulletData.speed;
            var angle = _kinematic.rotation;
            var targetEntity = _entity.targetNeutral;
            if (targetEntity != null)
            {
                var target = targetEntity.kinematic;

                Vector2 diffVel;
                if (weapon.weaponData.inheritVelocity)
                    diffVel = target.velocity - _kinematic.velocity; //add kinematic.velocity later
                else
                    diffVel = target.velocity;
                if (diffVel.sqrMagnitude < weapon.weaponData.bulletData.speed * weapon.weaponData.bulletData.speed) //microoptimization, ha
                {
                    float directAngle = Mathf.Atan2(target.transform.position.y - _kinematic.transform.position.y, target.transform.position.x - _kinematic.transform.position.x); //angle to target, in radians
                    Vector2 directVel = new Vector2(Mathf.Cos(directAngle), Mathf.Sin(directAngle));
                    float cx = directVel.x * diffVel.x;
                    float sy = directVel.y * diffVel.y;
                    float x2y2 = diffVel.x * diffVel.x + diffVel.y * diffVel.y;
                    float extraSpeed = Mathf.Sqrt(weapon.weaponData.bulletData.speed * weapon.weaponData.bulletData.speed - x2y2 + (cx + sy) * (cx + sy)) - cx - sy;
                    newProjVel = diffVel + directVel * extraSpeed;
                    //debug: newProjVel.magnitude should be bulletSpeed.
                    angle = Mathf.Atan2(newProjVel.y, newProjVel.x) * Mathf.Rad2Deg;
                    //then start the bullet at bulletSpeed in this ^  direction. Fixed speed bullets work best

                    var angleDiff = angle - _kinematic.rotation; //now check if it's actually in the range
                    if (angleDiff > 180)
                        angleDiff -= 360;
                    else if (angleDiff < -180)
                        angleDiff += 360;
                    if (angleDiff > weapon.weaponData.aimAssistArc / 2)
                        angle = _kinematic.rotation + weapon.weaponData.aimAssistArc / 2; //max arc, left
                    else if (angleDiff < -weapon.weaponData.aimAssistArc / 2)
                        angle = _kinematic.rotation - weapon.weaponData.aimAssistArc / 2; //max arc, right
                }
            }

            var rotation = angle; 
            if (weapon.weaponData.spread != 0)
                rotation+=-(-(weapon.weaponData.spread / 2) + (i * (weapon.weaponData.spread / weapon.weaponData.projectileCount)));

            Vector2 gunPoint = weapon.weaponData.gunPoint;

            if (weapon.weaponData.spacing != 0)
            {
                float offset = i + 0.5f;
                /* Position */
                Vector2 shift = new Vector2(Mathf.Cos((_kinematic.rotation) * Mathf.Deg2Rad), Mathf.Sin((_kinematic.rotation) * Mathf.Deg2Rad));
                float inc = weapon.weaponData.spacing / weapon.weaponData.projectileCount;
                Vector2 startingPos = new Vector2(weapon.weaponData.spacing * shift.x * .5f, weapon.weaponData.spacing * shift.y * .5f);
                gunPoint = new Vector3(transform.position.x + shift.x * offset * inc - startingPos.x + gunPoint.x, transform.position.y + shift.y * offset * inc - startingPos.y + gunPoint.y, 0);
            }
            else
                gunPoint += (Vector2)transform.position;

            Projectile bullet = LSObjectPool.GetUsableProjectile();

            var velocity = new Vector2();
            if (weapon.weaponData.inheritVelocity)
                velocity = newProjVel;
               //velocity = transform.up * (Mathf.Cos((Mathf.Atan2(_kinematic.velocity.y, _kinematic.velocity.x)) - rotation * Mathf.Deg2Rad) * 0.5f + 0.5f) * _kinematic.velocity.magnitude;

            bullet.entity = _entity;
            bullet.weapon = weapon;

            if (weapon.weaponData.scale != Vector2.zero)
                bullet.transform.localScale = weapon.weaponData.scale;

            bullet.transform.position = gunPoint;
            bullet.transform.eulerAngles = new Vector3(0, 0, rotation);
            //bullet.Initialize(velocity, weapon.weaponData.bulletData, weapon.Sprite, weapon.color, weapon.trailColor);
        }
        activeWeapon.timeSinceLastShot = 0;
    }
}
