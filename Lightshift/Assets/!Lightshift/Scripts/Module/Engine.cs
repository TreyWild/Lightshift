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
                if (_kinematic.velocity.sqrMagnitude > maxSpeed * maxSpeed * overDriveMultiplier * overDriveMultiplier && _input.OverDrive) //over max speed but boosting
                {
                    engineStr *= overDriveMultiplier;
                    _kinematic.AddForce(transform.up * engineStr/2);
                    _kinematic.AddForce(transform.transformPoint(_kinematic.velocity.normalized * -engineStr/2)) // push opposite of the direction of velocity
                }
                else if (_kinematic.velocity.sqrMagnitude > maxSpeed * maxSpeed) //over max speed, not boosting
                {
                    _kinematic.AddForce(transform.up * engineStr/2);
                    _kinematic.AddForce(transform.transformPoint(_kinematic.velocity.normalized * -engineStr/2)) // push opposite of the direction of velocity
                }
                else //under max speed
                {
                    _kinematic.AddForce(transform.up * engineStr);
                }
            }
            if (_input.Down)
            {
                engineStr *= brakeForce;
                if (_kinematic.velocity.sqrMagnitude > engineStr * engineStr) //this won't always work because the AddForce uses mass, so it will make a more sudden stop for very heavy ships.
                    _kinematic.AddForce(transform.up * -engineStr)
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