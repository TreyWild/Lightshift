using Assets._Lightshift.Scripts.Network;
using Lightshift;
using Mirror;
using System;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public enum DamageType 
{
    Projectile,
    Sun,
}
public class DamageableObject : NetworkBehaviour
{

    [SerializeField] private AudioClip _deathSound;
    [SerializeField] private GameObject _deathEffect;

    private Heart _heart;
    private Shield _shield;
    private Entity _entity;
    private GameObject _respawnHandler;

    private void Awake()
    {
        _heart = GetComponent<Heart>();
        _shield = GetComponent<Shield>();
        _entity = GetComponent<Entity>();
        _entity.onKilled += () =>
        {
            var obj = Instantiate(_deathEffect, _entity.kinematic.position, _entity.kinematic.Transform.rotation);

            var sound = obj.AddComponent<AudioSource>();
            sound.clip = _deathSound;
            sound.rolloffMode = AudioRolloffMode.Logarithmic;
            sound.minDistance = 1;
            sound.maxDistance = 150;
            sound.dopplerLevel = 0.6f;
            sound.spread = 19;
            sound.loop = false;
            sound.Play();
            Destroy(sound.gameObject, _deathSound.length + 1);
        }; 
    }

    private void OnDestroy()
    {
        _heart = null;
        _shield = null;
        _entity = null;
        _respawnHandler = null;
    }

    public bool HitObject(Projectile projectile)
    {
        if (projectile.entity.Id == _entity.Id || _entity.isInCheckpoint || !_entity.alive || _entity.teamId == projectile.entity.teamId)
            return false;

        DamageObject(projectile.data.damage, DamageType.Projectile, projectile.entity, projectile.weapon.DisplayName);

        return true;
    }

    public bool DamageObject(float amount, DamageType type, Entity attacker = null, string weaponName = "")
    {
        // TO DO : Show particle effect

        if (Settings.ShowDamageText)
            DamageTextHandler.AddDamage(_entity, amount);

        if (isServer)
        {
            var isDead = DoDamage(amount);

            if (isDead)
            {
                switch (type)
                {
                    case DamageType.Sun:
                        KillEntity($"was killed by flying into the sun", "Suicide");
                        break;
                    case DamageType.Projectile:
                        weaponName = $"{attacker.displayName}'s {weaponName}";
                        KillEntity($"was killed by {weaponName}", attacker.displayName);
                        break;
                }
            }
        }


        return true;
    }

    public void KillEntity(string deathReason, string killerName, Entity attacker = null, bool suicide = false)
    {
        if (isServer)
        {
            //Can't kill the dead lol
            if (!_entity.alive)
                return;
            //Kill Entity
            if (suicide)
                _entity.Suicide();
            else _entity.Kill();

            if (_entity.GetType() == typeof(PlayerShip))
            {
                if (attacker != null && attacker.connectionToClient != null)
                    Communication.ShowUserAlert(attacker.connectionToClient, $"You killed {_entity.displayName}!", Communication.AlertType.ScreenDisplay);

                Communication.ShowUserAlert(_entity.connectionToClient, $"You {deathReason}!", Communication.AlertType.ScreenDisplay);

                Communication.ShowUserAlert(_entity.connectionToClient, $"{_entity.displayName} {deathReason}!", Communication.AlertType.SystemMessage);

                var player = FindObjectsOfType<Player>().Where(s => s.isLocalPlayer).FirstOrDefault();
                if (player == null)
                    return;

                player.EjectAllResources();

                // Get Respawn Handler
                if (_respawnHandler == null)
                    _respawnHandler = Instantiate(LightshiftNetworkManager.GetPrefab<PlayerRespawnHandler>());

                NetworkServer.Spawn(_respawnHandler, connectionToClient);

                var script = _respawnHandler.GetComponent<PlayerRespawnHandler>();
                script.Initialize(respawnTime: 5.3f, killerName);
            }
            // TO DO : HANDLE DROPS
        }
    }

    private bool DoDamage(float damage)
    {
        if (_shield != null)
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

        if (_heart != null)
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
                if (_entity.alive)
                    CmdKillEntity();
            }
        }
    }

    [Command]
    private void CmdKillEntity()
    {
        if (_entity.alive)
            KillEntity($"{_entity.displayName} self distructed.", "Suicide");
    }

}
