using Mirror;
using Smooth;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Entity : NetworkBehaviour
{
    public float TargetCheckSpeed = 0.5f;

    [SyncVar(hook = nameof(SetDisplayName))]
    public string displayName; 
    [SyncVar(hook = nameof(UpdateCollision))]
    public bool hasCollision;
    [SyncVar]
    public string teamId;

    public List<Collider2D> colliders;
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
    private void FixedUpdate()
    {
        _timeSinceLastTargetUpdate += Time.fixedDeltaTime;
        if (_timeSinceLastTargetUpdate > 0.2f) 
        {
            UpdateTargets();
            _timeSinceLastTargetUpdate = 0;
        }
    }


    #region Safezone
    private void UpdateCollision(bool oldValue, bool value)
    {
        foreach (var collider in colliders)
            collider.isTrigger = value;

        if (isServer)
            hasCollision = value;
    }

    public bool IsInSafezone;

    public void HandleSafeZone() 
    {
        if (!IsInSafezone)
            return;

        //if (shield < maxShield)
        //{
        //    shield += (maxShield / 10) * Time.deltaTime;
        //    if (shield > maxShield)
        //        shield = maxShield;
        //}
        //if (health < maxHealth)
        //{
        //    health += (maxHealth / 10) * Time.deltaTime;
        //    if (health > maxHealth)
        //        health = maxHealth;
        //}
    }
    public void OnEnterSafezone()
    {
        //ClearDamageObjects();
        IsInSafezone = true;
        //weaponSystem.WeaponSystemDisabled = true;

        if (isLocalPlayer)
            GameUIManager.Instance.ShowScreenText("Entering Safezone, Weapons Disabled");
    }
    public void OnLeaveSafezone()
    {
        IsInSafezone = false;
        //weaponSystem.WeaponSystemDisabled = false;
        if (isLocalPlayer)
            GameUIManager.Instance.ShowScreenText("Leaving Safezone, Weapons Active");
    }
    #endregion




    public void SetDisplayName(string oldValue = "", string displayName = "")
    {
        if (isLocalPlayer)
            return;

        displayName = displayName;

        _ui.SetName(displayName);
    }


    //#region Death & Respawn

    //public void SendDeath(DamageObject damageObject) 
    //{
    //    if (IsMe)
    //        ClientManager.Instance.Send("g", "death", damageObject.entityId, damageObject.weaponId);
    //}
    //public virtual void OnDeath(int entityId, string weaponId) 
    //{
    //    if (dataObject.isDead)
    //        return;

    //    var entity = EntityManager.Instance.GetEntity(entityId);
    //    if (IsMe)
    //    {
    //        if (entity != null)
    //        {
    //            CameraFollow.Instance.Target = entity.gameObject;
    //            GameUIManager.Instance.HandleRespawnScreen(showRespawn: true, dataObject.displayName);
    //        }
    //    }
    //    else if (entity.IsMe)
    //        GameUIManager.Instance.ShowAnnouncementText($"You killed {dataObject.displayName}!");


    //    var deathEffect = LSObjectPoolManager.Instance.GetUsableEffect(3/*Death Effect Id*/);
    //    deathEffect.transform.position = transform.position;
    //    deathEffect.SetActive(true);

    //    SetAsDead();
    //}

    //private void SetAsDead() 
    //{
    //    _damageQueue.Clear();
    //    gameObject.SetActive(false);
    //}

    //public void Respawn()
    //{
    //    dataObject.Reinitialize();

    //    if (IsMe)
    //    {
    //        GameUIManager.Instance.HandleRespawnScreen(showRespawn: false);
    //        CameraFollow.Instance.Target = gameObject;
    //    }
    //}
    //#endregion

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
