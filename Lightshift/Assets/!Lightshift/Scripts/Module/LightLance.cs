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

    private float minimumDistance = 8;

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

            var force = (pullForce/distance) * Time.fixedDeltaTime;
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