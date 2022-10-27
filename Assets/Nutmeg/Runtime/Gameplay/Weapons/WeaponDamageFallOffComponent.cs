using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Weapons
{
    public class WeaponDamageFallOffComponent : WeaponDamageOverrideComponent
    {
        [SerializeField] private AnimationCurve damageFallOff;
        [SerializeField] [Range(0f, 1f)] private float minActivation;
        [SerializeField] [Range(0f, 1f)] private float maxActivation;

        public override bool Get(out object data)
        {
            DoDamage();
            data = default;
            return true;
        }

        private void DoDamage()
        {
            foreach (var m in modules)
            {
                float distance = Vector3.Distance(root.originComponent.Get<Vector3>(), m.transform.position) / root.stats.range;

                if (distance < minActivation)
                {
                    m.Damage(root.stats.damage, root.stats.damageType);
                    continue;
                }

                if (distance > maxActivation)
                {
                    m.Damage(damageFallOff.Evaluate(damageFallOff.keys[0].value) * root.stats.damage, root.stats.damageType);
                    continue;
                }
                
                m.Damage(damageFallOff.Evaluate(distance) * root.stats.damage, root.stats.damageType);
            }
        }
    }
}