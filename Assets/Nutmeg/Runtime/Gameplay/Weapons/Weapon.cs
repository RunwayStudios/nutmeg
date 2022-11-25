using System;
using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using Nutmeg.Runtime.Gameplay.Items;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Weapons
{
    public class Weapon : Item
    {
        public WeaponStats stats;
        public WeaponDamageComponent hitComponent;
        public WeaponComponent ammunitionComponent;
        public WeaponPoolComponent poolComponent;
        public WeaponComponent originComponent;
        public WeaponDamageOverrideComponent damageOverrideComponent;

        private float fireRateCooldown;

        [HideInInspector] public WeaponPreset preset;

        protected void Start()
        {
            ammunitionComponent = TryGetComponent(typeof(WeaponAmmunitionComponent), out var wac)
                ? (WeaponComponent) wac
                : null;
            originComponent = TryGetComponent(typeof(WeaponOriginComponent), out var woc)
                ? (WeaponComponent) woc
                : null;
            poolComponent = TryGetComponent(typeof(WeaponPoolComponent), out var wbc)
                ? (WeaponPoolComponent) wbc
                : null;
            damageOverrideComponent = TryGetComponent(typeof(WeaponDamageOverrideComponent), out var wdoc)
                ? (WeaponDamageOverrideComponent) wdoc
                : null;
        }

        private void Update()
        {
            UpdateFireRate();
        }

        public override void Use()
        {
            return;
            
            if (fireRateCooldown > 0f || ammunitionComponent != null && !ammunitionComponent.Get(out object _))
                return;

            if (hitComponent.Get(out var dms))
            {
                AddDamage((DamageableModule[]) dms);
            }

            fireRateCooldown = 1f / (stats.fireRate / 60f);
        }

        private void AddDamage(DamageableModule[] dms)
        {
            if (damageOverrideComponent != null)
            {
                damageOverrideComponent.modules = dms;
                damageOverrideComponent.Get();
            }
            else
            {
                foreach (var m in dms)
                    m.Damage(stats.damage, stats.damageType);
            }
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
        ExplosionObject,
        GrenadeLauncher
    }
}