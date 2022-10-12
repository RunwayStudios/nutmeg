using System;
using UnityEngine;

namespace Nutmeg.Runtime.Utility.Effects.AttackEffects
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
        

        public override void Initialize(Vector3 origin, Vector3 target, Action<GameObject> FinishedAction)
        {
            transform.position = origin;
            
            base.Initialize(origin, target, FinishedAction);
            
            expectedLifeTime = Vector3.Distance(origin, target) / speed;
            startTime = Time.time;
        }
    }
}
