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
    public float pullForce = 10;

    [SyncVar(hook = nameof(SetMaxRange))]
    public float maxRange = 15;

    [SyncVar]
    public float powerCost = 30;

    private Rigidbody2D _rigidBody;
    private BeamToTarget _lightLance;
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _lightLance = Instantiate(PrefabManager.Instance.lightLancePrefab, transform).GetComponent<BeamToTarget>();
        _lightLance.OnFocus += OnLightLanceFocus;
        _lightLance.CancelFocus();
    }

    public void HandleLightLance(bool active, Transform target)
    {
        if (active)
            _lightLance.TryFocusTarget(target);
        else if (!active)
            _lightLance.CancelFocus();
    }

    private void OnLightLanceFocus(Transform target, float distance)
    {
        _lightLance.TryDrawBeam();
        if (hasAuthority)
        {
            if (transform.position == target.position)
                return;

            var force = (pullForce / distance) * 100;
            _rigidBody.AddRelativeForce((target.position - transform.position)  * (force * Time.deltaTime));

        }
    }

    public void SetRange(float value) 
    {
        if (isServer)
            _lightLance.maxDistance = value;

        maxRange = value;
    }

    private void SetMaxRange(float oldValue = 0, float value = 0) 
    {
        _lightLance.maxDistance = value;
    }
}