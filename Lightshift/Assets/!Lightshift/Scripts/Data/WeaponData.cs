using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct WeaponData
{
    public float powerCost;
    public float refire;
    public bool inheritVelocity;
    public float spreadArc;
    public float trailLength;

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
