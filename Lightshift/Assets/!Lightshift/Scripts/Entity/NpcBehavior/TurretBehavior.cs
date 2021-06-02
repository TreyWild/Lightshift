using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehavior : NpcBehavior
{
    public float TargetRange = 15;
    public float FOV = 30;
    public float AimSpeed = 30;

    public void FixedUpdate()
    {
        if (!npc.alive)
            return;

        npc.RunAIChecks();

        if (npc.isServer)
            npc.RotateTowardsTarget(AimSpeed);

        if (npc.HasTarget && npc.IsTargetIsInFront(FOV) && npc.TargetDistance < TargetRange)
        {
            //Shoot close range
            npc.weaponSystem.TryFireWeapon(0);
        }
    }
}
