using Lightshift;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : Entity
{

    [Header("Modules")]
    [SerializeField] private Thruster _engine;




    private InputHandler _input;
    private void Start()
    {
        _input = GetComponent<InputHandler>();

        SoundManager.Instance.Play(17, transform.position);
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
