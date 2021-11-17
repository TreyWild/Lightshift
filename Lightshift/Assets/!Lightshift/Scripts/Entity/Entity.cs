using Assets._Lightshift.Scripts.Data;
using Assets._Lightshift.Scripts.Utilities;
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
    [HideInInspector]
    [SyncVar] private bool isLanding;

    [SyncVar]
    public short Id;

    [HideInInspector]
    [SyncVar(hook = nameof(SetDisplayName))]
    public string displayName;

    [SyncVar(hook = nameof(UpdateTeamId))]
    public string teamId;
    [HideInInspector]
    public SmoothSyncMirror smoothSync;
    [HideInInspector]
    public Rigidbody2D rigidBody;
    [HideInInspector]
    public Entity targetNeutral;
    [HideInInspector]
    public Entity targetEntity;
    [HideInInspector]
    public Kinematic kinematic;

    [HideInInspector]
    public EntityUI ui;
    [HideInInspector]
    private float _timeSinceLastTargetUpdate = 0;
    [HideInInspector]
    public bool isInCheckpoint;
    [HideInInspector]
    public Action onCleanup;
    [HideInInspector]
    public MapObject mapObject;

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
        mapObject = null;
        onKilled = null;
    }

    private Vector3 _startScale;

    public void Awake()
    {
        //weaponSystem = gameObject.AddComponent<WeaponSystem>();
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        kinematic = gameObject.GetComponent<Kinematic>();
        ui = gameObject.GetComponent<EntityUI>();
        mapObject = GetComponent<MapObject>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    public void Start()
    {
        _startScale = transform.localScale;
        EntityManager.AddEntity(this);
        onEnterCheckpoint += (checkpoint) => OnEnterCheckpoint(checkpoint);
        onLeaveCheckpoint += (checkpoint) => OnLeaveCheckpoint(checkpoint);
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

        ui.InitLocalPlayer(hasAuthority);

        var modifiers = Modifiers.ToList();
        foreach (var modifier in modifiers)
        {
            UpdateClientModifier(modifier.Key, modifier.Value);
        };

    }

    private void Modifiers_Callback(SyncIDictionary<Modifier, float>.Operation op, Modifier key, float value)
    {
        UpdateClientModifier(key, value);
    }

    [HideInInspector]
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
            UpdateModifier(modifier.Type, modifier.Value);;
    }

    public Action<Modifier, float> onModifierChanged;
    public void UpdateClientModifier(Modifier type, float value)
    {
        onModifierChanged?.Invoke(type, value);
    }

    public void UpdateModifier(Modifier type, float value)
    {
        if (isServer)
        {
            if (!Modifiers.ContainsKey(type))
                Modifiers.Add(type, value);
            else
                Modifiers[type] = value;
        }
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

        if (!hasAuthority && mapObject != null)
            mapObject.Name = displayName;

    }

    public static string LocalTeamId;
    private void UpdateTeamId(string oldValue, string newValue)
    {
        if (hasAuthority)
            LocalTeamId = newValue;

        ui?.SetTeam(teamId == LocalTeamId);

        if (mapObject == null)
            return;

        if (!hasAuthority)
        {
            if (teamId != LocalTeamId)
            {
                mapObject.iconColor = ColorHelper.GetEnemyColor();
            }
            else mapObject.iconColor = ColorHelper.GetTeamColor();

        }
        else
        {

            mapObject.IconSize = new Vector2(32, 32);
            mapObject.iconColor = Color.white;
        }
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

    //[HideInInspector]
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


    public void Suicide()
    {
        CmdKillEntity();
    }

    public void Kill()
    {
        kinematic.velocity = Vector2.zero;
        SetEntityKilled();
    }

    [Command]
    private void CmdKillEntity()
    {
        SetEntityKilled();
    }

    private void SetEntityKilled() 
    {
        alive = false;

        RpcKillEntity();
    }

    [ClientRpc]
    private void RpcKillEntity()
    {
        OnKilled();
    }

    public Action onKilled;
    public virtual void OnDeath()
    {
        if (mapObject != null)
            mapObject.IsVisible = false;

        if (hasAuthority)
        {
            transform.localScale = new Vector3(1,1,1);
        }
    }
    public virtual void OnKilled()
    {
        OnDeath();
        onKilled?.Invoke();
    }

    public void Respawn()
    {
        if (isServer)
            alive = true;

        OnRespawn();
    }

    public virtual void OnRespawn()
    {
        if (mapObject != null)
            mapObject.IsVisible = true;

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
        OnEnterCheckpoint(checkpoint);
    }
    public void LeaveCheckpoint(Checkpoint checkpoint)
    {
        onLeaveCheckpoint?.Invoke(checkpoint);
        OnLeaveCheckpoint(checkpoint);
    }
    #endregion
}
