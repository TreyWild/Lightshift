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
    private void Awake()
    {
        _thruster = Instantiate(PrefabManager.Instance.enginePrefab, transform).GetComponent<Thruster>();
        _kinematic = GetComponent<Kinematic>();
    }


    public void SetColor(Color color)
    {
        _thruster.SetColor(color);
    }

    public void Move(int axis, bool overDrive)
    {
        if (overDriveMultiplier != 0)
            if (axis == 1)
                _thruster.StartThruster(overDrive);
            else _thruster.StopThruster();

        if (hasAuthority)
        {
            float engineStr = acceleration * Time.fixedDeltaTime;
            if (overDrive)
                engineStr *= overDriveMultiplier;

            var maxSpeed = this.maxSpeed;
            if (overDrive)
                maxSpeed *= overDriveMultiplier;

            if (axis == 1)
            {
                if (overDrive && _kinematic.velocity.sqrMagnitude > maxSpeed * maxSpeed) //over max speed but boosting
                {
                    float speed = _kinematic.velocity.magnitude;
                    _kinematic.AddForce(transform.up * engineStr / 2);
                    _kinematic.velocity *= Mathf.Max(speed - engineStr / _kinematic.mass / 2, 0) / speed;
                }
                else if (_kinematic.velocity.sqrMagnitude > maxSpeed * maxSpeed) //over max speed, not boosting
                {
                    float speed = _kinematic.velocity.magnitude;
                    _kinematic.AddForce(transform.up * engineStr / 2);
                    _kinematic.velocity *= Mathf.Max(speed - engineStr / _kinematic.mass / 2, 0) / speed;
                }
                else //under max speed
                {
                    _kinematic.AddForce(transform.up * engineStr);
                }
            }
            if (axis == -1)
            {
                engineStr *= brakeForce / _kinematic.mass; //this uses mass
                if (_kinematic.velocity.sqrMagnitude > engineStr * engineStr)
                    _kinematic.velocity -= engineStr * _kinematic.velocity.normalized;
                else if (axis != 1)
                    _kinematic.velocity = Vector2.zero;
            }

            if (!overDrive)
                _kinematic.drag = 0.99f;
            else
                _kinematic.drag = 0.999f;
        }
    }
}