using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kinematic : MonoBehaviour
{
    public Vector2 velocity;
    public float drag = 0.99f;
    public float mass = 1;

    public void Awake()
    {
        KinematicManager.AddObject(gameObject);
    }
    private void OnDestroy()
    {
        KinematicManager.RemoveObject(gameObject);
    }

    void Update()
    {
        if (velocity != Vector2.zero)
            transform.position += new Vector3(velocity.x, velocity.y, 0) * Time.deltaTime;

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
        transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y, angle);
    }

    public void AddForce(Vector2 force) 
    {
        velocity += force / mass;
    }

    public float rotation => transform.eulerAngles.z;
}
