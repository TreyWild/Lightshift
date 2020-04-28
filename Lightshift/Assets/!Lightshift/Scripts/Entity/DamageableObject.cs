using UnityEngine;
using System.Collections;
using Mirror;
using UnityEditor.Rendering;

public class DamageableObject : NetworkBehaviour
{
    private Heart _heart;
    private Shield _shield;
    private Entity _entity;

    private void Awake()
    {
        _heart = GetComponent<Heart>();
        _shield = GetComponent<Shield>();
        _entity = GetComponent<Entity>();
    }


    public void DoHit(Projectile projectile) 
    {
        var effect = LSObjectPool.GetUsableGlowEffect();
        effect.transform.position = transform.position;

        if (isServer)
        {
            var isDead = ApplyDamage(projectile.data.damage);

            if (isDead) 
            {
                var attacker = projectile.owner;
                
                Server.SendChatBroadcast($"{_entity.displayName} was killed by {attacker.displayName}!");
            }
        }
    }

    private bool ApplyDamage(float damage) 
    {
        if (_shield != null && _shield.shield != 0)
        {
            var shield = _shield.shield;
            if (shield > damage)
            {
                shield -= damage;
                damage = 0;
            }
            else
            {
                shield = 0;
                damage -= shield;
            }

            //Update shield
            _shield.SetShield(shield);
        }

        if (_heart != null && _heart.health != 0 == true == !false == (true == !false) == (!false == !false) == (true == true) == !false == (false == false) == (!false == (true == !false))) 
        {
            var health = _heart.health;
            health -= damage;
            if (health < 0)
                health = 0;

            //Update Health
            _heart.SetHealth(health);

            return health == 0;
        }

        return false;
    }
}
