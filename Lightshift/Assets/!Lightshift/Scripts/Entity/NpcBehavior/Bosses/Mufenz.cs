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
        if (!behavior.alive)
            return;

        if (behavior.isServer)
            behavior.RunBasicFollowAI();

        if (behavior.HasTarget && behavior.IsTargetIsInFront(30) && behavior.TargetDistance < 15) 
        {
            //Shoot close range
            behavior.weaponSystem.TryFireWeapon(0);
        }
        else if (behavior.HasTarget && behavior.IsTargetIsInFront(90))
        {
            // Shoot Missiles
            behavior.weaponSystem.TryFireWeapon(1);
        }
    }
}

