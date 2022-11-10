using UnityEngine;
using UnityEngine.Events;

namespace Nutmeg.Runtime.Gameplay.Combat.CombatModules
{
    public class AttackerModule : CombatModule
    {
        [SerializeField] protected DetectorModule detector;

        [Space] [SerializeField] protected float attackDamage = 5f;
        [SerializeField] protected bool autoAttack = true;
        [SerializeField] private float attackInterval = 5f;
        private float lastAttackTry;
        protected bool attacking = false;

        [Space] [Header("Events")] [SerializeField]
        protected UnityEvent OnStartAttacking;

        [SerializeField] protected UnityEvent OnStartedAttacking;
        [SerializeField] protected UnityEvent OnAttack;
        [SerializeField] protected UnityEvent OnStoppedAttacking;


        public override void UpdateModule()
        {
            if (ShouldTryAttack()) 
                TryAttack();
        }


        protected virtual void Attack(CombatEntity target)
        {
            if (target.TryGetModule(typeof(DamageableModule), out CombatModule module))
            {
                ((DamageableModule)module).Damage(attackDamage);
            }
        }
        
        public virtual void TryAttack()
        {
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
            else if (attacking)
            {
                attacking = false;
                OnStoppedAttacking.Invoke();
            }
        }

        protected virtual bool ShouldTryAttack()
        {
            if (!autoAttack || lastAttackTry + attackInterval > Time.time)
                return false;

            lastAttackTry = Time.time;
            return true;
        }
    }
}