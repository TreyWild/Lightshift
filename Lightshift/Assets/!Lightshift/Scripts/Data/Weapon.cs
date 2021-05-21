﻿using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu(fileName = "Weapon", menuName = "Lightshift/Create Weapon", order = 3)]
public class Weapon : ModuleItem
{
    public GameObject hitEffectPrefab;
    public AudioClip ShootSound;
    public AudioClip HitSound;

    public Color trailColor = Color.white;

    public WeaponData weaponData;

    [HideInInspector]
    public float timeSinceLastShot;
}
