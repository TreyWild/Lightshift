using UnityEngine;
using System.Collections;
using Boo.Lang;

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
        activeWeapon = weapons[weapon];
        if (activeWeapon != null &&  activeWeapon.timeSinceLastShot > activeWeapon.weaponData.refire)
            FireWeapon(activeWeapon);
    }

    private void FireWeapon(Weapon weapon) 
    {
        float arc = 0;
        if (weapon.weaponData.spreadArc > 0)
            arc = Random.Range(-weapon.weaponData.spreadArc * 0.5f, weapon.weaponData.spreadArc * 0.5f);

        Vector2 gunPoint = transform.position;

        float x = Mathf.Cos((_kinematic.transform.eulerAngles.x - gunPoint.y + arc) * Mathf.Deg2Rad);
        float y = -Mathf.Sin((_kinematic.transform.eulerAngles.x - gunPoint.y + arc) * Mathf.Deg2Rad);

        Vector3 pos = _kinematic.transform.position + new Vector3(gunPoint.x * x - gunPoint.y * y, 0, gunPoint.y * x + gunPoint.x * y);

        Projectile bullet = LSObjectPool.GetUsableProjectile();


        bullet.transform.eulerAngles = new Vector3(bullet.transform.eulerAngles.x, bullet.transform.eulerAngles.y, (bullet.transform.eulerAngles.z + arc) - transform.eulerAngles.z);

        var velocity = new Vector2(x, y) * weapon.weaponData.bulletData.speed;

        bullet.owner = _entity;

        bullet.Initialize(velocity, gunPoint, weapon.weaponData.bulletData, weapon.Sprite, weapon.color);
    }
}
