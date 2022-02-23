using Nutmeg.Runtime.Utility.MouseController;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Weapons.Throwable
{
    public abstract class Throwable : Weapon
    {
        [SerializeField] private float throwOffset;
        [SerializeField] private float throwSpeed;

        private Vector3 origin;
        private Vector3 target;
        private float elapsedTime;

        private void Update()
        {
            UpdateArcPosition();
        }

        protected abstract void OnImpact();

        private void UpdateArcPosition()
        {
            Vector3 centerPivot = (origin + target) * 0.5f;

            centerPivot -= new Vector3(0, throwOffset, 0);

            Vector3 startRelativeCenter = origin - centerPivot;
            Vector3 endRelativeCenter = target - centerPivot;

            transform.position = Vector3.Slerp(startRelativeCenter, endRelativeCenter,
                elapsedTime += Time.deltaTime / Vector3.Distance(origin, target) * throwSpeed);
            transform.position += centerPivot;
        }
        
        private void OnCollisionEnter(Collision c)
        {
            OnImpact();
        }

        public void Start()
        {
            target = MouseController.ShootRayFromCameraToMouse(LayerMask.GetMask("MouseTargetable"))?.point ??
                     MouseController.GetLastMouseLookTargetPoint();

            origin = transform.position;
        }
    }
}