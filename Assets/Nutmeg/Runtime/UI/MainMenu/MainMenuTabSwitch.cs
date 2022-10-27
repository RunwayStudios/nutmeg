using Nutmeg.Runtime.Gameplay.MainMenu;
using Nutmeg.Runtime.Gameplay.MainMenu.CharacterSelection;
using UnityEngine;

namespace Nutmeg.Runtime.UI.MainMenu
{
    public class MainMenuTabSwitch : MonoBehaviour
    {
        [SerializeField] private MenuTabTag tag;

        public void SwitchMenuTab() => MainMenuManager.Main?.ChangeMenuTab(tag);
    }
}