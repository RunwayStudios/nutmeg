using UnityEngine;

namespace Gameplay.Items
{
    public class HitScanWeapon : Weapon
    {
        [SerializeField] private GameObject muzzleFlareFX;
        [SerializeField] private GameObject bulletTrailFX;
        [SerializeField] private Transform barrelEnd;

        private void Start()
        {
            currentAmmunitionAmount = stats.magazineSize;
        }

        protected override bool Fire()
        {
            if (!base.Fire()) return false;

            if (currentAmmunitionAmount <= 0)
            {
                ReloadWeapon();
                return false;
            }

            Instantiate(muzzleFlareFX, barrelEnd);

            currentAmmunitionAmount -= 1;

            if (HitScan(out RaycastHit hit))
                hit.transform.GetComponentInParent<DamageReceiver>().ReceiveDamage(stats.damage);

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
                * stats.range, Color.red,
                1f / (stats.fireRate / 60f));


            if (Physics.Raycast(transform.position,
                    offsetTargetDirection, out RaycastHit hit, stats.range
                ))
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
                SpawnBulletTrail(transform.position + offsetTargetDirection * stats.range);
            }

            return false;
        }

        protected Vector3 CalcRandomTargetOffsetByAccuracy(Vector3 origin)
        {
            float r = .4f * Mathf.Sqrt(Random.Range(0f, 1f - 2 * stats.accuracy + stats.accuracy * stats.accuracy));
            float theta = Random.Range(0f, 1f) * 2f * Mathf.PI;

            origin.x += r * Mathf.Cos(theta);
            origin.z += r * Mathf.Sin(theta);

            return origin;
        }
    }
}