using Lightshift;
using Mirror;
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
    public Sprite mapIcon;

    [SyncVar]
    public short Id;

    public float TargetCheckSpeed = 0.5f;

    [SyncVar(hook = nameof(SetDisplayName))]
    public string displayName;

    [SyncVar]
    public string teamId;

    [HideInInspector]
    public SmoothSyncMirror smoothSync { get; set; }
    [HideInInspector]
    public Rigidbody2D rigidBody { get; set; }


    public Entity targetNeutral { get; set; }
    public Entity targetEntity { get; set; }

    [HideInInspector]
    public Kinematic kinematic;

    private EntityUI _ui;
    private float _timeSinceLastTargetUpdate = 0;
    public void Awake()
    {
        //weaponSystem = gameObject.AddComponent<WeaponSystem>();
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        kinematic = gameObject.GetComponent<Kinematic>();
        _ui = GetComponent<EntityUI>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        SetAlive();
    }

    public void Start()
    {
        EntityManager.AddEntity(this);
    }

    public void OnDestroy()
    {
        EntityManager.RemoveEntity(this);
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


    #region Safezone

    public bool IsInSafezone;

    public void HandleSafeZone() 
    {
        if (!IsInSafezone)
            return;

    }
    public virtual void OnEnterSafezone(Entity entity)
    {
        //ClearDamageObjects();
        IsInSafezone = true;

        if (hasAuthority)
            GameUIManager.Instance.ShowScreenText("Entering Safezone, Weapons Disabled");
    }
    public void OnLeaveSafezone(Entity entity)
    {
        IsInSafezone = false;
        //weaponSystem.WeaponSystemDisabled = false;
        if (hasAuthority)
            GameUIManager.Instance.ShowScreenText("Leaving Safezone, Weapons Active");
    }
    #endregion




    public void SetDisplayName(string oldValue = "", string displayName = "")
    {
        _ui.SetName(displayName);

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

            if (entities[i].IsInSafezone)
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
        if (newValue)
            SetAlive();
        else SetDead();
    }

    public void SetDead()
    {
        if (isServer)
            alive = false;

        OnDeath();
    }

    public virtual void OnDeath() 
    {
        Instantiate(PrefabManager.Instance.deathEffectPrefab, transform.position, transform.rotation);
        SoundManager.PlayExplosion(transform.position);
    }

    public void SetAlive()
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
        transform.position = position;
    }

    public void SetPosition(Vector2 pos) 
    {
        // Set respawn position
        if (isServer && connectionToClient != null)
            TargetRpcSetPosition(pos);
        else if (isServer)
            transform.position = pos;
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
}
