using Lightshift;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : NetworkBehaviour
{
    [SyncVar]
    public bool LightLance;
    [SyncVar]
    public bool OverDrive;
    [SyncVar]
    public bool Down;
    [SyncVar]
    public bool Up;
    [SyncVar]
    public bool Left;
    [SyncVar]
    public bool Right;
    [SyncVar]
    public bool Weapon;
    [SyncVar]
    public int WeaponSlot;
    public int VerticalAxis => GetAxis(Up, Down);
    public int HorizontalAxis => GetAxis(Left, Right);

    public bool LocalLightLance;
    public bool LocalOverDrive;
    public bool LocalDown;
    public bool LocalUp;
    public bool LocalLeft;
    public bool LocalRight;
    public bool LocalWeapon;
    public int LocalWeaponSlot;
    public int LocalVerticalAxis => GetAxis(LocalUp, LocalDown);
    public int LocalHorizontalAxis => GetAxis(LocalLeft, LocalRight);

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
            LocalDown = down;
        }

        var up = Input.GetKey(Settings.Instance.UpKey);
        if (up != Up)
        {
            CmdUpdateUp(up);
            LocalUp = up;
        }
        

        var left = Input.GetKey(Settings.Instance.LeftKey) || Settings.Instance.UseMouseAim && GetMouseAimInput() == -1;
        if (left != Left)
        {
            CmdUpdateLeft(left);
            LocalLeft = left;
        }

        var right = Input.GetKey(Settings.Instance.RightKey) || Settings.Instance.UseMouseAim && GetMouseAimInput() == 1;
        if (right != Right)
        {
            CmdUpdateRight(right);
            LocalRight = right;
        }

        if (Settings.Instance.UseMouseAim && GetMouseAimInput() == 0)
        {
            LocalRight = false;
            LocalLeft = false;
            CmdUpdateRight(false);
            CmdUpdateLeft(false);
        }


        var overDrive = Input.GetKey(Settings.Instance.OverdriveKey);
        if (overDrive != OverDrive)
        {
            CmdUpdateOverDrive(overDrive);
            LocalOverDrive = overDrive;
        }

        var lightLance = Input.GetKey(Settings.Instance.LightLanceKey);
        if (lightLance != LightLance)
        {
            CmdUpdateLightLance(lightLance);
            LocalLightLance = lightLance;
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
            LocalWeapon = weapon;
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
    }

    [Command]
    public void CmdUpdateUp(bool value)
    {
        Up = value;
    }

    [Command]
    public void CmdUpdateLeft(bool value)
    {
        Left = value;
    }

    [Command]
    public void CmdUpdateRight(bool value)
    {
        Right = value;
    }

    [Command]
    public void CmdUpdateOverDrive(bool value)
    {
        OverDrive = value;
    }

    [Command]
    public void CmdUpdateLightLance(bool value)
    {
        LightLance = value;
    }

    [Command]
    public void CmdUpdateWeapon(bool value)
    {
        Weapon = value;
    }

    [Command]
    public void CmdUpdateWeaponSlot(short value)
    {
        WeaponSlot = value;
    }
}
