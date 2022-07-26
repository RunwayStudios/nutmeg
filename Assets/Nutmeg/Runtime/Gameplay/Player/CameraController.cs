using System;
using Cinemachine;
using Nutmeg.Runtime.Utility.InputSystem;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Player
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float scrollSpeed;
        [SerializeField] private InputEventChannel input;
        [SerializeField] private Vector3 dollyOffset;

        private CinemachineVirtualCamera virtualCamera;
        private CinemachineTrackedDolly dolly;
        
        private void Start()
        {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
            virtualCamera.m_LookAt = PlayerController.c_player.transform;
            virtualCamera.m_Follow = PlayerController.c_player.transform;
            dolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
            
        }

        private void Update()
        {
            UpdateDollyPosition();
        }

        private void OnEnable() => input.onScrollActionPerformed += OnScrollActionPerformed;

        private void UpdateDollyPosition() => dolly.m_Path.transform.position = new Vector3(virtualCamera.m_LookAt.position.x + dollyOffset.x, dollyOffset.y, virtualCamera.m_LookAt.position.z + dollyOffset.z);

        private void UpdateDollyPathPosition(int c) => dolly.m_PathPosition = Mathf.Clamp01(dolly.m_PathPosition + c * scrollSpeed);

        private void OnScrollActionPerformed(Vector2 value) => UpdateDollyPathPosition(value.y < 0 ? -1 : 1);
    }
}