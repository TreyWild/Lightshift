using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Npc : Ship
{
    private float _aimArc;
    private int _rotationMod;

    private SpriteRenderer _spriteRenderer;
    private PolygonCollider2D _collider;

    [SyncVar(hook = nameof(UpdateNpcData))]
    private string npcDataKey;

    //Simple hack to prevent scriptable stacking.
    [SerializeField]
    private NpcDataShell _npcDataShell;
    public NpcData npcData => _npcDataShell?.data;

    public Action onDataLoaded;

    private List<NpcBehavior> _behaviors = new List<NpcBehavior>();
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    public void Awake()
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    {
        base.Awake();
    }

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    public void Start()
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    {
        base.Start();
    }

    public override void OnStartServer()
    {
        if (isServer && npcData != null)
            LoadNpcData(npcData.key);
    }

    public void LoadNpcData(string key) 
    {
        if (key == "")
            return;

        npcDataKey = key;
        UpdateNpcData(key, key);
    }

    public void SetFaction(NpcFaction faction)
    {
        if (isServer)
            teamId = faction.ToString();
    }

    private void UpdateNpcData(string oldValue, string newValue)
    {
        _npcDataShell = new NpcDataShell {
            data = EntityManager.GetEntityData(newValue),
        };

        if (npcData == null)
            return;

        trackingRange = npcData.trackingRange;

        SetName(npcData.Name);
        SetWeapons(npcData.weapons);
        //stats = npcData.data;
        SetFaction(npcData.faction);
        LoadBehaviors(npcData.Behavior);

        if (npcData.scale == Vector2.zero)
            transform.localScale = new Vector3(1, 1, 1);
        transform.localScale = new Vector3(npcData.scale.x, npcData.scale.y, 1);

        //UpdateStats(true);
        onDataLoaded?.Invoke();
    }

    private Vector2 _spawnLocation;
    public void SetSpawnLocation(Vector2 spawnLocation) 
    {
        _spawnLocation = spawnLocation;
    }

    public void LoadBehaviors(List<UnityEngine.Object> npcBehaviors) 
    {
        var behaviors = new List<string>();
        foreach (var behavior in npcBehaviors) 
            behaviors.Add(behavior.name);

        for (int i = 0; i < _behaviors.Count; i++)
            Destroy(_behaviors[i]);
        _behaviors.Clear();

        foreach (var behavior in behaviors)
            gameObject.AddComponent(Type.GetType(behavior));
    }

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
        //MoveForwards(stats.maxSpeed);
        //RotateTowardsTarget(stats.agility);
        RunAIChecks();
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
        kinematic.SetDirection(kinematic.transform.eulerAngles.z + speed * _rotationMod * Time.fixedDeltaTime);
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

    public bool HasTarget => targetNeutral != null && TargetDistance < trackingRange && targetNeutral.alive && !targetNeutral.IsInSafezone;

    public float TargetDistance => Vector2.Distance(targetNeutral.transform.position, transform.position);

    public event Action<Npc> onDeath;
    public override void OnDeath()
    {
        base.OnDeath();
        //stats = new ModuleData { };
        onDeath?.Invoke(this);
    }
}