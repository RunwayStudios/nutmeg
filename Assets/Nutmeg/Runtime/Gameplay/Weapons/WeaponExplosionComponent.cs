using System.Collections.Generic;
using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Weapons
{
    public class WeaponExplosionComponent : WeaponDamageComponent
    {
        public override bool Get(out object data)
        {
            if (Explode(out var h))
            {
                data = h;
                return true;
            }

            data = default;
            return false;
        }

        private bool Explode(out DamageableModule[] hits)
        {
            List<DamageableModule> modules = new();

            Collider[] hitColliders = Physics.OverlapSphere(root.originComponent.Get<Vector3>(), root.stats.range);
            foreach (var h in hitColliders)
            {
                if (h.gameObject.TryGetComponent(typeof(DamageableModule), out var m))
                    modules.Add((DamageableModule) m);
            }

            hits = modules.ToArray();

            return hits.Length >= 0;
        }
    }
}