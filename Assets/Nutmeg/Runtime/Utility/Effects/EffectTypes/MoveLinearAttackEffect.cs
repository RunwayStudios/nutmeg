using System;
using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using UnityEngine;

namespace Nutmeg.Runtime.Utility.Effects.EffectTypes
{
    public class MoveLinearAttackEffect : AttackEffect
    {
        [Tooltip("Speed in m/s")]
        [SerializeField] private float speed = 100;
        
        private float startTime;
        private float expectedLifeTime;


        private void Update()
        {
            float progress = (Time.time - startTime) / expectedLifeTime;
            transform.position = Vector3.Lerp(origin, target, progress);

            if (progress >= 1)
                Finished();
        }
        

        public override void Initialize(DamageInfo info, Action<GameObject> FinishedAction)
        {
            base.Initialize(info, FinishedAction);
            
            transform.position = origin;
            
            expectedLifeTime = Vector3.Distance(origin, target) / speed;
            startTime = Time.time;
        }
    }
}
