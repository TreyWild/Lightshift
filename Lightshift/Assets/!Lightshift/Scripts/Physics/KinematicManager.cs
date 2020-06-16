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

    public static List<GameObject> GetObjectsNearPosition(Vector3 position, float range, bool useBoundingBoxOnly = false)
    {
        var newList = new List<GameObject>();
        if (useBoundingBoxOnly)
        {
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
        //else is implied
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

    public static GameObject GetClosestObjectInArc(Vector3 position, float direction, float range, float arc, float tuning = 1) //arc and direction are in degrees, direction is normally the way you face.
    //tuning is from 0 (always target closest to centerline) to infinity (always target the closest within the arc)
    {
        //direction = - direction + 90; //delete this line if there's no problem
        float c = Mathf.Cos(direction * Mathf.Deg2Rad);
        float s = Mathf.Sin(direction * Mathf.Deg2Rad);
        float aimLines = Mathf.Tan((90 - arc / 2) * Mathf.Deg2Rad);

        GameObject closestTarget = null;
        float minSqrDist = range * range;
        float minArcDist = 1 / aimLines;
        bool found = false;

        foreach (GameObject entity in Instance._kinematicObjects)
        {
            if (entity == null)
                continue;
            Vector3 positionDiff = entity.transform.position - position;
            float sqrDist = positionDiff.sqrMagnitude;
            if (sqrDist < range * range)
            {
                //this puts the point into local space
                float TransformedX = positionDiff.y * c + positionDiff.x * s; //Possible errors: if the arc turns left when you turn right, set direction to -direction. 
                float TransformedY = positionDiff.x * c - positionDiff.y * s; // If it's off by a quarter/half turn but moves with you, add or subtract 90 or 180 to direction at the start.  

                if (TransformedY >= Mathf.Abs(TransformedX) * aimLines) //is it in the arc?
                {
                    float arcDist = TransformedX / TransformedY; //Mathf.Atan(arcDist) is the angle that the target is offcenter

                    if ((sqrDist < minSqrDist && Mathf.Abs(TransformedX) <= tuning) || (!found && Mathf.Abs(arcDist) < minArcDist && Mathf.Abs(TransformedX) > tuning))
                    {
                        if (sqrDist < minSqrDist)
                            found = true;
                        minSqrDist = sqrDist;
                        minArcDist = Mathf.Abs(arcDist);
                        closestTarget = entity;
                    }
                }
            }
        }
        return closestTarget;
    }

    public static bool ObjectIsInArc(Kinematic target, Vector3 position, float direction, float arc) //direction and arc are in degrees
    {
        var targetAngle = Mathf.Atan2(target.transform.position.y - position.y, target.transform.position.x - position.x) * Mathf.Rad2Deg;
        var angleDiff = (direction - targetAngle) % 360;
        if (angleDiff > 180)
            angleDiff -= 360;
        else if (angleDiff < -180)
            angleDiff += 360;

        if (Mathf.Abs(angleDiff) <= arc / 2)
            return true;
        else
            return false;
    }
}

