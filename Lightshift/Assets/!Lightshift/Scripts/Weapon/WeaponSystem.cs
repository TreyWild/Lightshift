using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    public bool WeaponSystemDisabled = false;

    private Entity _entity;
    private Weapon[] _weapons = new Weapon[5];
    public Action<int> onWeaponSlotChanged;

    //Handles weapon order
    public int activeWeaponSlot;
    private void Awake()
    {
        _entity = GetComponent<Entity>();
    }

    public void SetWeaponOrder(string[] weaponKeyList)
    {
        for (int i = 0; i < weaponKeyList.Count(); i++)
        {
            var key = weaponKeyList[i];
            var slot = i;
            //if (key != null)
            //    DataManager.Instance.GetWeaponDataObject(key, delegate (DataObject dataObject)
            //    {
            //        AddOrReplaceWeapon(slot, dataObject);
            //    });
        }
    }

    public void SetWeapons(Weapon[] weapons) => _weapons = weapons;

    public void AddOrReplaceWeapon(int slot, WeaponData dataObject)
    {
        for (int i = 0; i < _weapons.Count(); i++)
        {
            var weapon = _weapons[i];
            if (weapon == null)
                continue;

            if (slot == i)
            {
                _weapons[i] = null;
                return;
            }
        }

        //switch (dataObject.weaponType)
        //{
        //    case WeaponType.Projectile:
        _weapons[slot] = new ProjectileWeapon { entity = _entity, weaponData = dataObject, weaponKey = dataObject.key };
        //        break;
        //}

    }
    public void ChangeWeapon(int weaponSlot)
    {
        if (activeWeaponSlot == weaponSlot)
            return;

        var weapon = _weapons[weaponSlot];
        if (weapon == null)
            return;

        activeWeaponSlot = weaponSlot;
    }
    public void TryFireWeapon()
    {
        var weapon = _weapons[activeWeaponSlot];

        if (weapon != null)
            weapon.Fire();
    }
    //public void ActivateWeapon(int turretId)
    //{
    //    if (!_activeWeapons.Contains(turretId))
    //        if (_weapons[turretId] != null)
    //        {
    //            _activeWeapons.Add(turretId);
    //            onTurretChanged?.Invoke(turretId, true);
    //        }

    //}

    //public void DeactivateWeapon(int turretId)
    //{
    //    if (_activeWeapons.Contains(turretId))
    //    {
    //        _activeWeapons.Remove(turretId);
    //        onTurretChanged?.Invoke(turretId, false);
    //    }
    //}

    //public void ForceSingleActiveWeapon(int turretId) 
    //{
    //    if (_activeWeapons.Count == 1 && _activeWeapons.Contains(turretId))
    //        return;

    //    _activeWeapons.Clear();
    //    ActivateWeapon(turretId);
    //}

    //public void TryFireWeapons()
    //{
    //    for (int i = 0; i < _activeWeapons.Count; i++) 
    //    {
    //        var weaponId = _activeWeapons[i];

    //        var weapon = _weapons[weaponId];
    //        if (weapon == null)
    //        {
    //            _activeWeapons.Remove(weaponId);
    //            continue;
    //        }

    //        weapon.Fire();
    //    }
    //}

    private void Update()
    {
        for (int i = 0; i < _weapons.Length; i++)
        {
            var weapon = _weapons[i];
            if (weapon == null)
                continue;

            weapon.Update();
        }
    }

    public Weapon[] GetWeapons() => _weapons;
}
