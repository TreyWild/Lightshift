using Lightshift;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : Entity
{
    private LightLance _lightLance;
    private Engine _engine;
    private Wing _wing;
    private Hull _hull;

    public override void OnStartServer()
    {
        base.OnStartServer();

        CameraFollow.Instance.Target = transform.gameObject;
    }
    private void Start()
    {
        _lightLance = gameObject.AddComponent<LightLance>();
        _engine = gameObject.AddComponent<Engine>();
        _wing = gameObject.AddComponent<Wing>();
        _hull = gameObject.AddComponent<Hull>();

        _hull.SetImage(10, Color.white);
        _wing.SetImage(13, Color.white);
        _engine.maxSpeed = 0.2f;
        _engine.acceleration = 2f;
        _engine.brakeForce = 2;
        _engine.weight = .5f;

        _wing.agility = 130f;
    }

    private void Update()
    {
        //HandlePowerRegen();
        //HandleShieldRegen();
        //HandleWeapons();
        //HandleDamageQueue();
        //HandleTargetting();
        HandleSafeZone();
    }
}
