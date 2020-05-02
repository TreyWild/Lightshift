﻿using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu(fileName = "Weapon", menuName = "Lightshift/Create Weapon", order = 3)]
public class Weapon : Item
{
    public WeaponData weaponData;

    [HideInInspector]
    public float timeSinceLastShot;
}
