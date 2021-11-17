using UnityEngine;
using System.Collections;
using TMPro;

public class Projectile : MonoBehaviour
{
    public bool isAlive;
    public Entity entity;
    public BulletData data;
    public Weapon weapon;
    private Kinematic _kinematic;
    private float _lifeTime;
    private float _remainingHits;
    private RaycastHit _raycast;
    private TrailRenderer _trailRenderer;
    private SpriteRenderer _renderer;
    private BoxCollider2D _collider;
    private Kinematic _target;
    private float speed;
    private AudioSource _audioSource;

    public TrailRenderer GetTrailRenderer() 
    {
        return _trailRenderer;
    }

    private void Awake()
    {
        _kinematic = gameObject.AddComponent<Kinematic>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _audioSource = GetComponent<AudioSource>();
        _renderer = GetComponent<SpriteRenderer>();
        _trailRenderer.Clear();
    }

    public void Initialize(Vector2 velocity, BulletData data, Sprite sprite, Color color, Color trailColor) 
    {
        if (entity != null && entity.targetNeutral != null)
            _target = entity.targetNeutral.kinematic;

        if (_collider == null)
            _collider = gameObject.AddComponent<BoxCollider2D>();

        //if (_physicalCollider == null)
        //    _physicalCollider = gameObject.AddComponent<BoxCollider2D>();

        this.data = data;
        _kinematic.velocity = velocity;
        _remainingHits = data.hitCount;
        _lifeTime = data.range;

        isAlive = true;

        _kinematic.mass = data.weight;

        _renderer.sprite = sprite;
        _renderer.color = color;
        _renderer.sortingOrder = SortingOrders.BULLET;

        _collider.isTrigger = true;
        _collider.size = _renderer.sprite.bounds.size;
        _collider.offset = _renderer.sprite.bounds.center;

        //_physicalCollider.size = _renderer.sprite.bounds.size;
        //_physicalCollider.offset = _renderer.sprite.bounds.center;

        _trailRenderer.sortingOrder = SortingOrders.BULLET_TRAIL;
        _trailRenderer.Clear();
        _trailRenderer.enabled = true;
        _trailRenderer.emitting = true;
        _trailRenderer.material.color = trailColor;
        _trailRenderer.startColor = trailColor;
        _trailRenderer.endColor = trailColor;
        _trailRenderer.time = data.trailLength;
        _trailRenderer.startWidth = data.trailSize;
        speed = data.speed * 100;

        _audioSource.clip = weapon.ShootSound;
        _audioSource.Play();
    }

    void FixedUpdate()
    {
        // Bullet is not alive. Don't update anything.
        if (!isAlive)
            return;

        if (isAlive)
        {
            _lifeTime -= Time.fixedDeltaTime;

            Move();
            if (data.tracksTarget)
                RotateTowardsTarget();
        }

        if (_lifeTime < 0)
            SetAsDead();
    }


    public void Move()
    {
        _kinematic.AddForce(_kinematic.Transform.up * speed * Time.deltaTime);
    }

    private void RotateTowardsTarget() 
    {
        var targetAngle = GetTargetAngle();
        _kinematic.SetDirection(_kinematic.rotation + data.agility * targetAngle * Time.fixedDeltaTime);
    }

    private void SetAsDead()
    {
        _trailRenderer.Clear();
        _trailRenderer.enabled = false;
        _trailRenderer.emitting = false;
        isAlive = false;
        gameObject.SetActive(false);
        entity = null;
        weapon = null;
        _kinematic = null;
        _target = null;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        var damageable = collision.GetComponentInParent<DamageableObject>();
        if (damageable != null && damageable.HitObject(this))
        {

            if (weapon.hitEffectPrefab != null)
                Instantiate(weapon.hitEffectPrefab, _kinematic.position, _kinematic.Transform.rotation);

            _audioSource.clip = weapon.HitSound;
            _audioSource.Play();

            if (--_remainingHits == 0)
                SetAsDead();
        }
    }

    public int GetTargetAngle()
    {
        var target = _target;
        if (target == null)
            return 0;

        var currentAngle = _kinematic.rotation + 90;
        var targetAngle = Mathf.Atan2(target.position.y - _kinematic.position.y, target.position.x - _kinematic.position.x) * 57.29578f;
        var angleDiff = currentAngle - targetAngle;

        for (int i = 0; i < 2; i++)
        {
            if (angleDiff > 180)
                angleDiff -= 360;
            else if (angleDiff < -180)
                angleDiff += 360;
        }


        if (angleDiff > 1)
            return -1;
        else if (angleDiff < -1)
            return 1;
        else
        {
            return 0;
        }
    }
}
