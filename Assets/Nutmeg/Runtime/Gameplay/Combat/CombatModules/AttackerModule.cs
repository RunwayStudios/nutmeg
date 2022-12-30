using UnityEngine;
using UnityEngine.Events;

namespace Nutmeg.Runtime.Gameplay.Combat.CombatModules
{
    public class AttackerModule : CombatModule
    {
        [SerializeField] protected DetectorModule detector;

        [Space] [SerializeField] protected float attackDamage = 5f;
        [SerializeField] protected DamageType attackDamageType;
        [SerializeField] protected Transform attackSource;
        [Space]
        [SerializeField] protected bool autoAttack = true;
        [SerializeField] private float attackInterval = 5f;
        private float lastAttackTry;
        protected bool attacking;

        [Space] [Header("Events")] [SerializeField]
        protected UnityEvent OnStartAttacking;

        [SerializeField] protected UnityEvent OnStartedAttacking;
        [SerializeField] protected UnityEvent OnAttack;
        [SerializeField] protected UnityEvent OnStoppedAttacking;


        public override void InitializeModule(CombatEntity entity)
        {
            base.InitializeModule(entity);

            if (!attackSource)
                attackSource = transform;
        }

        public override void UpdateModule()
        {
            if (ShouldTryAttack()) 
                TryAttack();
        }


        protected virtual void Attack(CombatEntity target)
        {
            if (target.TryGetModule(typeof(DamageableModule), out CombatModule module))
            {
                ((DamageableModule)module).Damage(attackDamage, attackDamageType, attackSource.position, target.transform.position);
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