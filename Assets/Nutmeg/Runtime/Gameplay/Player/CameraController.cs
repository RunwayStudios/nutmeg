using Cinemachine;
using Nutmeg.Runtime.Utility.InputSystem;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Player
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float scrollSpeed;
        [SerializeField] private InputEventChannel input;

        private CinemachineVirtualCamera virtualCamera;
    
        void Start() => virtualCamera = GetComponent<CinemachineVirtualCamera>();

        private void OnEnable() => input.onScrollActionPerformed += OnScrollActionPerformed;

        private void UpdateDollyPosition(int c)
        {
            CinemachineTrackedDolly dolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
            dolly.m_PathPosition = Mathf.Clamp01(dolly.m_PathPosition + c * scrollSpeed);
        }

        private void OnScrollActionPerformed(Vector2 value) => UpdateDollyPosition(value.y < 0 ? -1 : 1);
    }
}