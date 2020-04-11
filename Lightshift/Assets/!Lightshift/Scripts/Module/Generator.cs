using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Generator : NetworkBehaviour 
{
    [SyncVar(hook = nameof(SetUIPowerMax))]
    public float maxPower = 150;
    [SyncVar(hook = nameof(SetUIPower))]
    public float power = 150;
    [SyncVar]
    public float powerRegen = 20;

    private EntityUI _ui;

    private void Awake()
    {
        _ui = GetComponent<EntityUI>();
    }

    private void SetUIPowerMax(float old, float newValue)
    {
        _ui.SetPower(power, newValue);
    }

    private void SetUIPower(float old, float newValue)
    {
        _ui.SetPower(newValue, maxPower);
    }

    private void Update()
    {
        if (isServer)
        {
            if (power >= maxPower)
                return;

            power += powerRegen * Time.deltaTime;

            if (power >= maxPower)
                power = maxPower;
        }
    }
}
