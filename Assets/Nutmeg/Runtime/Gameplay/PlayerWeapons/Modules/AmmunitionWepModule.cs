using Nutmeg.Runtime.Utility.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Nutmeg.Runtime.Gameplay.PlayerWeapons.Modules
{
    public class AmmunitionWepModule : WeaponModule
    {
        [SerializeField, Min(0)] private int magazineSize = 12;
        [SerializeField] private bool infiniteMagazine;
        [SerializeField, Min(0)] private int reserveAmmunition = 100;
        [SerializeField] private bool infiniteAmmo;
        [SerializeField, Min(0)] private float reloadTime = 3f;

        [Space] [SerializeField] private int curMagazineAmmunition;
        private bool reloading;
        private float reloadFinnish;


        public override void InitializeModule(Weapon weapon, WeaponParent weaponParent)
        {
            base.InitializeModule(weapon, weaponParent);

            curMagazineAmmunition = magazineSize;
            // if infinite magazine also infinite ammo
            infiniteAmmo = infiniteMagazine || infiniteAmmo;

            InputManager.Input.Player.Reload.performed += ReloadCallbackContext;
        }

        public override void DestroyModule()
        {
            base.DestroyModule();

            InputManager.Input.Player.Reload.performed -= ReloadCallbackContext;
        }

        public bool TryUseAmmo()
        {
            if (reloading)
                return false;

            if (curMagazineAmmunition > 0)
            {
                if (!infiniteMagazine)
                    curMagazineAmmunition--;
                return true;
            }

            Reload();
            return false;
        }

        private void ReloadCallbackContext(InputAction.CallbackContext cc) => Reload();

        private void Reload()
        {
            if (reloading || curMagazineAmmunition >= magazineSize || reserveAmmunition == 0)
                return;

            reloading = true;
            reloadFinnish = Time.time + reloadTime;
        }

        public override void UpdateModule()
        {
            base.UpdateModule();

            if (reloading && Time.time > reloadFinnish)
                FinishReloading();
        }

        private void FinishReloading()
        {
            if (infiniteAmmo)
            {
                curMagazineAmmunition = magazineSize;
            }
            else if (reserveAmmunition < magazineSize - curMagazineAmmunition)
            {
                curMagazineAmmunition += reserveAmmunition;
                reserveAmmunition = 0;
            }
            else
            {
                reserveAmmunition -= magazineSize - curMagazineAmmunition;
                curMagazineAmmunition = magazineSize;
            }

            reloading = false;
        }
    }
}