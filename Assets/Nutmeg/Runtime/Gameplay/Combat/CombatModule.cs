using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Combat
{
    public abstract class CombatModule : MonoBehaviour
    {
        private CombatEntity entity;

        public virtual void InitializeModule(CombatEntity entity)
        {
            this.entity = entity;
        }
    
        public virtual void UpdateModule()
        {
        
        }


        protected CombatEntity Entity => entity;
    }
}
