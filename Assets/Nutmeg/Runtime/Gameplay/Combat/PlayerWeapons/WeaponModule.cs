using Nutmeg.Runtime.Gameplay.PlayerWeapons;
using Unity.Netcode;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Combat.PlayerWeapons
{
    public abstract class WeaponModule : NetworkBehaviour
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

        public virtual void DestroyModule()
        {
        }

        protected Weapon Weapon => weapon;

        protected WeaponParent WeaponParent => weaponParent;
    }
}