using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class LightLance : NetworkBehaviour
{
    [SyncVar]
    public float pullForce;

    [SyncVar(hook = nameof(SetMaxRange))]
    public float maxRange;

    [SyncVar]
    public float powerCost;

    private float minimumDistance = 2;

    private Kinematic _kinematic;
    private BeamToTarget _lightLance;
    private void Awake()
    {
        _kinematic = GetComponent<Kinematic>();
        _lightLance = Instantiate(PrefabManager.Instance.lightLancePrefab, transform).GetComponent<BeamToTarget>();
        _lightLance.OnFocus += OnLightLanceFocus;
        _lightLance.CancelFocus();
    }

    public void SetColor(Color color) => _lightLance.SetColor(color);
    public void HandleLightLance(bool active, Kinematic target)
    {
        if (active)
            _lightLance.TryFocusTarget(target);
        else if (!active)
            _lightLance.CancelFocus();
    }

    private void OnLightLanceFocus(Kinematic target, float distance)
    {
        _lightLance.TryDrawBeam();

        if (hasAuthority)
        {
            //if (transform.position == targetTransform.position)
            //    return;

            //var force = (pullForce/distance) * Time.fixedDeltaTime;
            //_kinematic.AddForce((targetTransform.position + (transform.forward * force) - transform.position) * (force));

            Vector2 diffVel = target.velocity - _kinematic.velocity;
            Vector3 diffPos = target.transform.position - _kinematic.transform.position;
            float dist = diffPos.magnitude;
            Vector3 diffPosN = diffPos / dist;

            if (dist <= maxRange)
            {
                float c = diffPosN.x;
                float s = -diffPosN.z;

                float xn = c * diffVel.x - s * diffVel.y; //force toward/away between target/origin
                Vector2 tempVelShift = new Vector2(diffPosN.x, diffPosN.z) * Mathf.Max(xn + (dist - minimumDistance) * 0.8f, (dist - minimumDistance) * 0.8f) / (target.mass + target.mass) * (0.0625f);

                target.velocity -= tempVelShift * _kinematic.mass; //mass crossover: one gets the other's mass
                _kinematic.velocity += tempVelShift * target.mass;

                //alt version, in case you prefer no movement for the other thing:
                //origin.velocity += new Vector2(diffPosN.x, diffPosN.z) * Mathf.Max(xn + (dist - minDist) * 0.8f, (dist - minDist) * 0.8f) * (0.0625f); //and delete the previous 3 lines
            }
        }
    }

    public void SetRange(float value) 
    {
        //if (isServer)
            _lightLance.maxDistance = value;

        //maxRange = value;
    }

    private void SetMaxRange(float oldValue = 0, float value = 0) 
    {
        _lightLance.maxDistance = value;
    }
}