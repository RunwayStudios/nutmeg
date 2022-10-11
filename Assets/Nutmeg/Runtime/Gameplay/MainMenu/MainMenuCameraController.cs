using System;
using Cinemachine;
using JetBrains.Annotations;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.MainMenu
{
    public class MainMenuCameraController : MonoBehaviour
    {
        [SerializeField] private CameraConfiguration[] cameras;
        [SerializeField] private CameraDirection defaultDirection;

        private CameraConfiguration previousConfiguration;

        [CanBeNull] public static MainMenuCameraController Main { get; private set; }

        private void Start()
        {
            Main = this;

            DisableAllVirtualCameras();
            SwitchToDefaultCamera();
        }

        private void DisableAllVirtualCameras()
        {
            foreach (var c in cameras)
                c.virtualCamera.enabled = false;
        }

        private CameraConfiguration GetCameraConfigurationWithDirection(CameraDirection direction)
        {
            foreach (var c in cameras)
            {
                if (c.direction == direction)
                    return c;
            }

            throw new Exception("There is no camera configuration with direction " + direction);
        }

        public void ChangeCameras(CameraDirection direction)
        {
            CameraConfiguration newConfiguration = GetCameraConfigurationWithDirection(direction);

            if(previousConfiguration != null)
                previousConfiguration.virtualCamera.enabled = false;
            newConfiguration.virtualCamera.enabled = true;
            previousConfiguration = newConfiguration;
        }

        public void SwitchToDefaultCamera() => ChangeCameras(defaultDirection);
    }

    [Serializable]
    internal class CameraConfiguration
    {
        public CameraDirection direction;
        public CinemachineVirtualCamera virtualCamera;
    }

    public enum CameraDirection
    {
        Forward,
        Left,
        Right
    }
}