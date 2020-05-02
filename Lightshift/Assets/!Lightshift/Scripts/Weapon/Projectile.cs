using UnityEngine;
using System.Collections;
using TMPro;

public class Projectile : MonoBehaviour
{
    public bool isAlive;
    public Entity owner;
    public BulletData data;

    private Kinematic _kinematic;
    private float _lifeTime;
    private float _remainingHits;
    private RaycastHit _raycast;
    private TrailRenderer _trailRenderer;
    private SpriteRenderer _renderer;

    private void Awake()
    {
        _kinematic = gameObject.AddComponent<Kinematic>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _renderer = GetComponent<SpriteRenderer>();
        _trailRenderer.Clear();
    }

    public void Initialize(Vector2 velocity, BulletData data, Sprite sprite, Color color) 
    {
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

            //Check for collision
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out _raycast, Time.fixedDeltaTime * _kinematic.velocity.magnitude * 2))
            {
                Debug.LogError("hit");
                DamageableObject damageableObject = _raycast.transform.GetComponentInParent<DamageableObject>();

                if (damageableObject != null)
                    damageableObject.DoHit(this);

                if (_remainingHits == 0)
                    SetAsDead();
                else
                    _remainingHits -= 1;
            }
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
}
