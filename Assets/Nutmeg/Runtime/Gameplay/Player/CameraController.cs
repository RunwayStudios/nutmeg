using System;
using Cinemachine;
using Nutmeg.Runtime.Utility.InputSystem;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Nutmeg.Runtime.Gameplay.Player
{
    public class CameraController : NetworkBehaviour
    {
        [SerializeField] private float scrollSpeed;
        [SerializeField] private Vector3 dollyOffset;
        [SerializeField] private NetworkObject playerNetworkObject;

        private CinemachineVirtualCamera virtualCamera;
        private CinemachineTrackedDolly dolly;
        private InputActions input;

        private void Start()
        {
            if (playerNetworkObject.IsLocalPlayer)
            {
                virtualCamera = GetComponent<CinemachineVirtualCamera>();
                dolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
                
                virtualCamera.enabled = true;
                virtualCamera.m_LookAt = NetworkPlayerController.Main.transform;
                virtualCamera.m_Follow = NetworkPlayerController.Main.transform;
            
                input = InputManager.Input;
                input.Player.Scroll.performed += OnScrollActionPerformed; 
            }
        }

        private void UpdateDollyPosition() => dolly.m_Path.transform.position =
            new Vector3(virtualCamera.m_LookAt.position.x + dollyOffset.x, dollyOffset.y,
                virtualCamera.m_LookAt.position.z + dollyOffset.z);

        private void UpdateDollyPathPosition(int c) =>
            dolly.m_PathPosition = Mathf.Clamp01(dolly.m_PathPosition + c * scrollSpeed);

        private void OnScrollActionPerformed(InputAction.CallbackContext context) =>
            UpdateDollyPathPosition(context.ReadValue<Vector2>().y < 0 ? -1 : 1);
    }
}