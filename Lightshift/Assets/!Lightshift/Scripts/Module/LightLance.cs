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
    [SyncVar]
    public float maxRange;
    [SyncVar]
    public float powerCost;

    private Rigidbody2D _rigidBody;
    private BeamToTarget _lightLance;
    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _lightLance = Instantiate(PrefabManager.Instance.lightLancePrefab, transform).GetComponent<BeamToTarget>();
        _lightLance.OnFocus += OnLightLanceFocus;
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
        if (isServer)
        {
            if (transform.position == target.position)
                return;


            _rigidBody.AddForce((target.position - transform.position) * ((pullForce / distance) * 100) * Time.deltaTime);

        }
    }
}