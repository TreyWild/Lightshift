using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BasicFollow : NpcBehavior
{
    public void Update()
    {
        if (!behavior.alive)
            return;

        if (behavior.isServer)
            behavior.RunBasicFollowAI();

        if (behavior.HasTarget && behavior.IsTargetIsInFront(40) && behavior.TargetDistance < 10)
        {
            //Shoot close range
            behavior.weaponSystem.TryFireWeapon(0);
        }
    }
}
