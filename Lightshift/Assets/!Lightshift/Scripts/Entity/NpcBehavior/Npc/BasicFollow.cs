using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BasicFollow : NpcBehavior
{
    public void Update()
    {
        if (!npc.alive)
            return;

        if (npc.isServer)
            npc.RunBasicFollowAI();

        if (npc.HasTarget && npc.IsTargetIsInFront(40) && npc.TargetDistance < 10)
        {
            //Shoot close range
            npc.weaponSystem.TryFireWeapon(0);
        }
    }
}
