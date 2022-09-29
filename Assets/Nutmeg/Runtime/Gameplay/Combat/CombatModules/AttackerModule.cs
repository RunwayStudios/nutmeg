using UnityEngine;
using UnityEngine.Events;

namespace Nutmeg.Runtime.Gameplay.Combat.CombatModules
{
    public class AttackerModule : CombatModule
    {
        [SerializeField] private DetectorModule detector;

        [Space] [SerializeField] private float attackDamage = 5f;
        [SerializeField] private float attackInterval = 5f;
        private float lastAttackTry;
        private bool attacking = false;

        [Space] [Header("Events")]
        [SerializeField] private UnityEvent OnStartAttacking;
        [SerializeField] private UnityEvent OnAttack;
        [SerializeField] private UnityEvent OnStopAttacking;


        public override void UpdateModule()
        {
            base.UpdateModule();

            if (ShouldTryAttack())
            {
                if (detector.TryGetTarget(out CombatEntity target))
                {
                    if (!attacking)
                    {
                        attacking = true;
                        OnStartAttacking.Invoke();
                    }
                    Attack(target);
                    OnAttack.Invoke();
                }
                else
                {
                    if (attacking)
                    {
                        attacking = false;
                        OnStopAttacking.Invoke();
                    }
                }
            }
        }


        protected virtual void Attack(CombatEntity target)
        {
            if (target.TryGetModule(typeof(DamageableModule), out CombatModule module))
            {
                ((DamageableModule)module).Damage(attackDamage, DamageType.Default);
            }
        }

        protected virtual bool ShouldTryAttack()
        {
            if (lastAttackTry + attackInterval > Time.time)
                return false;

            lastAttackTry = Time.time;
            return true;
        }
    }
}