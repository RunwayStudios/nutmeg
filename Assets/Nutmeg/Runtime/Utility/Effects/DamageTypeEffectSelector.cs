using System;
using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using UnityEngine;

namespace Nutmeg.Runtime.Utility.Effects
{
    public class DamageTypeEffectSelector : MonoBehaviour
    {
        [SerializeField] private EffectSpawner[] defaultEffectSpawner;
        [SerializeField] private EffectSpawner[] fireEffectSpawner;
        [SerializeField] private EffectSpawner[] explosionEffectSpawner;
        [SerializeField] private EffectSpawner[] waterEffectSpawner;
        [Space]
        
        [SerializeField] private EffectSpawner[] defaultDeathEffectSpawner;
        [SerializeField] private EffectSpawner[] fireDeathEffectSpawner;
        [SerializeField] private EffectSpawner[] explosionDeathEffectSpawner;
        [SerializeField] private EffectSpawner[] waterDeathEffectSpawner;

        public void OnDamage(DamageInfo info)
        {
            switch (info.Type)
            {
                case DamageType.Default:
                    for (int i = 0; i < defaultEffectSpawner.Length; i++)
                        defaultEffectSpawner[i].SpawnEffect(info);
                    break;
                case DamageType.Fire:
                    for (int i = 0; i < fireEffectSpawner.Length; i++)
                        fireEffectSpawner[i].SpawnEffect(info);
                    break;
                case DamageType.Explosion:
                    for (int i = 0; i < explosionEffectSpawner.Length; i++)
                        explosionEffectSpawner[i].SpawnEffect(info);
                    break;
                case DamageType.Water:
                    for (int i = 0; i < waterEffectSpawner.Length; i++)
                        waterEffectSpawner[i].SpawnEffect(info);
                    break;
            }
        }
    }
}