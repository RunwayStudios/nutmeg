using System;
using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using UnityEngine;

namespace Nutmeg.Runtime.Utility.Effects.EffectTypes
{
    public abstract class AttackEffect : Effect
    {
        protected Vector3 origin;
        protected Vector3 target;


        public override void Initialize(DamageInfo info, Action<GameObject> finishedAction)
        {
            if (!info.SourcePosSpecified || !info.HitPosSpecified)
                throw new ArgumentException();
            
            origin = info.SourcePos;
            target = info.HitPos;
            
            base.Initialize(finishedAction);
        }

        public override void Initialize(Action<GameObject> finishedAction)
        {
            throw new MethodAccessException("AttackEffects don't support initialization without DamageInfo");
        }
    }
}
