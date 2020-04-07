using System;
using UnityEngine;

[Serializable]
public struct WeaponData 
{
    public string key;
    //Weapon Info
    public float rateOfFire;
    public float cooldown;
    public float projectileCount;
    public int shootSoundEffect;
    public int randomness;
    public Vector2 scale;
    public Vector2 offset;
    public int projectileId;
    public float spread;
    public float spacing;

    // Bullet data
    public BulletData bulletData;
}