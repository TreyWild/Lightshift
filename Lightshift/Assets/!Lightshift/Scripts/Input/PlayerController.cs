using Lightshift;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public bool Locked;
    public bool LightLance;
    public bool OverDrive;
    public bool Down;
    public bool Up;
    public bool Left;
    public bool Right;
    public bool Weapon;
    public bool Drifting;
    public int WeaponSlot;
    public int VerticalAxis => GetAxis(Up, Down);
    public int HorizontalAxis => GetAxis(Left, Right);

    private Kinematic _kinematic;

    private void Awake()
    {
        _kinematic = GetComponent<Kinematic>();
    }
    private int GetAxis(bool x, bool y)
    {
        if (y)
            return -1;
        else if (x)
            return 1;
        else return 0;
    }

    private void Update()
    {
        if (Locked)
            return;

        if (!hasAuthority || Settings.KeysLocked)
            return;

        if (Input.GetKeyDown(Settings.Weapon1))
            if (WeaponSlot != 0)
                UpdateWeaponSlot(0);
        if (Input.GetKeyDown(Settings.Weapon2))
            if (WeaponSlot != 1)
                UpdateWeaponSlot(1);
        if (Input.GetKeyDown(Settings.Weapon3))
            if (WeaponSlot != 2)
                UpdateWeaponSlot(2);
        if (Input.GetKeyDown(Settings.Weapon4))
            if (WeaponSlot != 3)
                UpdateWeaponSlot(3);
        if (Input.GetKeyDown(Settings.Weapon5))
            if (WeaponSlot != 4)
                UpdateWeaponSlot(4);

        var down = Input.GetKey(Settings.DownKey);
        if (down != Down)
        {
            CmdUpdateDown(down);
            Down = down;
        }
        else
        {

            var up = Input.GetKey(Settings.UpKey);
            if (up != Up)
            {
                CmdUpdateUp(up);
                Up = up;
            }
        }

        if ((Settings.Steering == Settings.SteeringMode.Mouse && GetMouseAimInput() == 0) && (Left || Right))
        {
            Right = false;
            Left = false;
            CmdUpdateRight(false);
            CmdUpdateLeft(false);
        }
        else
        {

            var left = Input.GetKey(Settings.LeftKey) || Settings.Steering == Settings.SteeringMode.Mouse && GetMouseAimInput() == -1;
            if (left != Left)
            {
                CmdUpdateLeft(left);
                Left = left;
            }
            else
            {
                var right = Input.GetKey(Settings.RightKey) || Settings.Steering == Settings.SteeringMode.Mouse && GetMouseAimInput() == 1;
                if (right != Right)
                {
                    CmdUpdateRight(right);
                    Right = right;
                }
            }
        }



        var overDrive = Input.GetKey(Settings.OverdriveKey);
        if (overDrive != OverDrive)
        {
            CmdUpdateOverDrive(overDrive);
            OverDrive = overDrive;
        }

        var lightLance = Input.GetKey(Settings.LightLanceKey);
        if (lightLance != LightLance)
        {
            CmdUpdateLightLance(lightLance);
            LightLance = lightLance;
        }

        var drifting = Input.GetKey(Settings.DriftKey);
        if (drifting != Drifting)
        {
            CmdUpdateDrifting(drifting);
            Drifting = drifting;
        }

        var weapon = Input.GetKey(Settings.FireKey) || (Settings.FireWithWeaponHotkeys && (
            Input.GetKey(Settings.Weapon1) ||
            Input.GetKey(Settings.Weapon2) ||
            Input.GetKey(Settings.Weapon3) ||
            Input.GetKey(Settings.Weapon4) ||
            Input.GetKey(Settings.Weapon5)));

        if (weapon != Weapon)
        {
            CmdUpdateWeapon(weapon);
            Weapon = weapon;
        }
    }

    public int GetMouseAimInput()
    {
        var currentAngle = _kinematic.rotation + 90;
        var targetAngle = Mathf.Atan2(Input.mousePosition.y - Screen.height * 0.5f, Input.mousePosition.x - Screen.width * 0.5f) * 57.29578f;
        var angleDiff = currentAngle - targetAngle;

        for (int i = 0; i < 2; i++)
        {
            if (angleDiff > 180)
                angleDiff -= 360;
            else if (angleDiff < -180)
                angleDiff += 360;
        }


        if (angleDiff > 10)
            return 1;
        else if (angleDiff < -10)
            return -1;
        else
        {
            return 0;
        }
    }

    private void UpdateWeaponSlot(short slot) 
    {
        WeaponSlot = slot;
        CmdUpdateWeaponSlot(slot);
    }

    [Command]
    public void CmdUpdateDown(bool value)
    {
        Down = value;
        RpcUpdateDown(value);
    }

    [ClientRpc]
    public void RpcUpdateDown(bool value)
    {
        if (hasAuthority)
            return;

        Down = value;
    }

    [Command]
    public void CmdUpdateUp(bool value)
    {
        Up = value;
        RpcUpdateUp(value);
    }

    [ClientRpc]
    public void RpcUpdateUp(bool value)
    {
        if (hasAuthority)
            return;

        Up = value;
    }

    [Command]
    public void CmdUpdateLeft(bool value)
    {
        Left = value;
        RpcUpdateLeft(value);
    }

    [ClientRpc]
    public void RpcUpdateLeft(bool value)
    {
        if (hasAuthority)
            return;

        Left = value;
    }

    [Command]
    public void CmdUpdateRight(bool value)
    {
        Right = value;
        RpcUpdateRight(value);
    }

    [ClientRpc]
    public void RpcUpdateRight(bool value)
    {
        if (hasAuthority)
            return;

        Right = value;
    }

    [Command]
    public void CmdUpdateOverDrive(bool value)
    {
        OverDrive = value;
        RpcUpdateOverDrive(value);
    }
    [ClientRpc(channel = 2)]
    public void RpcUpdateOverDrive(bool value)
    {
        if (hasAuthority)
            return;

        OverDrive = value;
    }

    [Command]
    public void CmdUpdateLightLance(bool value)
    {
        LightLance = value;
        RpcUpdateLightLance(value);
    }

    [ClientRpc]
    public void RpcUpdateLightLance(bool value)
    {
        if (hasAuthority)
            return;

        LightLance = value;
    }

    [Command]
    public void CmdUpdateDrifting(bool value)
    {
        Drifting = value;
        RpcUpdateDrifting(value);
    }

    [ClientRpc]
    public void RpcUpdateDrifting(bool value)
    {
        if (hasAuthority)
            return;

        Drifting = value;
    }

    [Command]
    public void CmdUpdateWeapon(bool value)
    {
        Weapon = value;
        RpcUpdateWeapon(value);
    }

    [ClientRpc]
    public void RpcUpdateWeapon(bool value)
    {
        if (hasAuthority)
            return;

        Weapon = value;
    }

    [Command]
    public void CmdUpdateWeaponSlot(short value)
    {
        WeaponSlot = value;
        RpcUpdateWeaponSlot(value);
    }

    [ClientRpc]
    public void RpcUpdateWeaponSlot(short value) 
    {
        WeaponSlot = value;
    }
}
