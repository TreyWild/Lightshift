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
    private float _magnitude;
    private float _remainingHits;
    private Vector2 _oldVelocity;
    private RaycastHit _raycast;
    private TrailRenderer _trailRenderer;
    private SpriteRenderer _renderer;

    private void Awake()
    {
        _kinematic = gameObject.AddComponent<Kinematic>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _trailRenderer.Clear();
    }

    public void Initialize(Vector2 velocity, Vector2 position, BulletData data, Sprite sprite, Color color) 
    {
        this.data = data;
        _kinematic.velocity = velocity;
        transform.position = position;
        _remainingHits = data.hitCount;
        _lifeTime = data.range;

        isAlive = true;

        _renderer.sprite = sprite;
        _renderer.color = color;
        _trailRenderer.enabled = true;
        _trailRenderer.material.color = color;
    }

    void FixedUpdate()
    {
        // Bullet is not alive. Don't update anything.
        if (!isAlive)
            return;

        //Reset local magnitude
        _magnitude = 0;

        if (isAlive)
        {
            _lifeTime -= Time.fixedDeltaTime;

            if (_oldVelocity != _kinematic.velocity)
            {
                _magnitude = _kinematic.velocity.magnitude;
                _oldVelocity = _kinematic.velocity;
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Atan2(-_kinematic.velocity.y, _kinematic.velocity.x) * Mathf.Rad2Deg, transform.eulerAngles.z);
            }

            transform.position += new Vector3(_kinematic.velocity.x, 0, _kinematic.velocity.y) * Time.fixedDeltaTime;

            //Check for collision
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out _raycast, Time.fixedDeltaTime * _magnitude * 2))
            {
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

    private void SetAsDead() 
    {
        _trailRenderer.Clear();
        _trailRenderer.enabled = false;
        isAlive = false;
        gameObject.SetActive(false);
    }
}
