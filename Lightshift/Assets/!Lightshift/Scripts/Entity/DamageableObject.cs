using Lightshift;
using Mirror;
using System.Diagnostics;
using UnityEngine;

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

                KillEntity($"{_entity.displayName} was killed by {attacker.displayName}!", attacker);
            }
        }


        return true;
    }

    public void KillEntity(string deathReason, Entity attacker)
    {
        if (isServer)
        {
            if (_entity.Id != attacker.Id)
                attacker.connectionToClient.Send(new YouKilledEntityMessage
                {
                    username = _entity.displayName
                });

            _entity.connectionToClient.Send(new YouWereKilledMessage
            {
                killerEntityId = attacker.Id,
                killerName = attacker.displayName
            });

            Server.SendChatBroadcast(deathReason);

            ShowDeathEffect();
            RpcKillEntity();

            if (_entity.GetType() == typeof(PlayerShip))
            {
                // If player: Create Respawn Handler
                var respawnHandler = Instantiate(NetworkManager.singleton.spawnPrefabs[PrefabManager.RESPAWN_PREFAB_ID]);
                NetworkServer.Spawn(respawnHandler, connectionToClient);

                var script = respawnHandler.GetComponent<PlayerRespawnHandler>();
                script.Initialize(respawnTime: 5.3f, attacker.displayName);
            }
            
            NetworkServer.Destroy(_entity.gameObject);

            // TO DO : HANDLE DROPS
        }
    }

    [ClientRpc]
    private void RpcKillEntity()
    {
        ShowDeathEffect();
    }

    private void ShowDeathEffect()
    {
        Instantiate(PrefabManager.Instance.deathEffectPrefab, transform.position, transform.rotation);
        SoundManager.PlayExplosion(transform.position);
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


    private void Update()
    {
        if (!Settings.Instance.KeysLocked)
        {
            if (Input.GetKeyDown(Settings.Instance.SelfDestruct) && hasAuthority)
            {
                CmdKillEntity();
            }
        }
    }

    [Command]
    private void CmdKillEntity()
    {
        KillEntity($"{_entity.displayName} committed die", _entity);
    }
}
