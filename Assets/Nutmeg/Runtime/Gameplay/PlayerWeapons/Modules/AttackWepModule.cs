using System;
using Nutmeg.Runtime.Utility.InputSystem;
using UnityEngine.InputSystem;

namespace Nutmeg.Runtime.Gameplay.PlayerWeapons.Modules
{
    public abstract class AttackWepModule : WeaponModule
    {
        public override void InitializeModule(Weapon weapon, WeaponParent weaponParent)
        {
            base.InitializeModule(weapon, weaponParent);

            InputManager.Input.Player.Primary.started += Attack;
            InputManager.Input.Player.Primary.canceled += AttackCancelled;
        }

        public override void DestroyModule()
        {
            InputManager.Input.Player.Primary.started -= Attack;
            InputManager.Input.Player.Primary.canceled -= AttackCancelled;
        }

        protected virtual void Attack(InputAction.CallbackContext context)
        {
        }

        protected virtual void AttackCancelled(InputAction.CallbackContext context)
        {
        }
    }
}