using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingObject : MonoBehaviour
{

    public float DamagePerSecond = 1;
    public DamageType Type;

    public void OnTriggerStay2D(Collider2D collision)
    {
        var damageable = collision.GetComponentInParent<DamageableObject>();
        if (damageable != null)
            damageable.DamageObject(DamagePerSecond * Time.deltaTime, Type);
    }
}
