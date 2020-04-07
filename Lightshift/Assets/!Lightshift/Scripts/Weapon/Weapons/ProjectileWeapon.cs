using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    public override void OnWeaponFire()
    {
        base.OnWeaponFire();

        FireRound();
    }

    private void FireRound()
    {
        if (weaponData.projectileCount > 1)
        {
            gunSpreadInitial = -weaponData.spread / 2;
            gunSpacingInitial = weaponData.spacing / 2;
            gunSpreadIncrement = weaponData.spread / (weaponData.projectileCount - 1);
            gunSpacingIncrement = weaponData.spacing / (weaponData.projectileCount - 1);
        }
        else
        {
            gunSpacingInitial = 0f;
            gunSpreadInitial = 0f;
            gunSpreadIncrement = 0f;
            gunSpacingIncrement = 0f;
        }

        //Shooting audio
        SoundManager.Instance.Play(weaponData.shootSoundEffect, entity.transform.position, true);


        for (var i = 0; i < weaponData.projectileCount; i++)
        {
            var bullet = LSObjectPoolManager.Instance.GetUsableBullet(weaponData.projectileId).GetComponent<Bullet>();

            var offsetX = (gunSpacingInitial - i * gunSpacingIncrement);
            var offsetY = weaponData.bulletData.range;
            var angle = Mathf.Atan2(offsetY, offsetX) * Mathf.Rad2Deg + aimAngle + -90; //add or subtract 90 as you see fit, or swap X and Y and put negatives, etc

            // Setup the point at which bullets need to be placed based on all the parameters
            var bulletRandomness = weaponData.randomness;
            var initialPosition = entity.transform.position + (entity.transform.forward * (gunSpacingInitial - i * gunSpacingIncrement));
            var bulletPosition = new Vector3(initialPosition.x + offsetX + UnityEngine.Random.Range(0f, 1f) * bulletRandomness - bulletRandomness / 2,
                initialPosition.y + offsetY + UnityEngine.Random.Range(0f, 1f) * bulletRandomness - bulletRandomness / 2, 0f);

            //Init Posiition
            bullet.transform.position = bulletPosition;
            bullet.Initialize(entity: entity, bulletData: weaponData.bulletData, angle, entity.rigidBody.velocity);

            bullet.transform.localScale = weaponData.scale;

            var spriteRenderer = bullet.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null) spriteRenderer.color = weaponData.bulletData.bulletColor;

            bullet.gameObject.SetActive(true);
        }
    }
}

