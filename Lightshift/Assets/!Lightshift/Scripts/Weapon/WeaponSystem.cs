using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class WeaponSystem : MonoBehaviour
{
    public Weapon[] weapons = new Weapon[5];

    public Weapon activeWeapon;
    public int activeWeaponSlot;
    private Kinematic _kinematic;
    private Entity _entity;
    private void Awake()
    {
        _kinematic = GetComponent<Kinematic>();
        _entity = GetComponent<Entity>();
    }
    public void AddWeapon(Weapon weapon, int slot) 
    {
        Debug.LogError("Weapon Added");
        if (slot < weapons.Length)
            weapons[slot] = weapon;
        else
            weapons = weapons.Append(weapon);
    }

    public void RemoveWeapon(int slot)
    {
        Debug.LogError("Weapon Removed");
        if (weapons[slot] != null)
            weapons[slot] = null;
    }

    private void FixedUpdate()
    {
        if (activeWeapon == null)
            return;

        activeWeapon.timeSinceLastShot += Time.fixedDeltaTime;
    }

    public void TryFireWeapon(int weapon) 
    {
        activeWeaponSlot = weapon;
        if (activeWeapon != weapons[weapon])
            activeWeapon = weapons[weapon];

        if (_entity.IsInSafezone)
            return;

        if (activeWeapon != null &&  activeWeapon.timeSinceLastShot > activeWeapon.weaponData.refire)
            FireWeapon(activeWeapon);
    }

    private void FireWeapon(Weapon weapon)
    {
        if (weapon.ShootSound != null)
            SoundManager.Play(weapon.ShootSound, transform.position);

        for (int i = 0; i < weapon.weaponData.projectileCount; i++)
        {
            var rotation = _kinematic.rotation; 
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


            bullet.transform.eulerAngles = new Vector3(0,0, rotation);
            bullet.transform.position = gunPoint;
            if (weapon.weaponData.scale != Vector2.zero)
                bullet.transform.localScale = weapon.weaponData.scale;

            bullet.entityId = _entity.Id;

            bullet.weapon = weapon;

            bullet.Initialize(_kinematic.velocity, weapon.weaponData.bulletData, weapon.Sprite, weapon.color);
        }
        activeWeapon.timeSinceLastShot = 0;
    }
}
