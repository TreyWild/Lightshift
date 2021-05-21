using Assets._Lightshift.Scripts.Data;
using Lightshift;
using Mirror;
using SharedModels.Models.Game;
using Smooth;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Kinematic), typeof(Kinematic))]
public class Entity : NetworkBehaviour
{

    [HideInInspector]
    [SyncVar(hook = nameof(UpdateLivingState))] public bool alive;

    [SyncVar] private bool isLanding;

    [SyncVar]
    public short Id;

    [SyncVar(hook = nameof(SetDisplayName))]
    public string displayName;

    [SyncVar]
    public string teamId;
    public SmoothSyncMirror smoothSync;
    public Rigidbody2D rigidBody;
    public Entity targetNeutral;
    public Entity targetEntity;

    public Kinematic kinematic;

    public EntityUI ui;
    private float _timeSinceLastTargetUpdate = 0;
    public bool isInCheckpoint;
    public Action onCleanup;
    public void OnDestroy()
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    {
        onCleanup?.Invoke();
        onCleanup = null;
        EntityManager.RemoveEntity(this);
        teamId = null;
        smoothSync = null;
        rigidBody = null;
        targetNeutral = null;
        targetEntity = null;
        kinematic = null;
        ui = null;
        onModifierChanged = null;
        onLeaveCheckpoint = null;
        onEnterCheckpoint = null;
    }
    public void Awake()
    {
        //weaponSystem = gameObject.AddComponent<WeaponSystem>();
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        kinematic = gameObject.GetComponent<Kinematic>();
        ui = gameObject.AddComponent<EntityUI>();

        onEnterCheckpoint += (checkpoint) => OnEnterCheckpoint(checkpoint);

        onLeaveCheckpoint += (checkpoint) => OnLeaveCheckpoint(checkpoint);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        //SetAlive();
    }

    public void Start()
    {
        EntityManager.AddEntity(this);

        ui.Init(hasAuthority, isServer);
    }

    public virtual void OnEnterCheckpoint(Checkpoint checkpoint)
    {
        isInCheckpoint = true;
    }

    public virtual void OnLeaveCheckpoint(Checkpoint checkpoint)
    {
        isInCheckpoint = false;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        Modifiers.Callback += Modifiers_Callback;

        var modifiers = Modifiers.ToList();
        foreach (var modifier in modifiers)
        {
            UpdateModifier(modifier.Key, modifier.Value);
        };
    }

    private void Modifiers_Callback(SyncIDictionary<Modifier, float>.Operation op, Modifier key, float value)
    {
        if (isServer)
            return;

        UpdateModifier(key, value);
    }

    public readonly SyncDictionary<Modifier, float> Modifiers = new SyncDictionary<Modifier, float>();

    public void ClearModifiers()
    {
        if (isServer)
        {
            var modifiers = Modifiers.ToList();
            foreach (var modifier in modifiers)
                UpdateModifier(modifier.Key, modifier.Value);
        }
    }
    public void SetModifiers(List<GameModifier> modifiers)
    {
        if (!isServer)
            return;

        ClearModifiers();

        foreach (var modifier in modifiers)
        {
            UpdateModifier(modifier.Type, modifier.Value);

            switch (modifier.Type)
            {
                case Modifier.MaxHealth:
                    UpdateModifier(Modifier.Health, modifier.Value);
                    break;

                case Modifier.MaxShield:
                    UpdateModifier(Modifier.Shield, modifier.Value);
                    break;

                case Modifier.MaxPower:
                    UpdateModifier(Modifier.Power, modifier.Value);
                    break;
            }
        }

        //foreach (var modifier in Modifiers)
        //    UpdateModifier(modifier.Key, modifier.Value);
    }

    public Action<Modifier, float> onModifierChanged;
    public void UpdateModifier(Modifier type, float value)
    {
        if (isServer)
        {
            if (!Modifiers.ContainsKey(type))
                Modifiers.Add(type, value);
            else
                Modifiers[type] = value;
        }

        onModifierChanged?.Invoke(type, value);
    }

    public void FixedUpdate()
    {
        if (!isServer)
            return;

        _timeSinceLastTargetUpdate += Time.fixedDeltaTime;
        if (_timeSinceLastTargetUpdate > 0.2f)
        {
            UpdateTargets();
            _timeSinceLastTargetUpdate = 0;
        }
    }

    public void SetDisplayName(string oldValue = "", string displayName = "")
    {
        ui.SetName(displayName);

        if (isServer)
            this.displayName = displayName;
    }

    //#region Collision
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    var entity = collision.gameObject.GetComponent<Entity>();
    //    if (entity == null || entity.uniqueId == uniqueId)
    //        return;

    //    ///....
    //    collision.relativeVelocity.....
    //}
    //#endregion Collision

    [SyncVar]
    public float trackingRange = 100;

    [SyncVar(hook = nameof(UpdateNeutralTarget))]
    private short _targetNeutral;

    private void UpdateNeutralTarget(short oldValue = 0, short newValue = 0)
    {
        if (targetNeutral == null)
            targetNeutral = EntityManager.GetEntity(newValue);
        else if (targetNeutral.Id != newValue)
            targetNeutral = EntityManager.GetEntity(newValue);
    }

    [SyncVar(hook = nameof(UpdateTarget))]
    private short _target;

    private void UpdateTarget(short oldValue = 0, short newValue = 0)
    {
        if (targetEntity == null)
            targetEntity = EntityManager.GetEntity(newValue);
        else if (targetEntity.Id != newValue)
            targetEntity = EntityManager.GetEntity(newValue);
    }

    [ClientRpc]
    private void RpcUpdateTarget(short target)
    {
        UpdateTarget(newValue: target);
    }

    [ClientRpc]
    private void RpcUpdateNeutral(short target)
    {
        UpdateNeutralTarget(newValue: target);
    }

    public void UpdateTargets()
    {
        Entity closestEntity = null;
        Entity closestNuetralEntity = null;

        var entities = GetNearbyEntities();
        var smallestDistance = Mathf.Infinity;

        for (int i = 0; i < entities.Count; i++)
        {

            if (entities[i] == null)
                continue;

            if (entities[i] == this)
                continue;

            if (entities[i].GetType() == typeof(Station))
                continue;

            if (!entities[i].alive)
                return;

            if (entities[i].isInCheckpoint)
                return;

            var distance = (transform.position - entities[i].transform.position).sqrMagnitude;

            if (distance < smallestDistance)
            {
                smallestDistance = distance;

                if (teamId != entities[i].teamId)
                {
                    closestNuetralEntity = entities[i];
                }
                closestEntity = entities[i];
            }


            //Assign entity targets
            if (targetEntity != closestEntity)
            {
                targetEntity = closestEntity;
                short targetId = -1;
                if (targetEntity != null)
                    targetId = targetEntity.Id;

                RpcUpdateTarget(targetId);
                _target = targetId;
            }
            if (targetNeutral != closestNuetralEntity)
            {
                targetNeutral = closestNuetralEntity;

                short targetId = -1;
                if (targetNeutral != null)
                    targetId = targetNeutral.Id;

                RpcUpdateNeutral(targetId);
                _targetNeutral = targetId;
            }
        }
    }

    private List<Entity> GetNearbyEntities()
    {
        var nearbyEntities = new List<Entity>();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, trackingRange);
        for (int i = 0; i < colliders.Length; i++)
        {
            var nearbyEntity = colliders[i].transform.root.GetComponent<Entity>();

            if (nearbyEntity == null || !nearbyEntity.alive)
                continue;

            nearbyEntities.Add(nearbyEntity);
        }

        return nearbyEntities;
    }

    private void UpdateLivingState(bool oldValue, bool newValue)
    {
        if (alive)
            Respawn();
        else OnDeath();
    }

    public void SetLanding()
    {
        if (isServer)
        {
            isLanding = true;
            SetDead();
        }
    }

    public void SetDead()
    {
        if (isServer)
            alive = false;

        if (hasAuthority)
            kinematic.velocity = Vector2.zero;

        OnDeath();
    }


    public void Kill()
    {
        CmdKillEntity();
    }

    [Command]
    private void CmdKillEntity()
    {
        alive = false;

        RpcKillEntity();
    }

    [ClientRpc]
    private void RpcKillEntity()
    {
        OnKilled();
    }

    public virtual void OnDeath()
    {

    }
    public virtual void OnKilled()
    {
        Instantiate(PrefabManager.Instance.deathEffectPrefab, kinematic.position, kinematic.Transform.rotation);
    }

    public void Respawn()
    {
        if (isServer)
            alive = true;

        OnRespawn();
    }

    public virtual void OnRespawn()
    {

        //Respawn Effect
        Instantiate(PrefabManager.Instance.spawnEffectPrefab, transform);
    }

    [TargetRpc]
    private void TargetRpcSetPosition(Vector2 position)
    {
        kinematic.position = position;
    }

    public void SetPosition(Vector2 pos)
    {
        // Set respawn position
        if (isServer && connectionToClient != null)
            TargetRpcSetPosition(pos);
        else if (isServer)
            kinematic.position = pos;
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    var kinematic = collision.GetComponent<Kinematic>();
    //    if (kinematic == null)
    //        return;

    //    if (isServer && connectionToClient == null || hasAuthority) 
    //    {

    //    } 
    //}

    #region Checkpoints
    public event Action<Checkpoint> onLeaveCheckpoint;
    public event Action<Checkpoint> onEnterCheckpoint;

    public void EnterCheckpoint(Checkpoint checkpoint)
    {
        onEnterCheckpoint?.Invoke(checkpoint);
    }
    public void LeaveCheckpoint(Checkpoint checkpoint)
    {
        onLeaveCheckpoint?.Invoke(checkpoint);
    }
    #endregion
}
