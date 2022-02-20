using Gameplay.Items.Weapons;
using UnityEngine;
using UnityTemplateProjects.Gameplay;

namespace Gameplay.Items
{
    public class HitScanWeapon : Weapon
    {
        [SerializeField] private GameObject muzzleFlareFX;
        [SerializeField] private GameObject bulletTrailFX;
        [SerializeField] private Transform barrelEnd;

        [Header("Attachment positions")] [SerializeField]
        private Transform barrelPosition;

        [SerializeField] private Transform bodyPosition;
        [SerializeField] private Transform handlePosition;
        [SerializeField] private Transform magazinePosition;
        [SerializeField] private Transform muzzlePosition;
        [SerializeField] private Transform opticPosition;
        [SerializeField] private Transform stockPosition;
        [SerializeField] private Transform underBarrelPosition;

        [Header("Attachment gameObjects")] [SerializeField]
        private GameObject currentMuzzleAttachmentGameObject;

        [SerializeField] private GameObject currentOpticAttachmentGameObject;
        [SerializeField] private GameObject currentUnderBarrelAttachmentGameObject;

        protected override bool Attack()
        {
            if (!base.Attack()) return false;

            if (currentAmmunitionAmount <= 0)
            {
                ReloadWeapon();
                return false;
            }

            Instantiate(muzzleFlareFX, barrelEnd);

            currentAmmunitionAmount -= 1;

            if (HitScan(out RaycastHit hit))
                hit.transform.GetComponentInParent<DamageReceiver>().ReceiveDamage(Stats[StatType.Damage]);

            return true;
        }

        protected void SpawnBulletTrail(Vector3 target)
        {
            Instantiate(bulletTrailFX, barrelEnd.position, barrelEnd.rotation).GetComponent<Bullet>()
                .Initialize(target);
        }

        protected bool HitScan(out RaycastHit raycastHit)
        {
            raycastHit = new RaycastHit();

            Vector3 targetPosition =
                MouseController.ShootRayFromCameraToMouse(~0)?.point ??
                MouseController.GetLastMouseLookTargetPoint();

            Vector3 normalizedDirection = new Vector3(
                (targetPosition - transform.position).normalized.x, 0f,
                (targetPosition - transform.position).normalized.z);

            Vector3 offsetTargetDirection = CalcRandomTargetOffsetByAccuracy(
                normalizedDirection);

            Debug.DrawRay(transform.position,
                offsetTargetDirection
                * Stats[StatType.Range], Color.red,
                1f / (Stats[StatType.FireRate] / 60f));


            if (Physics.Raycast(transform.position, offsetTargetDirection, out RaycastHit hit, Stats[StatType.Range]))
            {
                SpawnBulletTrail(hit.point);
                if (hit.transform.CompareTag("DamageReceiver"))
                {
                    raycastHit = hit;
                    return true;
                }
            }
            else
            {
                SpawnBulletTrail(transform.position + offsetTargetDirection * Stats[StatType.Range]);
            }

            return false;
        }

        protected Vector3 CalcRandomTargetOffsetByAccuracy(Vector3 origin)
        {
            float r = .4f * Mathf.Sqrt(Random.Range(0f,
                1f - 2 * Stats[StatType.Accuracy] + Stats[StatType.Accuracy] * Stats[StatType.Accuracy]));
            float theta = Random.Range(0f, 1f) * 2f * Mathf.PI;

            origin.x += r * Mathf.Cos(theta);
            origin.z += r * Mathf.Sin(theta);

            return origin;
        }
    }
}