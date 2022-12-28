using System;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.PlayerWeapons
{
    public class TriggerHitDetector : MonoBehaviour
    {
        [SerializeField] private Vector3 velocity;
        [SerializeField] private float lifeTime;

        private Vector3 trueVelocity;
        private float finishTime;
        private Action<GameObject> finishedAction;


        private void Update()
        {
            if (finishTime < Time.time)
            {
                Finished();
                return;
            }

            transform.position += velocity * Time.deltaTime;
        }


        public void Initialize(Transform source, Action<GameObject> finishedActionIn)
        {
            transform.position = source.position;
            trueVelocity = source.rotation * velocity;
            finishTime = Time.time + lifeTime;
            finishedAction = finishedActionIn;
            gameObject.SetActive(true);
        }
        
        private void Finished()
        {
            gameObject.SetActive(false);
            finishedAction(gameObject);
        }
    }
}