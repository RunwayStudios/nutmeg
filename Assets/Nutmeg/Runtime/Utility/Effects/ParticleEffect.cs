using System;
using UnityEngine;

namespace Nutmeg.Runtime.Utility.Effects
{
    public class ParticleEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particles;
        private Action<GameObject> finishedAction;
        
        public void Initialize(Action<GameObject> finishedAction)
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

        private void Finished()
        {
            gameObject.SetActive(false);
            finishedAction(gameObject);
        }
    }
}
