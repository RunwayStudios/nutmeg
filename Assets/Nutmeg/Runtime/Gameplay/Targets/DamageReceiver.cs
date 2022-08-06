using Nutmeg.Runtime.Utility.WorldSpaceUI;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Targets
{
    public class DamageReceiver : MonoBehaviour
    {
        [SerializeField] private GameObject hitNumberObject;
        [SerializeField] private Transform hitNumberSpawnPosition;

        private float damageNumber;
        private WSUIElement worldSpaceUIElement;

        public void ReceiveDamage(float damage)
        {
            SpawnHitNumbers(damage);
        }

        private void SpawnHitNumbers(float value)
        {
            if (worldSpaceUIElement == null)
            {
                worldSpaceUIElement =
                    WorldSpaceUI.Create(hitNumberObject, hitNumberSpawnPosition, .3f).DestroyOnComplete();
                worldSpaceUIElement.onDestroy += () => worldSpaceUIElement = null;
            }
            else
            {
                worldSpaceUIElement.Reset();
            }

            var hn = worldSpaceUIElement.GameObject.GetComponent<HitNumber>();
            hn.SetNumberAndPosition(hn.Number + value, hitNumberSpawnPosition.position);
        }
    }
}