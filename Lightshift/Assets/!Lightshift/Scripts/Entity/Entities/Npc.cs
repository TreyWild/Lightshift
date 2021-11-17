using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Npc : Ship
{
    public NpcSettings Settings;

    private float _aimArc;
    private int _rotationMod;

    public override void OnStartServer()
    {
        if (isServer)
            InitSettings();
    }

    public void SetFaction(NpcFaction faction)
    {
        if (isServer)
            teamId = faction.ToString();
    }

    private void InitSettings()
    {
        trackingRange = Settings.TrackingRange;
        SetName(Settings.Name);
        SetWeapons(Settings.Weapons.ToArray());
        SetFaction(Settings.Faction);
        SetModifiers(Settings.Stats);
        SetSpawnLocation(kinematic.position);
        Respawn();
    }

    private Vector2 _spawnLocation;
    public void SetSpawnLocation(Vector2 spawnLocation) 
    {
        _spawnLocation = spawnLocation;
    }

    //public void LoadBehaviors(List<UnityEngine.Object> npcBehaviors) 
    //{
    //    var behaviors = new List<string>();
    //    foreach (var behavior in npcBehaviors) 
    //        behaviors.Add(behavior.name);

    //    for (int i = 0; i < _behaviors.Count; i++)
    //        Destroy(_behaviors[i]);
    //    _behaviors.Clear();

    //    foreach (var behavior in behaviors)
    //        gameObject.AddComponent(Type.GetType(behavior));
    //}

    public void SetWeapons(Weapon[] weapons) 
    {
        for (int i = 0; i < weapons.Length; i++)
            if (weapons[i] != null)
                weaponSystem.AddWeapon(weapons[i], i);
    }
    public void SetName(string name = "") 
    {
        SetDisplayName(displayName: name);
    }


    public void RunBasicFollowAI() 
    {
        RunAIChecks();
        MoveForwards(speed);
        RotateTowardsTarget(agility);
    }

    public void MoveForwards(float speed)
    {
        if (HasTarget && !IsTargetIsInFront(50))
            speed *= 3.5f;
        kinematic.AddForce(transform.up * speed * Time.fixedDeltaTime);
    }
    public void RotateTowardsTarget(float speed)
    {
        if (HasTarget && !IsTargetIsInFront(50))
            speed *= 3.5f;
        var targetAngle = GetTargetRotationMod();
        kinematic.SetDirection(kinematic.rotation + speed * targetAngle * Time.fixedDeltaTime);
    }

    public void RunAIChecks()
    {
        var target = targetNeutral;

        Vector2 targetPos = _spawnLocation;
        if (!HasTarget)
        {
            _aimArc = 0;
            _rotationMod = 0;
        }else targetPos = target.transform.position;

        var currentAngle = kinematic.rotation + 90;
        var targetAngle = Mathf.Atan2(targetPos.y - transform.position.y, targetPos.x - transform.position.x) * 57.29578f;
        _aimArc = currentAngle - targetAngle;

        for (int i = 0; i < 2; i++)
        {
            if (_aimArc > 180)
                _aimArc -= 360;
            else if (_aimArc < -180)
                _aimArc += 360;
        }


        if (_aimArc > 1)
            _rotationMod = -1;
        else if (_aimArc < -1)
            _rotationMod = 1;
        else
            _rotationMod = 0;
    }

    public bool IsTargetIsInFront(float arcRange)
    {
        if (_aimArc < arcRange && _aimArc > -arcRange)
            return true;

        return false;
    }

    public int GetTargetRotationMod()
    {
        return _rotationMod;
    }

    public bool HasTarget => targetNeutral != null && TargetDistance < trackingRange && targetNeutral.alive && !targetNeutral.isInCheckpoint;

    public float TargetDistance => Vector2.Distance(targetNeutral.transform.position, transform.position);

    public event Action<Npc> onDeath;
    public override void OnDeath()
    {
        base.OnDeath();
        //stats = new ModuleData { };
        onDeath?.Invoke(this);
    }
}