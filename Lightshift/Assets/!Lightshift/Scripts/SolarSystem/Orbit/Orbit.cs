using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : NetworkBehaviour
{
    private Transform Transform
    {
        get
        {
            if (_transform == null)
                _transform = transform;
            return transform;
        }
    }

    private Transform _transform;
    public float orbitSpeed = 5;
    void FixedUpdate()
    {
        if (isServer)
            Transform.Rotate(0, 0, orbitSpeed * Time.deltaTime);
    }
}
