using Nutmeg.Runtime.Gameplay.PlayerWeapons;
using Nutmeg.Runtime.Utility.InputSystem;
using UnityEngine.InputSystem;

namespace Nutmeg.Runtime.Gameplay.Combat.PlayerWeapons.Modules
{
    public abstract class AttackWepModule : WeaponModule
    {
        public override void InitializeModule(Weapon weapon, WeaponParent weaponParent)
        {
            base.InitializeModule(weapon, weaponParent);

            if (Weapon.LocalPlayerWeapon)
            {
                InputManager.Input.Player.Primary.started += Attack;
                InputManager.Input.Player.Primary.canceled += AttackCancelled;
            }
        }

        public override void DestroyModule()
        {
            if (Weapon.LocalPlayerWeapon)
            {
                InputManager.Input.Player.Primary.started -= Attack;
                InputManager.Input.Player.Primary.canceled -= AttackCancelled;
            }
        }

        public virtual void Attack(InputAction.CallbackContext context)
        {
        }

        public virtual void AttackCancelled(InputAction.CallbackContext context)
        {
        }
    }
}