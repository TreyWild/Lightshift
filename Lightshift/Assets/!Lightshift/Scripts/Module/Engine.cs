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
    public float brakeForce;
    [SyncVar]
    public float overDrivePowerCost;
    [SyncVar]
    public float overDriveMultiplier;

    private Thruster _thruster;
    private Kinematic _kinematic;
    private PlayerController _input;
    private void Awake()
    {
        _kinematic = GetComponent<Kinematic>();
        _thruster = Instantiate(PrefabManager.Instance.enginePrefab, transform).GetComponent<Thruster>();
        _input = GetComponent<PlayerController>();
    }

    public void Move(int axis, bool overDrive)
    {
        _thruster.RunEngine(axis == 1, overDrive);

        if (isLocalPlayer)
        {
            float engineStr = acceleration * Time.fixedDeltaTime;

            if (_input.Up)
            {
                if (_kinematic.velocity.sqrMagnitude > maxSpeed * maxSpeed)
                {
                    float speed = _kinematic.velocity.magnitude;
                    if (_input.OverDrive)
                    {
                        _kinematic.velocity *= (speed - engineStr) / speed;
                        _kinematic.AddForce(transform.up * engineStr * overDriveMultiplier);
                    }
                    else
                    {
                        _kinematic.AddForce(transform.up * engineStr);
                        _kinematic.velocity *= Mathf.Max(speed - engineStr * brakeForce, maxSpeed) / speed;
                    }

                }
                else
                {
                    _kinematic.AddForce(transform.up * acceleration * Time.fixedDeltaTime * overDriveMultiplier);
                }
            }
            if (_input.Down)
            {
                if (_kinematic.velocity.sqrMagnitude > acceleration * Time.fixedDeltaTime * acceleration * Time.fixedDeltaTime)
                    _kinematic.velocity -= _kinematic.velocity.normalized * acceleration * Time.fixedDeltaTime;
                else if (!_input.Up)
                    _kinematic.velocity = Vector2.zero;
            }

            if (!_input.OverDrive)
                _kinematic.drag = 0.99f;
            else
                _kinematic.drag = 0.999f;
        }
    }
}