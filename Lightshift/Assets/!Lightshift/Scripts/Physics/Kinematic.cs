using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kinematic : MonoBehaviour
{
    public Vector2 velocity;
    public float drag = 0.99f;
    public float mass = 1;

    private Transform _transform { get; set; }
    public void Awake()
    {
        _transform = transform;
        KinematicManager.AddObject(gameObject);
    }
    private void OnDestroy()
    {
        KinematicManager.RemoveObject(gameObject);
    }

    void Update()
    {
        if (velocity != Vector2.zero)
            _transform.position += new Vector3(velocity.x, velocity.y, 0) * Time.deltaTime;

    }

    //public void Collide(Kinematic other) 
    //{
    //    var mass = this.mass + other.mass;

    //    AddForce(other.velocity*mass);
    //    other.AddForce(velocity*mass);
    //}

    void FixedUpdate()
    {
        if (velocity.sqrMagnitude < 0.000001)
            velocity = Vector2.zero;
        else if (drag != 1)
            velocity *= drag;
    }
    public void SetDirection(float angle)
    {
        _transform.eulerAngles = new Vector3(_transform.eulerAngles.x, _transform.eulerAngles.y, angle);
    }

    public void AddForce(Vector2 force) 
    {
        velocity += force / mass;
    }

    public float rotation
    {
        get { return _transform.eulerAngles.z; }
        set { _transform.eulerAngles = new Vector3(_transform.eulerAngles.x, _transform.eulerAngles.y, value); }
    }

    public Vector2 position
    {
        get { return new Vector2(_transform.position.x, _transform.position.y); }
        set { _transform.position = new Vector3(value.x, value.y, _transform.position.z); }
    }

    public Transform Transform => _transform;
}
