using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.PlayerWeapons
{
    public class TriggerHitDetector : MonoBehaviour
    {
        [SerializeField] private Vector3 velocity;
        [SerializeField] private float lifeTime;
        [Space] [SerializeField] Vector3 colliderHalfExtends;

        private Vector3 trueVelocity;
        private float finishTime;
        private Action<GameObject> finishedAction;
        private Collider[] colliderBuffer = new Collider[5];


        private void Update()
        {
            if (finishTime < Time.time)
            {
                Finished();
                return;
            }

            transform.position += trueVelocity * Time.deltaTime;
        }


        public void AddNewCollisions(List<Transform> collisions)
        {
            int count = Physics.OverlapBoxNonAlloc(transform.position, colliderHalfExtends, colliderBuffer);

            for (int i = 0; i < count; i++)
                if (!collisions.Contains(colliderBuffer[i].transform))
                    collisions.Add(colliderBuffer[i].transform);
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


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            // Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.DrawWireCube(transform.position, colliderHalfExtends * 2);
        }
#endif
    }
}