using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Heart : NetworkBehaviour 
{
    [SyncVar]
    public float healthRegen = 50;
    [SyncVar(hook = nameof(SetUIHealthMax))]
    public float maxHealth = 2000;
    [SyncVar(hook = nameof(SetUIHealth))]
    public float health = 2000;

    private Entity _entity;
    private EntityUI _ui;

    private void Awake()
    {
        _ui = GetComponent<EntityUI>();
        _entity = GetComponent<Entity>();
    }
    public void Update()
    {
        if (isServer)
        {
            var health = this.health;
            if (health >= maxHealth)
                return;

            health += healthRegen * Time.deltaTime;

            if (health >= maxHealth)
                health = maxHealth;

            SetHealth(health);
        }
    }

    private void SetUIHealthMax(float old, float newValue)
    {
        _ui.SetHealth(health, newValue);
    }

    private void SetUIHealth(float old, float newValue)
    {
        _ui.SetHealth(newValue, maxHealth);
    }

    public void SetHealth(float value)
    {
        if (!isServer)
            return;
        SetUIHealth(health, value);
        health = value;
    }

    public void SetMaxHealth(float value)
    {
        if (!isServer)
            return;
        SetUIHealthMax(maxHealth, value);
        maxHealth = value;
    }
}