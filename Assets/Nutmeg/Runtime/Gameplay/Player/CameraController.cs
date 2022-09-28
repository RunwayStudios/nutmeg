using System;
using Cinemachine;
using Nutmeg.Runtime.Utility.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Nutmeg.Runtime.Gameplay.Player
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float scrollSpeed;
        [SerializeField] private Vector3 dollyOffset;

        private CinemachineVirtualCamera virtualCamera;
        private CinemachineTrackedDolly dolly;
        private InputActions input;

        private void Start()
        {
            input = InputManager.input;
            
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
            virtualCamera.m_LookAt = PlayerController.c_player.transform;
            virtualCamera.m_Follow = PlayerController.c_player.transform;
            dolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        }

        private void Update()
        {
        }

        private void OnEnable() => input.Player.Scroll.performed += OnScrollActionPerformed;

        private void UpdateDollyPosition() => dolly.m_Path.transform.position =
            new Vector3(virtualCamera.m_LookAt.position.x + dollyOffset.x, dollyOffset.y,
                virtualCamera.m_LookAt.position.z + dollyOffset.z);

        private void UpdateDollyPathPosition(int c) =>
            dolly.m_PathPosition = Mathf.Clamp01(dolly.m_PathPosition + c * scrollSpeed);

        private void OnScrollActionPerformed(InputAction.CallbackContext context) =>
            UpdateDollyPathPosition(context.ReadValue<Vector2>().y < 0 ? -1 : 1);
    }
}