using System;
using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using Nutmeg.Runtime.Utility.MouseController;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Nutmeg.Runtime.Gameplay.Weapons
{
    public abstract class WeaponDamageComponent : WeaponComponent
    {
        public Action<DamageableModule[]> onHit;

        protected Vector3 Origin
        {
            get
            {
                root.originComponent.Get(out object t);
                return ((Transform) t).position;
            }
        }

        protected Vector3 TargetPosition
        {
            get
            {
                Vector3 targetPosition = GetTargetPosition(~0);
                targetPosition.y += 1f;
                return targetPosition;
            }
        }

        protected Vector3 OffsetDirection => CalcRandomTargetOffsetByAccuracy((TargetPosition - Origin).normalized);

        protected override void Start()
        {
            base.Start();
            root.hitComponent = this;
        }
        
        protected Vector3 GetTargetPosition(LayerMask mask)
        {
            return MouseController.ShootRayFromCameraToMouse(mask)?.point ??
                MouseController.GetLastMouseLookTargetPoint();
        }
        
        protected Vector3 CalcRandomTargetOffsetByAccuracy(Vector3 origin)
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