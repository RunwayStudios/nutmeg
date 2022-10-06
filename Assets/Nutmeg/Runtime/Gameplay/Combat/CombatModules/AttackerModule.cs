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
        [SerializeField] private UnityEvent OnStartedAttacking;
        [SerializeField] private UnityEvent OnAttack;
        [SerializeField] private UnityEvent OnStoppedAttacking;


        public override void UpdateModule()
        {
            if (!ShouldTryAttack()) return;
            
            if (detector.TryGetTarget(out CombatEntity target))
            {
                if (!attacking)
                {
                    OnStartAttacking.Invoke();
                }
                Attack(target);
                if (!attacking)
                {
                    OnStartedAttacking.Invoke();
                    attacking = true;
                }
                OnAttack.Invoke();
            }
            else
            {
                if (attacking)
                {
                    attacking = false;
                    OnStoppedAttacking.Invoke();
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