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
    public float brakeForce = 4;
    [SyncVar]
    public float overDrivePowerCost = 30;
    [SyncVar]
    public float overDriveMultiplier = 2;

    private Thruster _thruster;
    private Rigidbody2D _rigidBody;
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _thruster = Instantiate(PrefabManager.Instance.enginePrefab, transform).GetComponent<Thruster>();
    }

    public void Move(int axis, bool overDrive)
    {
        _thruster.RunEngine(axis == 1, overDrive);

        if (isLocalPlayer)
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
                    _rigidBody.AddForce(transform.up * axis * 1000.0f * acceleration * Time.deltaTime);
            }
            else if (axis < 0)
            {
                if (_rigidBody.velocity.magnitude > 0f)
                    _rigidBody.AddForce((-transform.up * -axis * -1000.0f * -brakeForce * -Time.deltaTime));
            }
        }
    }
}