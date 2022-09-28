using System;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Weapons
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float lifeTime = .1f;
        private Vector3 origin;
        private Vector3 target;

        private float elapsedTime;
        private bool locked = true;

        private Action<GameObject> releaseAction;

        private void Update()
        {
            if (locked)
                return;
            transform.position = Vector3.Lerp(origin, target,
                elapsedTime += Time.deltaTime / (Vector3.Distance(origin, target) / lifeTime));

            if (transform.position == target)
                ReleaseBullet();
        }

        public void SetReleaseAction(Action<GameObject> action) => releaseAction = action;

        private void ReleaseBullet()
        {
            locked = true;
            elapsedTime = 0f;
            releaseAction.Invoke(gameObject);
        }

        public void Initialize(Vector3 origin, Vector3 target)
        {
            this.origin = origin;
            this.target = target;
            locked = false;
        }
    }
}