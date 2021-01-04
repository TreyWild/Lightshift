using Lightshift;
using Mirror;
using System;
using System.Diagnostics;
using UnityEngine;

public class DamageableObject : NetworkBehaviour
{

    private Heart _heart;
    private Shield _shield;
    private Entity _entity;

    private GameObject _respawnHandler;
    private void Awake()
    {
        _heart = GetComponent<Heart>();
        _shield = GetComponent<Shield>();
        _entity = GetComponent<Entity>();
    }

    public bool HitObject(Projectile projectile)
    {
        if (projectile.entity.Id == _entity.Id || _entity.IsInSafezone || !_entity.alive || _entity.teamId == projectile.entity.teamId)
            return false;

        // TO DO : Show particle effect

        if (Settings.ShowDamageText)
            DamageTextHandler.AddDamage(_entity, projectile.data.damage);

        if (isServer)
        {
            var isDead = ApplyDamage(projectile.data.damage);

            if (isDead)
            {
                var attacker = EntityManager.GetEntity(projectile.entity.Id);

                KillEntity($"{_entity.displayName} was killed by {attacker.displayName}!", attacker);
            }
        }


        return true;
    }

    public void KillEntity(string deathReason, Entity attacker)
    {
        if (isServer)
        {
            //Kill Entity
            _entity.Kill();

            if (_entity.GetType() == typeof(PlayerShip))
            {
                if (_entity.Id != attacker.Id && attacker.connectionToClient != null)
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

                // Get Respawn Handler
                if (_respawnHandler == null)
                    _respawnHandler = Instantiate(LightshiftNetworkManager.GetPrefab<PlayerRespawnHandler>());

                NetworkServer.Spawn(_respawnHandler, connectionToClient);

                var script = _respawnHandler.GetComponent<PlayerRespawnHandler>();
                script.Initialize(respawnTime: 5.3f, attacker.displayName);
            }
            // TO DO : HANDLE DROPS
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


    private void Update()
    {
        if (!Settings.KeysLocked)
        {
            if (Input.GetKeyDown(Settings.SelfDestruct) && hasAuthority)
            {
                CmdKillEntity();
            }
        }
    }

    [Command]
    private void CmdKillEntity()
    {
        KillEntity($"{_entity.displayName} self distructed.", _entity);
    }
}
