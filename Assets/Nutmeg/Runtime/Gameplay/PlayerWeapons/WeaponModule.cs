using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.PlayerWeapons
{
    public abstract class WeaponModule : MonoBehaviour
    {
        private Weapon weapon;
        private WeaponParent weaponParent;

        public virtual void InitializeModule(Weapon weapon, WeaponParent weaponParent)
        {
            this.weapon = weapon;
            this.weaponParent = weaponParent;
        }

        public virtual void UpdateModule()
        {
            
        }

        protected Weapon Weapon => weapon;

        public WeaponParent WeaponParent => weaponParent;
    }
}
