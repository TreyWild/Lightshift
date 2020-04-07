using Lightshift;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : NetworkBehaviour
{
    [SyncVar(hook = nameof(LightLanceUpdated))]
    public bool LightLance;
    [SyncVar(hook = nameof(OverDriveUpdated))]
    public bool OverDrive;
    [SyncVar(hook = nameof(VerticalInputUpdated))]
    public bool Down;
    [SyncVar(hook = nameof(VerticalInputUpdated))]
    public bool Up;
    [SyncVar(hook = nameof(HorizontalInputUpdated))]
    public bool Left;
    [SyncVar(hook = nameof(HorizontalInputUpdated))]
    public bool Right;
    [SyncVar(hook = nameof(WeaponUpdated))]
    public bool Weapon;
    [SyncVar(hook = nameof(WeaponSlotUpdated))]
    public int WeaponSlot;
    public int VerticalAxis => GetAxis(Down, Up);
    public int HorizontalAxis => GetAxis(Right, Left);

    public event Action<bool> OnLightLanceInputChanged;
    public event Action<bool> OnOverDriveInputChanged;
    public event Action<int> OnHorizontalInputChanged;
    public event Action<int> OnVerticalInputChanged;
    public event Action<bool> OnWeaponInputChanged;
    public event Action<int> OnWeaponSlotInputChanged;

    private int GetAxis(bool x, bool y) 
    {
        if (y)
            return 1;
        else if (x)
            return -1;
        else return 0;
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        var down = Input.GetKey(Settings.Instance.DownKey);
        if (down != Down)
            CmdUpdateDown(down);

        var up = Input.GetKey(Settings.Instance.UpKey);
        if (up != Up)
            CmdUpdateUp(up);

        var left = Input.GetKey(Settings.Instance.LeftKey) || Settings.Instance.UseMouseAim && GetMouseAimInput() == -1;
        if (left != Left)
            CmdUpdateLeft(left);

        var right = Right = Input.GetKey(Settings.Instance.RightKey) || Settings.Instance.UseMouseAim && GetMouseAimInput() == 1;
        if (right != Right)
            CmdUpdateRight(right);

        var overDrive = Input.GetKey(Settings.Instance.OverdriveKey);
        if (overDrive != OverDrive)
            CmdUpdateOverDrive(overDrive);

        var lightLance = Input.GetKey(Settings.Instance.LightLanceKey);
        if (lightLance != LightLance)
            CmdUpdateLightLance(lightLance);

        var weapon = Input.GetKey(Settings.Instance.FireKey) || (Settings.Instance.FireWithWeaponHotkeys && (
            Input.GetKey(Settings.Instance.Weapon1) ||
            Input.GetKey(Settings.Instance.Weapon2) ||
            Input.GetKey(Settings.Instance.Weapon3) ||
            Input.GetKey(Settings.Instance.Weapon4) ||
            Input.GetKey(Settings.Instance.Weapon5)));

        if (weapon != Weapon)
            CmdUpdateWeapon(weapon);
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


        if (angleDiff > 5)
            return 1;
        else if (angleDiff < -5)
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

    private void LightLanceUpdated(bool value, bool newValue) 
    {
        OnLightLanceInputChanged?.Invoke(newValue);
    }

    private void OverDriveUpdated(bool value, bool newValue)
    {
        OnOverDriveInputChanged?.Invoke(newValue);
    }

    private void HorizontalInputUpdated(bool value, bool newValue)
    {
        OnVerticalInputChanged?.Invoke(GetAxis(Right, Left));
    }

    private void VerticalInputUpdated(bool value, bool newValue)
    {
        OnVerticalInputChanged?.Invoke(GetAxis(Down, Up));
    }

    private void WeaponUpdated(bool value, bool newValue)
    {
        OnWeaponInputChanged?.Invoke(newValue);
    }

    private void WeaponSlotUpdated(int value, int newValue)
    {
        OnWeaponSlotInputChanged?.Invoke(newValue);
    }
}
