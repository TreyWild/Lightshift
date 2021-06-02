using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Mufenz : NpcBehavior
{
    public void FixedUpdate()
    {
        if (!npc.alive)
            return;

        if (npc.isServer)
            npc.RunBasicFollowAI();

        if (npc.HasTarget && npc.IsTargetIsInFront(30) && npc.TargetDistance < 30) 
        {
            //Shoot close range
            npc.weaponSystem.TryFireWeapon(0);
        }
        else if (npc.HasTarget && npc.IsTargetIsInFront(80) && npc.TargetDistance > 30)
        {
            // Shoot Missiles
            npc.weaponSystem.TryFireWeapon(1);
        }
    }
}

