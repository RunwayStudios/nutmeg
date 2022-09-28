using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using Nutmeg.Runtime.Gameplay.Items;
using Nutmeg.Runtime.Gameplay.Weapons.Editor;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Weapons
{
    public class Weapon : Item
    {
        public WeaponStats stats;
        public WeaponComponent hitComponent;
        public WeaponComponent originComponent;

        private float fireRateCooldown;

        [HideInInspector]public WeaponPreset preset;
        
        private void Update()
        {
            UpdateFireRate();
        }

        public override void Use()
        {
            if (fireRateCooldown > 0f)
                return;

            hitComponent.Get(out object hit);
            DamageableModule m = (DamageableModule) hit;

            m.Damage(stats.damage, stats.damageType);
            fireRateCooldown = stats.fireRate / (1000f / 60f);
        }

        private void UpdateFireRate()
        {
            if (fireRateCooldown <= 0f)
                return;

            fireRateCooldown = Mathf.Clamp(fireRateCooldown -= Time.deltaTime, 0f, stats.fireRate / (1000f / 60f));
        }
    }
    
}