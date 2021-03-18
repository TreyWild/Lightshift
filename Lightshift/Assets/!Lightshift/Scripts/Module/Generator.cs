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
    public float maxPower;
    [SyncVar(hook = nameof(SetUIPower))]
    public float power;
    [SyncVar]
    public float powerRegen;

    private EntityUI _ui;
    private Entity _entity;

    private void Awake()
    {
        _ui = GetComponent<EntityUI>();
        _entity = GetComponent<Entity>();
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
            {
                power = maxPower;
                return;
            }

            if (_entity.isInCheckpoint)
                power += maxPower / 10 * Time.deltaTime;
            else
                power += powerRegen * Time.deltaTime;


            if (power >= maxPower)
                power = maxPower;
        }
    }

    public void ConsumePower(int amount) 
    {
        if (isServer)
        {
            power -= amount;
            if (power < 0)
                power = 0;
        }
    }
}
