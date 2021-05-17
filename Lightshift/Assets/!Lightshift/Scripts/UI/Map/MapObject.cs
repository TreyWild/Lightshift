using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    public string Id = Guid.NewGuid().ToString();
    public Color nameColor = Color.white;
    public Color iconColor = Color.white;

    public string Name;
    public string Lore;

    public Sprite Icon;
}
