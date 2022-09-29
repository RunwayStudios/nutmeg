using System;
using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using Nutmeg.Runtime.Gameplay.Items;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Weapons
{
    public class Weapon : Item
    {
        public WeaponStats stats;
        public WeaponComponent hitComponent;
        public WeaponComponent ammunitionComponent;
        public WeaponComponent bulletComponent;
        public WeaponComponent originComponent;

        private float fireRateCooldown;

        [HideInInspector] public WeaponPreset preset;

        private void Start()
        {
            ammunitionComponent = TryGetComponent(typeof(WeaponAmmunitionComponent), out var wac)
                ? (WeaponComponent) wac
                : null;
            originComponent = TryGetComponent(typeof(WeaponOriginComponent), out var woc)
                ? (WeaponComponent) woc
                : null;
            bulletComponent = TryGetComponent(typeof(WeaponBulletComponent), out var wbc)
                ? (WeaponComponent) wbc
                : null;
        }

        private void Update()
        {
            UpdateFireRate();
        }

        public override void Use()
        {
            if (fireRateCooldown > 0f || ammunitionComponent == null || !ammunitionComponent.Get(out object _))
                return;

            if (hitComponent.Get(out object hit))
            {
                DamageableModule m = (DamageableModule) hit;

                m.Damage(stats.damage, stats.damageType);
            }

            fireRateCooldown = 1f / (stats.fireRate / 60f);
        }

        private void UpdateFireRate()
        {
            if (fireRateCooldown <= 0f)
                return;

            fireRateCooldown = Mathf.Clamp(fireRateCooldown -= Time.deltaTime, 0f, 1f / (stats.fireRate / 60f));
        }
    }

    public enum WeaponPreset
    {
        None,
        Melee,
        Rifle,
        ShotGun,
        RocketLauncher,
        GrenadeLauncher
    }
}