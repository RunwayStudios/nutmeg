using System;
using JetBrains.Annotations;
using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using Nutmeg.Runtime.Gameplay.Weapons.Editor;
using Nutmeg.Runtime.Utility.MouseController;
using UnityEngine;

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

            Vector3 targetPosition =
                MouseController.ShootRayFromCameraToMouse(~0)?.point ??
                MouseController.GetLastMouseLookTargetPoint();

            Vector3 normalizedDirection = new Vector3(
                (targetPosition - transform.position).normalized.x, 0f,
                (targetPosition - transform.position).normalized.z);

            
            Debug.DrawRay(transform.position, normalizedDirection * root.stats.range, Color.red,
                root.stats.fireRate / (1000f / 60f));

            if (Physics.Raycast(transform.position, normalizedDirection, out RaycastHit h, root.stats.range))
            {
                if (h.transform.TryGetComponent(out DamageableModule m))
                {
                    hit = m;
                    return true;
                }
            }

            return false;
        }
    }
}