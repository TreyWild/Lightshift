using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "SolarSystem", menuName = "Create SolarSystem", order = 2)]
public class SolarSystem : ScriptableObject
{
    public string Name;

    public string Id;

    [Scene]
    public string Scene;

    public AudioClip Music;

    public bool Default = false;

    public int Cost;

    public int TravelCost;
}
