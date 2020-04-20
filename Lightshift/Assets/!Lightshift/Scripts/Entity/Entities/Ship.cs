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
    private PlayerController _input;
    private Heart _heart;
    private Shield _shield;
    private Generator _generator;
    private void Start()
    {
        _lightLance = GetComponent<LightLance>();
        _engine = GetComponent<Engine>();
        _wing = GetComponent<Wing>();
        _hull = GetComponent<Hull>();
        _input = GetComponent<PlayerController>();
        _shield = GetComponent<Shield>();
        _heart = GetComponent<Heart>();
        _generator = GetComponent<Generator>();

        if (isServer || isLocalPlayer)
            CameraFollow.Instance.SetTarget(gameObject.transform);

        if (isServer)
        {
            var player = Server.GetPlayer(connectionToClient);

            if (player != null)
                SetDisplayName(displayName: player.Username);
            else SetDisplayName(displayName: $"Player {connectionToClient.connectionId}");

            _engine.maxSpeed = 10f;
            _engine.acceleration = 4f;
            _engine.brakeForce = 1;
            _hull.weight = .5f;

            _wing.agility = 60f;

            _heart.SetMaxHealth(1000);
            _heart.SetHealth(500);

            _shield.SetMaxShield(850);
            _shield.SetShield(500);

            _generator.maxPower = 100;
            _generator.power = 100;

            _lightLance.SetRange(16);
            _lightLance.pullForce = 150;
            _lightLance.powerCost = 50;
        }

        _hull.SetImage(18, Color.white);
        _wing.SetImage(19, Color.white);
    }

    IEnumerator RandomizeShip() 
    {
        _hull.SetImage(UnityEngine.Random.Range(0, PrefabManager.Instance.Hulls.Count-1), Color.white);
        _wing.SetImage(UnityEngine.Random.Range(0, PrefabManager.Instance.Wings.Count - 1), Color.white);

        yield return new WaitForSeconds(1);

        StartCoroutine(RandomizeShip());
    }

    private void FixedUpdate()
    {
        //HandlePowerRegen();
        //HandleShieldRegen();
        //HandleWeapons();
        //HandleDamageQueue();
        //HandleTargetting();

        _engine.Move(_input.VerticalAxis, _input.OverDrive);
        if (targetEntity != null)
            _lightLance.HandleLightLance(_input.LightLance, targetEntity.transform);
        _wing.Turn(_input.HorizontalAxis);

        HandleSafeZone();
    }
}
