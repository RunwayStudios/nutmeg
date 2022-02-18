using System.Collections;
using UnityEngine;

namespace Gameplay.Items.Weapons
{
    public abstract class Weapon : Item
    {
        [SerializeField] protected WeaponData data;

        protected float fireRateCoolDown;
        protected int currentAmmunitionAmount;

        public float ReloadProgress { get; private set; }

        public override void Use()
        {
            if (ReloadProgress == 0f)
                Attack();
        }

        public void ReloadWeapon()
        {
            if (ReloadProgress == 0f && currentAmmunitionAmount != data.stats.magazineSize)
                StartCoroutine(ReloadEnumerator());
        }

        private float reloadTimer;
        private IEnumerator ReloadEnumerator()
        {
            while ((reloadTimer += Time.deltaTime) <= data.stats.reloadTime)
            {
                ReloadProgress = reloadTimer / data.stats.reloadTime;
                yield return null;
            }

            ReloadProgress = reloadTimer = 0f;
            currentAmmunitionAmount = data.stats.magazineSize;
        }
    
        private void Update()
        {
            UpdateFireRateTimer();
        }

        private void UpdateFireRateTimer()
        {
            fireRateCoolDown = Mathf.Clamp(fireRateCoolDown -= Time.deltaTime, 0f, float.MaxValue);
        }

        protected virtual bool Attack()
        {
            if (fireRateCoolDown != 0) return false;

            fireRateCoolDown = 1f / (data.stats.fireRate / 60f);
            return true;
        }
    }
}