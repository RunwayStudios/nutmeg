using JetBrains.Annotations;
using Nutmeg.Runtime.Utility.MouseController;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Weapons
{
    public class HitScanComponent : WeaponComponent
    {
        [CanBeNull]
        public override T Get<T>()
        {
            return default;
        }

        private bool HitScan(out RaycastHit hit)
        {
            hit = new RaycastHit();

            Vector3 targetPosition =
                MouseController.ShootRayFromCameraToMouse(~0)?.point ??
                MouseController.GetLastMouseLookTargetPoint();

            Vector3 normalizedDirection = new Vector3(
                (targetPosition - transform.position).normalized.x, 0f,
                (targetPosition - transform.position).normalized.z);

            Debug.DrawRay(transform.position, normalizedDirection * 100f, Color.red,
                1f / (1000 / 60f));

            if (Physics.Raycast(transform.position, normalizedDirection, out RaycastHit h, 100f))
            {
                if (h.transform.CompareTag("DamageReceiver"))
                {
                    hit = h;
                    return true;
                }
            }

            return false;
        }
    }
}