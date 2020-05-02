using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu(fileName = "Starship", menuName = "Lightshift/Create Starship", order = 1)]
public class Starship : Item
{
    public int cost;

    public Sprite StoreIcon;
}