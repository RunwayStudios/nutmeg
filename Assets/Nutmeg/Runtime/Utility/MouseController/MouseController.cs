using UnityEngine;
using UnityEngine.InputSystem;

namespace Nutmeg.Runtime.Utility.MouseController
{
    public class MouseController : MonoBehaviour
    {
        public static Camera camera;
        private static RaycastHit? lastLookTarget;

        private void Start() => camera = GetComponent<Camera>();

        public static GameObject GetLastMouseLookTargetGameObject() => lastLookTarget?.transform.gameObject;

        public static Vector3 GetLastMouseLookTargetPoint() => lastLookTarget?.point ?? Vector3.zero;

        public static void UpdateMouseLookTarget() => lastLookTarget =
            ShootRayFromCameraToMouse(LayerMask.GetMask("MouseTargetable")) ?? lastLookTarget;

        public static RaycastHit? ShootRayFromCameraToMouse(LayerMask mask)
        {
            Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, mask))
            {
                Debug.DrawRay(camera.transform.position,
                    camera.transform.position + (hit.point - camera.transform.position) * 100f);
                return hit;
            }

            return null;
        }
    }
}