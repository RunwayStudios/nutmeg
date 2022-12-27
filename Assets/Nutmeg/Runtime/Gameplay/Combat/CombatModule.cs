using Unity.Netcode;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Combat
{
    public abstract class CombatModule : NetworkBehaviour
    {
        private CombatEntity entity;

        public virtual void InitializeModule(CombatEntity entity)
        {
            this.entity = entity;
        }

        public virtual void UpdateModule()
        {
            
        }

        public CombatEntity Entity => entity;
    }
}
