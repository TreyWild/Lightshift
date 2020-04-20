using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HeldItemUI : MonoBehaviour
{
    public Vector3 offset;
    void Update()
    {
        transform.position = new Vector2(offset.x + Input.mousePosition.x, offset.y + Input.mousePosition.y);
    }
}
