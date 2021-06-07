using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kinematic : MonoBehaviour
{
    public Vector2 velocity 
    {
        get => _rigidBody.velocity;
        set => _rigidBody.velocity = value;
    }
    public float drag 
    {
        get => _rigidBody.drag;
        set => _rigidBody.drag = value;
    }
    public float mass = 1;
    private Rigidbody2D _rigidBody;
    private Transform _transform { get; set; }
    public void Awake()
    {
        _transform = transform;
        _rigidBody = GetComponent<Rigidbody2D>();
        if (_rigidBody == null)
            _rigidBody = gameObject.AddComponent<Rigidbody2D>();
        KinematicManager.AddObject(gameObject);
        _rigidBody.gravityScale = 0;
        _rigidBody.useAutoMass = true;
        _rigidBody.angularDrag = 1;
        _rigidBody.drag = 1;
    }
    private void OnDestroy()
    {
        KinematicManager.RemoveObject(gameObject);
    }

    //void Update()
    //{
    //    if (velocity != Vector2.zero)
    //        _transform.position += new Vector3(velocity.x, velocity.y, 0) * Time.deltaTime;

    //}

    //public void Collide(Kinematic other) 
    //{
    //    var mass = this.mass + other.mass;

    //    AddForce(other.velocity*mass);
    //    other.AddForce(velocity*mass);
    //}

    //void FixedUpdate()
    //{
    //    if (velocity.sqrMagnitude < 0.000001)
    //        velocity = Vector2.zero;
    //    else if (drag != 1)
    //        velocity *= drag;
    //}
    public void SetDirection(float angle)
    {
        //_transform.eulerAngles = new Vector3(_transform.eulerAngles.x, _transform.eulerAngles.y, angle);
        _rigidBody.SetRotation(angle);
    }

    public void AddForce(Vector2 force) 
    {
        //velocity += force / mass;
        if (_rigidBody != null && force != Vector2.zero)
            _rigidBody.AddForce(force);
    }

    public float rotation
    {
        //get { return _transform.eulerAngles.z; }
        //set { _transform.eulerAngles = new Vector3(_transform.eulerAngles.x, _transform.eulerAngles.y, value); }
        get => _rigidBody.rotation;
        set 
            {
            _rigidBody.SetRotation(value);
            _rigidBody.angularVelocity = 0;
        }
    }

    public Vector2 position
    {
        //get { return new Vector2(_transform.position.x, _transform.position.y); }
        //set { _transform.position = new Vector3(value.x, value.y, _transform.position.z); }
        get => _rigidBody.position;
        set => _rigidBody.position = value;
    }

    public Transform Transform => _transform;
}
