using System;
using UnityEngine;

namespace Nutmeg.Runtime.Utility.Effects
{
    public abstract class AttackEffect : MonoBehaviour
    {
        protected Vector3 origin;
        protected Vector3 target;

        protected AttackEffectSpawner spawner;


        protected virtual void Finished()
        {
            gameObject.SetActive(false);
            spawner.Finished(gameObject);
        }

        public virtual void Initialize(Vector3 origin, Vector3 target, AttackEffectSpawner spawner)
        {
            this.origin = origin;
            this.target = target;
            this.spawner = spawner;
            gameObject.SetActive(true);
        }
    }
}
