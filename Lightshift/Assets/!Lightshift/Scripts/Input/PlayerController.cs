using Lightshift;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public bool LightLance;
    public bool OverDrive;
    public bool Down;
    public bool Up;
    public bool Left;
    public bool Right;
    public bool Weapon;
    public int WeaponSlot;
    public int VerticalAxis => GetAxis(Up, Down);
    public int HorizontalAxis => GetAxis(Left, Right);
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
        if (!isLocalPlayer)
            return;

        if (Input.GetKey(Settings.Instance.Weapon1))
            if (WeaponSlot != 0)
                CmdUpdateWeaponSlot(0);
        if (Input.GetKey(Settings.Instance.Weapon2))
            if (WeaponSlot != 1)
                CmdUpdateWeaponSlot(1);
        if (Input.GetKey(Settings.Instance.Weapon3))
            if (WeaponSlot != 2)
                CmdUpdateWeaponSlot(2);
        if (Input.GetKey(Settings.Instance.Weapon4))
            if (WeaponSlot != 3)
                CmdUpdateWeaponSlot(3);
        if (Input.GetKey(Settings.Instance.Weapon5))
            if (WeaponSlot != 4)
                CmdUpdateWeaponSlot(4);

        var down = Input.GetKey(Settings.Instance.DownKey);
        if (down != Down)
        {
            CmdUpdateDown(down);
            Down = down;
        }

        var up = Input.GetKey(Settings.Instance.UpKey);
        if (up != Up)
        {
            CmdUpdateUp(up);
            Up = up;
        }


        var left = Input.GetKey(Settings.Instance.LeftKey) || Settings.Instance.UseMouseAim && GetMouseAimInput() == -1;
        if (left != Left)
        {
            CmdUpdateLeft(left);
            Left = left;
        }

        var right = Input.GetKey(Settings.Instance.RightKey) || Settings.Instance.UseMouseAim && GetMouseAimInput() == 1;
        if (right != Right)
        {
            CmdUpdateRight(right);
            Right = right;
        }

        if (Settings.Instance.UseMouseAim && GetMouseAimInput() == 0)
        {
            Right = false;
            Left = false;
            CmdUpdateRight(false);
            CmdUpdateLeft(false);
        }


        var overDrive = Input.GetKey(Settings.Instance.OverdriveKey);
        if (overDrive != OverDrive)
        {
            CmdUpdateOverDrive(overDrive);
            OverDrive = overDrive;
        }

        var lightLance = Input.GetKey(Settings.Instance.LightLanceKey);
        if (lightLance != LightLance)
        {
            CmdUpdateLightLance(lightLance);
            LightLance = lightLance;
        }

        var weapon = Input.GetKey(Settings.Instance.FireKey) || (Settings.Instance.FireWithWeaponHotkeys && (
            Input.GetKey(Settings.Instance.Weapon1) ||
            Input.GetKey(Settings.Instance.Weapon2) ||
            Input.GetKey(Settings.Instance.Weapon3) ||
            Input.GetKey(Settings.Instance.Weapon4) ||
            Input.GetKey(Settings.Instance.Weapon5)));

        if (weapon != Weapon)
        {
            CmdUpdateWeapon(weapon);
            Weapon = weapon;
        }
    }

    public int GetMouseAimInput()
    {
        var currentAngle = transform.rotation.eulerAngles.z + 90;
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
        if (hasAuthority)
            return;

        WeaponSlot = value;
    }
}
