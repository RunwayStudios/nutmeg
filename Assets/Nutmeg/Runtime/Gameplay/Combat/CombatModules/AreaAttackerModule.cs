using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Combat.CombatModules
{
    public class AreaAttackerModule : AttackerModule
    {
        [SerializeField] int targetsBufferSize = 5;
        private CombatEntity[] targetsBuffer;
        
        
        public override void InitializeModule(CombatEntity entity)
        {
            base.InitializeModule(entity);

            targetsBuffer = new CombatEntity[targetsBufferSize];
        }

        
        public override void TryAttack()
        {
            int size = detector.GetTargetsNonAlloc(targetsBuffer);
            if (size > 0)
            {
                if (!attacking)
                {
                    OnStartAttacking.Invoke();
                }

                for (int i = 0; i < size; i++)
                {
                    Attack(targetsBuffer[i]);
                }

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
    }
}