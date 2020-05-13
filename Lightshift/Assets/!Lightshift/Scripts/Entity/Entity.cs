using Lightshift;
using Mirror;
using Smooth;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Entity : NetworkBehaviour
{
    [SyncVar]
    public string Id;

    public float TargetCheckSpeed = 0.5f;

    [SyncVar(hook = nameof(SetDisplayName))]
    public string displayName; 
    [SyncVar]
    public string teamId;

    public SmoothSyncMirror smoothSync;
    public Rigidbody2D rigidBody;
    public ModuleData shipData;
    public Entity targetNeutral;
    public Entity targetEntity;


    private EntityUI _ui;
    private float _timeSinceLastTargetUpdate = 0;
    public void Awake()
    {
        //weaponSystem = gameObject.AddComponent<WeaponSystem>();
        rigidBody = gameObject.GetComponent<Rigidbody2D>();

        _ui = GetComponent<EntityUI>();
    }

    public void Start()
    {
        Instantiate(PrefabManager.Instance.spawnEffectPrefab, transform);
        EntityManager.AddEntity(this);
    }

    public void OnDestroy()
    {
        EntityManager.RemoveEntity(this);
    }

    public void FixedUpdate()
    {
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

        if (isLocalPlayer)
            GameUIManager.Instance.ShowScreenText("Entering Safezone, Weapons Disabled");
    }
    public void OnLeaveSafezone(Entity entity)
    {
        IsInSafezone = false;
        //weaponSystem.WeaponSystemDisabled = false;
        if (isLocalPlayer)
            GameUIManager.Instance.ShowScreenText("Leaving Safezone, Weapons Active");
    }
    #endregion




    public void SetDisplayName(string oldValue = "", string displayName = "")
    {
        _ui.SetName(displayName);

        if (isLocalPlayer)
            return;

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


    private float _trackingRange = 100;
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
            targetEntity = closestEntity;
            targetNeutral = closestNuetralEntity;
        }
    }

    private List<Entity> GetNearbyEntities()
    {
        var nearbyEntities = new List<Entity>();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _trackingRange);
        for (int i = 0; i < colliders.Length; i++)
        {
            var nearbyEntity = colliders[i].transform.root.GetComponent<Entity>();

            if (nearbyEntity == null)
                continue;

            nearbyEntities.Add(nearbyEntity);
        }

        return nearbyEntities;
    }
}
