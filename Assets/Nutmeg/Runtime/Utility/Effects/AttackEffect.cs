using System;
using UnityEngine;

namespace Nutmeg.Runtime.Utility.Effects
{
    public abstract class AttackEffect : MonoBehaviour
    {
        protected Vector3 origin;
        protected Vector3 target;

        protected Action<GameObject> finishedAction;


        protected virtual void Finished()
        {
            gameObject.SetActive(false);
            finishedAction(gameObject);
        }

        public virtual void Initialize(Vector3 origin, Vector3 target, Action<GameObject> finishedAction)
        {
            this.origin = origin;
            this.target = target;
            this.finishedAction = finishedAction;
            gameObject.SetActive(true);
        }
    }
}
