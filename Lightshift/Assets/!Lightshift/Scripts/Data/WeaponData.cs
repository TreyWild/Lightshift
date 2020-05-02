using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct WeaponData
{
    public float powerCost;
    public float refire;
    public bool inheritVelocity;
    public float spread;
    public float spacing;
    public float trailLength;
    public int projectileCount;
    public Vector2 gunPoint;
    public Vector2 scale;
    public BulletData bulletData;
}

[Serializable]
public struct BulletData
{
    public float speed;
    public float speedVariance;
    public float damage;
    public int hitCount;
    public float range;
};
