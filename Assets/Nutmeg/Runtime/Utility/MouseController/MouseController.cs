using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Nutmeg.Runtime.Utility.MouseController
{
    public class MouseController : MonoBehaviour
    {
        public static Camera camera;
        public static LayerMask defaultLayerMask;
        private static RaycastHit lastLookTarget;

        private Transform camTransform;
        private Vector3 prevCamPos = Vector3.zero;
        private Vector2 prevCursorPos = Vector2.zero;

        private void Start()
        {
            camera = GetComponent<Camera>();
            camTransform = camera.transform;
            defaultLayerMask = LayerMask.GetMask("Terrain");
        }

        private void Update()
        {
            if (prevCamPos != camTransform.position || prevCursorPos != Mouse.current.position.ReadValue())
                UpdateMouseLookTarget();
        }

        public static GameObject GetLastMouseLookTargetGameObject() => lastLookTarget.transform.gameObject;

        public static Vector3 GetLastMouseLookTargetPoint() => lastLookTarget.point;

        private void UpdateMouseLookTarget()
        {
            prevCamPos = camera.transform.position;
            prevCursorPos = Mouse.current.position.ReadValue();
            lastLookTarget = ShootRayFromCameraToMouse_Internal(defaultLayerMask) ?? lastLookTarget;
        }

        private static RaycastHit? ShootRayFromCameraToMouse_Internal(LayerMask mask)
        {
            Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, mask))
            {
                // Debug.DrawRay(camera.transform.position,
                //     camera.transform.position + (hit.point - camera.transform.position) * 100f);
                return hit;
            }

            return null;
        }

        public static RaycastHit? ShootRayFromCameraToMouse(LayerMask mask)
        {
#if UNITY_EDITOR
            if (mask == defaultLayerMask)
                throw new ArgumentException("use GetLastMouseLookTarget() when using default layer mask");
#endif
            
            return ShootRayFromCameraToMouse_Internal(mask);
        }
    }
}