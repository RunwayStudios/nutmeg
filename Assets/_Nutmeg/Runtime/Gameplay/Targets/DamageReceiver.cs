using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Targets
{
    public class DamageReceiver : MonoBehaviour
    {
        [SerializeField] private GameObject hitNumberObject;
        [SerializeField] private Transform hitNumberSpawnPosition;

        private HitNumber hitNumber;

        private float damageNumber;

        public void ReceiveDamage(float damage)
        {
            SpawnHitNumbers(damage);
        }

        private void SpawnHitNumbers(float value)
        {
            if (hitNumber != null)
            {
                damageNumber += value;
                hitNumber.SetNumberAndPosition(damageNumber, hitNumberSpawnPosition.position);
                hitNumber.ResetLifeTime();
                return;
            }

            damageNumber = value;

            hitNumber = Instantiate(hitNumberObject, hitNumberSpawnPosition.position, Quaternion.identity)
                .GetComponent<HitNumber>();

            hitNumber.SetNumberAndPosition(value, hitNumberSpawnPosition.position);
        }
    }
}