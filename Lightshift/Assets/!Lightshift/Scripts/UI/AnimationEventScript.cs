using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventScript : MonoBehaviour
{
    public void DestroyParentGameobject() 
    {
        Destroy(transform.parent.gameObject);
    }

    public void DestroyGameobject()
    {
        Destroy(gameObject);
    }
    public void DisableGameobject() 
    {
        gameObject.SetActive(false);
    }
}
