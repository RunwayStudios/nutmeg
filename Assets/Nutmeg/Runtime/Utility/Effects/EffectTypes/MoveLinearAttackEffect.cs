using System;
using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using UnityEngine;

namespace Nutmeg.Runtime.Utility.Effects.EffectTypes
{
    public class MoveLinearAttackEffect : AttackEffect
    {
        [Tooltip("Speed in m/s")]
        [SerializeField] private float speed = 100;
        [SerializeField] private float stayAtTargetForSeconds = 0.3f;
        [SerializeField] private TrailRenderer trailRenderer;
        
        private float startTime;
        private float expectedLifeTime;
        private float timeFinished;


        private void Update()
        {
            if (timeFinished != default)
            {
                if (timeFinished + stayAtTargetForSeconds < Time.time)
                    Finished();
                return;
            }
            
            float progress = (Time.time - startTime) / expectedLifeTime;
            transform.position = Vector3.Lerp(origin, target, progress);

            if (progress >= 1)
                timeFinished = Time.time;
        }
        

        public override void Initialize(DamageInfo info, Action<GameObject> finishedAction)
        {
            if (!info.SourcePosSpecified || !info.HitPosSpecified)
                throw new ArgumentException();

            Initialize(info.SourcePos, info.HitPos, finishedAction);
        }

        public override void Initialize(Vector3 sourcePos, Vector3 targetPos, Action<GameObject> finishedAction)
        {
            base.Initialize(sourcePos, targetPos, finishedAction);

            transform.position = origin;
            if (trailRenderer)
                trailRenderer.Clear();

            timeFinished = default;
            expectedLifeTime = Vector3.Distance(origin, target) / speed;
            startTime = Time.time;
        }
    }
}
