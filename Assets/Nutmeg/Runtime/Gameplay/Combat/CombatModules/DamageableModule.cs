using Gameplay.Level.LevelGenerator;
using UnityEngine;
using UnityEngine.Events;

namespace Nutmeg.Runtime.Gameplay.Combat.CombatModules
{
    public class DamageableModule : CombatModule
    {
        [SerializeField] private float health = 100f;
        [SerializeField] private UnityEvent OnReceiveDamage;
        [SerializeField] private UnityEvent OnDeath;


        public virtual void Damage(float value, DamageType type)
        {
            health -= value;
            
            OnReceiveDamage.Invoke();

            if (health <= 0f)
            {
                Entity.OnDeath();
                OnDeath.Invoke();
                
                // todo call event instead
                if (Entity.Group == CombatGroup.Building)
                    LevelGenerator.Main.UpdateNavMesh();
            }
        }
    }
    
    public enum DamageType
    {
        Default,
        Fire,
        Explosion,
        Water
    }
}