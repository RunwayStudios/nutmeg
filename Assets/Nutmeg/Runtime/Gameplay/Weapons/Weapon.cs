using System.Collections;
using Nutmeg.Runtime.Gameplay.Items;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Weapons
{
    public abstract class Weapon : Item
    {
        protected float fireRateCoolDown;
        protected int currentAmmunitionAmount;

        public float ReloadProgress { get; private set; }

        private void Start() => ResetAmmunition();

        public override void Use()
        {
            if (ReloadProgress == 0f)
                Attack();
        }

        public void ReloadWeapon()
        {
            if(!Stats.HasType(StatType.ReloadTime))
                return;
            
            if (ReloadProgress == 0f && currentAmmunitionAmount != (int) Stats[StatType.MagazineSize])
                StartCoroutine(ReloadEnumerator());
        }

        private float reloadTimer;

        private IEnumerator ReloadEnumerator()
        {
            while ((reloadTimer += Time.deltaTime) <= Stats[StatType.ReloadTime])
            {
                ReloadProgress = reloadTimer / Stats[StatType.ReloadTime];
                yield return null;
            }

            ReloadProgress = reloadTimer = 0f;
            ResetAmmunition();
        }

        private void Update()
        {
            UpdateFireRateTimer();
        }

        private void UpdateFireRateTimer()
        {
            if(fireRateCoolDown <= 0f)
                return;
            fireRateCoolDown = Mathf.Clamp(fireRateCoolDown -= Time.deltaTime, 0f, float.MaxValue);
        }

        protected void ResetAmmunition() => currentAmmunitionAmount = (int) Stats[StatType.MagazineSize];

        protected virtual bool Attack()
        {
            if (fireRateCoolDown != 0) return false;

            fireRateCoolDown = 1f / (Stats[StatType.FireRate] / 60f);
            return true;
        }
    }
}