using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Targets
{
    public class DamageReceiver : MonoBehaviour
    {
        [SerializeField] private GameObject hitNumberObject;
        [SerializeField] private Transform hitNumberSpawnPosition;

        private float damageNumber;

        public void ReceiveDamage(float damage)
        {
            SpawnHitNumbers(damage);
        }

        private void SpawnHitNumbers(float value)
        {
            
        }
    }
}