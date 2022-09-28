using System.Collections;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Weapons
{
    public class WeaponAmmunitionComponent : WeaponComponent
    {
        private int currentAmmunition;
        private float reloadTimer;
        private bool reloading;

        protected override void Start()
        {
            base.Start();

            currentAmmunition = root.stats.maxAmmunition;
        }

        public override bool Get(out object data)
        {
            data = currentAmmunition;
            if (reloading) 
                return false;
            
            data = --currentAmmunition;
            if (currentAmmunition == 0)
                StartCoroutine(Reload());

            return currentAmmunition > 0;
        }

        private IEnumerator Reload()
        {
            reloading = true;
            while ((reloadTimer += Time.deltaTime) <= root.stats.reloadTime)
                yield return null;

            reloadTimer = 0f;
            reloading = false;
            currentAmmunition = root.stats.maxAmmunition;
        }
    }
}