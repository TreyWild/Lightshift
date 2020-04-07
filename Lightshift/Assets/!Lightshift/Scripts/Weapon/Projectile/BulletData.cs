using System;
using UnityEngine;

[Serializable]
public struct BulletData
{
    public float range;
    public float speed;
    public float guidance;
    public float trailLength;
    public float trailSize;
    public float trackingDelay;
    public float acceleration;
    public bool hasTrail;
    public Color trailColor;
    public Color bulletColor;
    public int hitEffect;
    public int hitSoundEffect;
}