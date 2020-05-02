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
    public void HandleLightLance(bool active, Transform target)
    {
        if (active)
            _lightLance.TryFocusTarget(target);
        else if (!active)
            _lightLance.CancelFocus();
    }

    private void OnLightLanceFocus(Transform targetTransform, float distance)
    {
        _lightLance.TryDrawBeam();

        if (hasAuthority)
        {
            if (transform.position == targetTransform.position)
                return;

            //var target = targetTransform.GetComponent<Kinematic>();

            //Vector2 diffVel = target.velocity - _kinematic.velocity;
            //Vector3 diffPos = target.transform.position - _kinematic.transform.position;
            //float dist = diffPos.magnitude;
            //Vector3 diffPosN = diffPos / dist;

            //float c = diffPosN.x;
            //float s = -diffPosN.z;

            //float xn = c * diffVel.x - s * diffVel.y; //force toward/away between target/origin
            //float yn = s * diffVel.x + c * diffVel.y; //force side/side: maybe do something with this?
            //Vector2 radialDV = new Vector2(xn, yn);

            ////now radialDV.x = force in/out, and radialDV.y = force side/side
            //Vector2 tempVelShift = new Vector2(diffPosN.x, diffPosN.z) * radialDV.x / (target.mass + _kinematic.mass); //replace radialDV.x with xn or Mathf.Max(maxforce, xn) if you want a max force

            //target.velocity -= tempVelShift * _kinematic.mass; //mass crossover: one gets the other's mass
            //_kinematic.velocity += tempVelShift * target.mass; //multiply both by 0.25 or 0.1 for some "flexibility"

            var force = (distance/pullForce) * Time.fixedDeltaTime;
            _kinematic.AddForce((targetTransform.position + (transform.forward * force) - transform.position) * (force));
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