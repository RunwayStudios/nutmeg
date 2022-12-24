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
        private static RaycastHit lastHit;

        private static Transform camTransform;
        private static Vector3 prevCamPos = Vector3.zero;
        private static Vector2 prevCursorPos = Vector2.zero;

        private static bool updatedThisFrame;

        private void Start()
        {
            camera = GetComponent<Camera>();
            camTransform = camera.transform;
            defaultLayerMask = LayerMask.GetMask("Terrain");
        }

        private void Update()
        {
            updatedThisFrame = false;
        }

        public static GameObject GetLastMouseLookTargetGameObject()
        {
            UpdateMouseLookTarget();
            return lastHit.transform.gameObject;
        }

        public static Vector3 GetLastMouseLookTargetPoint()
        {
            UpdateMouseLookTarget();
            return lastHit.point;
        }

        private static void UpdateMouseLookTarget()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            if (updatedThisFrame || (prevCamPos == camTransform.position && prevCursorPos == mousePos))
                return;

            updatedThisFrame = true;

            prevCamPos = camera.transform.position;
            prevCursorPos = mousePos;
            lastHit = ShootRayFromCameraToMouse_Internal(defaultLayerMask) ?? lastHit;
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