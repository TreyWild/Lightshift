using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class KinematicManager : MonoBehaviour
{

    private static KinematicManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private List<GameObject> _kinematicObjects = new List<GameObject>();

    public static void AddObject(GameObject gameObject) 
    {
        if (Instance._kinematicObjects.Contains(gameObject))
            return;

        Instance._kinematicObjects.Add(gameObject);
    }

    public static void RemoveObject(GameObject gameObject)
    {
        if (Instance._kinematicObjects.Contains(gameObject))
            return;

        Instance._kinematicObjects.Remove(gameObject);
    }

    public static List<GameObject> GetObjectsNearPosition(Vector3 position, float range)
    {
        var newList = new List<GameObject>();
        foreach (GameObject entity in Instance._kinematicObjects)
        {
            if (entity == null)
                continue;
            Vector3 difference = entity.transform.position - position;
            if (Mathf.Abs(difference.x) < range && Mathf.Abs(difference.y) < range && Mathf.Abs(difference.z) < range)
            {
                //inside square range
                if (difference.sqrMagnitude < range * range)
                {
                    newList.Add(entity);
                }
            }
        }
        return newList;
    }
}

