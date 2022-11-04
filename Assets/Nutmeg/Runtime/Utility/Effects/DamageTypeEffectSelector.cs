using System;
using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using UnityEngine;

namespace Nutmeg.Runtime.Utility.Effects
{
    public class DamageTypeEffectSelector : MonoBehaviour
    {
        [SerializeField] private EffectSpawner defaultEffectSpawner;

        public void OnDamage(Vector3 pos, DamageType type)
        {
            switch (type)
            {
                case DamageType.Default:
                    defaultEffectSpawner.SpawnEffect();
                    break;
                case DamageType.Fire:
                    break;
                case DamageType.Explosion:
                    break;
                case DamageType.Water:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}