using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SolarSystem : MonoBehaviour
{
    public float OrbitalSpeed;

    [HideInInspector]
    public List<float> OrbitSizes = new List<float>();

    private List<OrbitalData> _orbitalData = new List<OrbitalData>();
    private void Awake()
    {
        var orbits = GetComponentsInChildren<Orbit>();
        if (orbits == null)
            return;

        foreach (var orbit in orbits)
        {
            var mapObject = orbit.GetComponentInChildren<MapObject>();
            if (mapObject == null)
                continue;

            var distance = Vector2.Distance(transform.position, mapObject.transform.position);

            OrbitSizes.Add(distance);

            orbit.orbitSpeed = (OrbitalSpeed*100) / (distance/2);

            orbit.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));

            _orbitalData.Add(new OrbitalData { distance = distance, orbit = orbit });
        }      
    }

    private void Update()
    {
        foreach (var orbit in _orbitalData)
        {
            orbit.orbit.orbitSpeed = (OrbitalSpeed*100) / (orbit.distance / 2);
        }
    }
}

public class OrbitalData 
{
    public Orbit orbit;
    public float distance;
}
