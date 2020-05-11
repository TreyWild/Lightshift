using Mirror;
using System.Diagnostics;

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

    public bool HitObject(Projectile projectile) 
    {
        if (projectile.entityId == _entity.Id || _entity.IsInSafezone)
            return false;

        // TO DO : Show particle effect
        if (isServer)
        {
            var isDead = ApplyDamage(projectile.data.damage);

            if (isDead) 
            {
                var attacker = EntityManager.GetEntity(projectile.entityId);
                attacker.connectionToClient.Send(new YouKilledEntityMessage
                {
                    username = _entity.displayName
                });

                _entity.connectionToClient.Send(new YouWereKilledMessage
                {
                    killerEntityId = attacker.Id,
                    killerName = attacker.displayName
                });

                Server.SendChatBroadcast($"{_entity.displayName} was killed by {attacker.displayName}!");


                Instantiate(PrefabManager.Instance.deathEffectPrefab, transform.position, transform.rotation);
                SoundManager.PlayExplosion(transform.position);

                // TO DO : Handle Drops
                NetworkServer.Destroy(_entity.gameObject);
            }
        }


        return true;
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
