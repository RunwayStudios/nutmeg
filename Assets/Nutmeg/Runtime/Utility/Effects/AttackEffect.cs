using System;
using UnityEngine;

namespace Nutmeg.Runtime.Utility.Effects
{
    public abstract class AttackEffect : MonoBehaviour
    {
        protected Vector3 origin;
        protected Vector3 target;

        protected Action<GameObject> FinishedAction;


        protected virtual void Finished()
        {
            gameObject.SetActive(false);
            FinishedAction(gameObject);
        }

        public virtual void Initialize(Vector3 origin, Vector3 target, Action<GameObject> FinishedAction)
        {
            this.origin = origin;
            this.target = target;
            this.FinishedAction = FinishedAction;
            gameObject.SetActive(true);
        }
    }
}
