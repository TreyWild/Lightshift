using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landable : MonoBehaviour
{
    public string Id = Guid.NewGuid().ToString();

    public LandableType Type;

}
