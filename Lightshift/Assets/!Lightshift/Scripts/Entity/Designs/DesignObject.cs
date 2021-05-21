using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DesignObject : MonoBehaviour
{
    public SpriteRenderer Forground;
    public SpriteRenderer Background;

    public PolygonCollider2D GenerateCollider() 
    {
        var collider = GetComponent<PolygonCollider2D>();
        if (collider != null)
            Destroy(collider);
        collider = gameObject.AddComponent<PolygonCollider2D>();

        return collider;
    }
}
