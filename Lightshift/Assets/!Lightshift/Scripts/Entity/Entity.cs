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
    public string username; 
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

    public void SetupStats(ModuleData moduleData) 
    {
        //maxHealth = moduleData.maxHealth;
        //maxPower = moduleData.maxPower;
        //maxShield = moduleData.maxShield;
        //maxSpeed = moduleData.maxSpeed;
        //acceleration = moduleData.acceleration;
        //agility = moduleData.agility;
        //weight = moduleData.weight;
        //brakeForce = moduleData.brakeForce;
        //shieldRegen = moduleData.shieldRegen;
        //healthRegen = moduleData.healthRegen;
        //powerRegen = moduleData.powerRegen;
        //overDriveMultiplier = moduleData.overDriveBoostMultiplier;
        //overDrivePowerCost = moduleData.overDrivePowerCost;
        //lightLancePowerCost = moduleData.lightLancePowerCost;
        //lightLanceRange = moduleData.lightLanceRange;
        //lightLancePullForce = moduleData.lightLancePullForce;

        //health = maxHealth;
        //power = maxPower;
        //shield = maxShield;
    }

    #region Damage Handler
    //private List<DamageObject> _damageQueue = new List<DamageObject>();
    //private float _lastDamageTick;
    //public void HandleDamageQueue()
    //{
    //    _lastDamageTick -= Time.deltaTime * 1.0f;

    //    for (int i = 0; i < _damageQueue.Count; i++)
    //    {
    //        var dmgObj = _damageQueue[i];

    //        dmgObj.effectDuration -= Time.deltaTime * 1.0f;
    //        dmgObj.startTime += Time.deltaTime * 1.0f;

    //        var damage = CalculateDamage(dmgObj);

    //        if (dmgObj.IsDot)
    //            damage = damage / dmgObj.totalEffectDuration * Time.deltaTime;
    //        if (dmgObj.IsBurn)
    //            damage = damage / dmgObj.totalEffectDuration * 2 * (dmgObj.effectDuration / dmgObj.totalEffectDuration) * Time.deltaTime;

    //        DamageEntity(damage, dmgObj);

    //        if (dmgObj.effectDuration < 0)
    //            _damageQueue.Remove(dmgObj);
    //    }
    //}
    ////public void HitEntity(Entity attacker, WeaponConfig weapon, PropertiesObject weaponProperties, ModifierObject weaponModifiers) 
    ////{
    ////    var damageObject = _damageQueue.FirstOrDefault(d => d.entityId == attacker.uniqueId && weapon.uniqueId == d.weaponId);

    ////    if (damageObject == null)
    ////    {
    ////        //New Damage Object
    ////        damageObject = new DamageObject
    ////        {
    ////            corrosiveDamage = weaponModifiers.Get(Modifiers.CorrosiveDamage),
    ////            energyDamage = weaponModifiers.Get(Modifiers.EnergyDamage),
    ////            kineticDamage = weaponModifiers.Get(Modifiers.KineticDamage),
    ////            IsBurn = weaponProperties.GetBool(Properties.IsBurn),
    ////            IsDot = weaponProperties.GetBool(Properties.IsDot),
    ////            IsStacking = weaponProperties.GetBool(Properties.IsStacking),
    ////            effectDuration = weaponModifiers.Get(Modifiers.EffectDuration),
    ////            totalEffectDuration = weaponModifiers.Get(Modifiers.EffectDuration),
    ////            entityId = attacker.uniqueId,
    ////            weaponId = weapon.uniqueId
    ////        };

    ////        //Apply damage
    ////        DamageEntity(CalculateDamage(damageObject), damageObject);

    ////        //If dot or burn, store to be handled.
    ////        if (damageObject.IsBurn || damageObject.IsDot)
    ////        {
    ////            damageObject.corrosiveDamage = weaponModifiers.Get(Modifiers.CorrosiveDamageOverTime);
    ////            damageObject.energyDamage = weaponModifiers.Get(Modifiers.EnergyDamageOverTime);
    ////            damageObject.kineticDamage = weaponModifiers.Get(Modifiers.KineticDamageOverTime);
    ////            _damageQueue.Add(damageObject);

    ////        }
    ////    }
    ////    else
    ////    {
    ////        //Update Existing Damage Object
    ////        if (damageObject.IsStacking) {
    ////            damageObject.effectDuration = weaponModifiers.Get(Modifiers.EffectDuration);
    ////            damageObject.corrosiveDamage += weaponModifiers.Get(Modifiers.CorrosiveDamage);
    ////            damageObject.energyDamage += weaponModifiers.Get(Modifiers.EnergyDamage);
    ////            damageObject.kineticDamage += weaponModifiers.Get(Modifiers.KineticDamage);
    ////        }
    ////    }
    ////}

    //private float CalculateDamage(DamageObject damageObject) 
    //{
    //    float damage = 0;

    //    if (IsShielded)
    //    {
    //        if (damageObject.corrosiveDamage > 0)
    //            damage = (1 - dataObject.corrosiveResistance) * damageObject.corrosiveDamage;
    //        if (damageObject.kineticDamage > 0)
    //            damage = (1 - dataObject.kineticResistance) * damageObject.kineticDamage;
    //        if (damageObject.energyDamage > 0)
    //            damage = (1 - dataObject.energyResistance) * damageObject.energyDamage;
    //    }
    //    else damage = Mathf.Max((damageObject.kineticDamage + damageObject.energyDamage + damageObject.corrosiveDamage) - dataObject.armor, damage * 0.15f);

    //    return damage;
    //}

    //private void DamageEntity(float damage, DamageObject damageObject) 
    //{
    //    var difference = dataObject.shield - damage;

    //    if (difference <= 0)
    //    {
    //        dataObject.health += difference;
    //        dataObject.shield = 0;

    //        if (_visualShield != null && _visualShield.activeInHierarchy)
    //            _visualShield.SetActive(false);
    //    }
    //    else
    //    {
    //        dataObject.shield = difference;
    //        if (_visualShield != null && !_visualShield.activeInHierarchy)
    //            _visualShield.SetActive(true);
    //    }

    //    _lastHitTime = DateTime.Now;

    //    if (dataObject.health <= 0)
    //        SendDeath(damageObject);
    //}

    //private DateTime _lastHitTime;

    //public void ClearDamageObjects() 
    //{
    //    //Clear damage objects
    //    _damageQueue.Clear();
    //}
    #endregion
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

        username = displayName;

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
