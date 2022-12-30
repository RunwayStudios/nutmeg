using Nutmeg.Runtime.Gameplay.Combat.PlayerWeapons.Modules;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Nutmeg.Runtime.Gameplay.Combat.CombatModules
{
    public class PlayerWeaponAttackerModule : AttackerModule
    {
        [SerializeField] private bool shootContinuous;
        [SerializeField] private Transform rotatingTransform;
        [SerializeField] private AttackWepModule attackModule;


        protected override void Attack(CombatEntity target)
        {
            attackModule.Attack(new InputAction.CallbackContext());

            if (!shootContinuous)
                attackModule.AttackCancelled(new InputAction.CallbackContext());
        }

        protected virtual void UpdateRotation(Vector3 targetPos)
        {
            rotatingTransform.LookAt(new Vector3(targetPos.x, rotatingTransform.position.y, targetPos.z));
        }

        protected virtual void StopAttack()
        {
            attackModule.AttackCancelled(new InputAction.CallbackContext());
        }

        public override void TryAttack()
        {
            if (detector.TryGetTarget(out CombatEntity target))
            {
                if (!attacking)
                {
                    OnStartAttacking.Invoke();
                    if (shootContinuous)
                        Attack(target);
                }
                
                UpdateRotation(target.transform.position);
                if (!shootContinuous)
                    Attack(target);

                if (!attacking)
                {
                    OnStartedAttacking.Invoke();
                    attacking = true;
                }

                OnAttack.Invoke();
            }
            else if (attacking)
            {
                attacking = false;
                if (shootContinuous)
                    StopAttack();
                OnStoppedAttacking.Invoke();
            }
        }
    }
}