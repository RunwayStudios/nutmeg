using Gameplay.Level.LevelGenerator;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Combat.CombatModules
{
    public class DamageableModule : CombatModule
    {
        [SerializeField] private float health = 100f;


        public virtual void Damage(float value, DamageType type)
        {
            health -= value;

            if (health <= 0f)
            {
                DestroyImmediate(gameObject);
                
                // todo call event instead
                if (Entity.Group == CombatGroup.Building)
                    LevelGenerator.Main.UpdateNavMesh();
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
}