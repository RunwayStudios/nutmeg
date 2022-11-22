using System;
using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using UnityEngine;

namespace Nutmeg.Runtime.Utility.Effects
{
    public abstract class Effect : MonoBehaviour
    {
        protected Action<GameObject> finishedAction;
        
        
        public virtual void Initialize(Action<GameObject> finishedAction)
        {
            this.finishedAction = finishedAction;
            gameObject.SetActive(true);
        }
        
        public virtual void Initialize(DamageInfo info, Action<GameObject> finishedAction)
        {
            this.finishedAction = finishedAction;
            gameObject.SetActive(true);
        }

        protected virtual void Finished()
        {
            gameObject.SetActive(false);
            finishedAction(gameObject);
        }
    }
}
