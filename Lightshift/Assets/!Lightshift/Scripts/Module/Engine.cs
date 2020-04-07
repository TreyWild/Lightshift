using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class Engine : NetworkBehaviour 
{
    [SyncVar]
    public float maxSpeed;
    [SyncVar]
    public float acceleration;
    [SyncVar]
    public float weight;
    [SyncVar]
    public float brakeForce;
    [SyncVar]
    public float overDrivePowerCost;
    [SyncVar]
    public float overDriveMultiplier;

    private Thruster _thruster;
    private Rigidbody2D _rigidBody;
    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _thruster = Instantiate(PrefabManager.Instance.enginePrefab, transform).GetComponent<Thruster>();
    }

    public void Move(int axis, bool overDrive)
    {
        _thruster.RunEngine(axis == -1, overDrive);

        if (isServer)
        {
            var maxSpeed = this.maxSpeed;
            var acceleration = this.acceleration;

            if (overDrive)
            {
                maxSpeed *= ((0.01f) + overDriveMultiplier);
                acceleration *= ((0.01f) + overDriveMultiplier);
            }

            /* Handle Movement */
            if (axis > 0)
            {
                if (_rigidBody.velocity.magnitude < maxSpeed * 1000.0f)
                    _rigidBody.AddForce(transform.forward * axis * 1000.0f * acceleration * Time.deltaTime);

                _rigidBody.drag = weight;
            }
            else if (axis < 0)
            {
                _rigidBody.drag += (weight + brakeForce) * Time.deltaTime;
            }
            else _rigidBody.drag += weight * Time.deltaTime;
        }
    }
}