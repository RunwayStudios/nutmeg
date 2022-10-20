using System;
using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using UnityEngine;
using UnityEngine.Events;

namespace Nutmeg.Runtime.Gameplay.Weapons.Misc
{
    public class Projectile : MonoBehaviour, IWeaponPoolObject
    {
        [SerializeField] private float speed = 250f;
        private Vector3 origin;
        private Vector3 target;

        private float elapsedTime;
        private bool locked = true;

        private Action<GameObject> releaseAction;

        public UnityEvent onImpact;

        private float distance;

        private void Start()
        {
            distance = Vector3.Distance(origin, target);
        }

        private void Update()
        {
            //if (locked)
            //    return;
            transform.position = Vector3.Lerp(origin, target, elapsedTime += Time.deltaTime / distance / speed);

            transform.LookAt(target);

            if (transform.position == target)
                ReleaseBullet();
        }

        public void SetReleaseAction(Action<GameObject> action) => releaseAction = action;

        protected virtual void ReleaseBullet()
        {
            onImpact?.Invoke();
            releaseAction.Invoke(gameObject);

            locked = true;
            elapsedTime = 0f;
        }

        public void Initialize(Vector3 origin, Vector3 target)
        {
            this.origin = origin;
            this.target = target;
            transform.position = origin;
            gameObject.SetActive(true);
            locked = false;
        }
    }
}