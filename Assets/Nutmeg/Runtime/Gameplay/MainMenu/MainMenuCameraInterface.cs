using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.MainMenu
{
    public class MainMenuCameraInterface : MonoBehaviour
    {
        [SerializeField] private MenuTabTag tag;

        public void SwitchCamera() => MainMenuManager.Main?.ChangeMenuTab(tag);
    }
}