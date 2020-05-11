using UnityEngine;
using System.Collections;
using TMPro;

public class Projectile : MonoBehaviour
{
    public bool isAlive;
    public string entityId;
    public BulletData data;
    public Weapon weapon;
    private Kinematic _kinematic;
    private float _lifeTime;
    private float _remainingHits;
    private RaycastHit _raycast;
    private TrailRenderer _trailRenderer;
    private SpriteRenderer _renderer;
    private CircleCollider2D _collider;
    private void Awake()
    {
        _kinematic = gameObject.AddComponent<Kinematic>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _renderer = GetComponent<SpriteRenderer>();
        _trailRenderer.Clear();
    }

    public void Initialize(Vector2 velocity, BulletData data, Sprite sprite, Color color) 
    {
        if (_collider == null)
            _collider = gameObject.AddComponent<CircleCollider2D>();

        _collider.isTrigger = true;
        //_collider.radius = (transform.localScale.x) / 2;
        this.data = data;
        _kinematic.velocity = velocity;
        _remainingHits = data.hitCount;
        _lifeTime = data.range;

        isAlive = true;

        _renderer.sprite = sprite;
        _renderer.color = color;
        _trailRenderer.Clear();
        _trailRenderer.enabled = true;
        _trailRenderer.emitting = true;
        _trailRenderer.material.color = color;
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
        }

        if (_lifeTime < 0)
            SetAsDead();
    }


    public void Move()
    {
        _kinematic.AddForce(transform.up * data.speed * Time.deltaTime);
    }

    private void SetAsDead() 
    {
        _trailRenderer.Clear();
        _trailRenderer.enabled = false;
        _trailRenderer.emitting = false;
        isAlive = false;
        gameObject.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        var damageable = collision.GetComponentInParent<DamageableObject>();
        if (damageable != null && damageable.HitObject(this))
        {
            //Play sound Effect
            if (weapon.HitSound != null)
                SoundManager.Play(weapon.HitSound, transform.position);

            if (weapon.hitEffectPrefab != null)
                Instantiate(weapon.hitEffectPrefab, transform.position, transform.rotation);

            if (--_remainingHits == 0)
                SetAsDead();
        }
    }
}
