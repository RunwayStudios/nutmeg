using System;
using JetBrains.Annotations;
using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using Nutmeg.Runtime.Gameplay.Weapons.Editor;
using Nutmeg.Runtime.Utility.MouseController;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Nutmeg.Runtime.Gameplay.Weapons
{
    [DisallowMultipleComponent]
    public class WeaponHitScanComponent : WeaponDamageComponent
    {
        public override bool Get(out object data)
        {
            bool b = HitScan(out DamageableModule r);
            data = r;
            return b;
        }

        protected virtual bool HitScan(out DamageableModule hit)
        {
            hit = default;
            root.originComponent.Get(out object t);
            Transform transform = (Transform) t;
            
            Vector3 targetPosition =
                MouseController.ShootRayFromCameraToMouse(~0)?.point ??
                MouseController.GetLastMouseLookTargetPoint();

            Vector3 normalizedDirection = new Vector3(
                (targetPosition - transform.position).normalized.x, 0f,
                (targetPosition - transform.position).normalized.z);
            
            Vector3 offsetDirection = CalcRandomTargetOffsetByAccuracy(
                normalizedDirection);

            
            Debug.DrawRay(transform.position, offsetDirection * root.stats.range, Color.red,
                1f / (root.stats.fireRate / 60f));

            if (Physics.Raycast(transform.position, offsetDirection, out RaycastHit h, root.stats.range))
            {
                if (h.transform.TryGetComponent(out DamageableModule m))
                {
                    hit = m;
                    return true;
                }
            }

            return false;
        }
        
        private Vector3 CalcRandomTargetOffsetByAccuracy(Vector3 origin)
        {
            float r = .4f * Mathf.Sqrt(Random.Range(0f,
                1f - 2 * root.stats.accuracy + root.stats.accuracy * root.stats.accuracy));
            float theta = Random.Range(0f, 1f) * 2f * Mathf.PI;

            origin.x += r * Mathf.Cos(theta);
            origin.z += r * Mathf.Sin(theta);

            return origin;
        }
    }
}