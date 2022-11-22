using System;
using UnityEngine;

namespace Nutmeg.Runtime.Utility.Effects.EffectTypes
{
    public class ParticleEffect : Effect
    {
        [SerializeField] private ParticleSystem particles;
        
        
        public override void Initialize(Action<GameObject> finishedAction)
        {
            this.finishedAction = finishedAction;
            
            gameObject.SetActive(true);
            particles.Play();
        }

        private void Update()
        {
            if (particles.isStopped)
                Finished();
        }
    }
}
