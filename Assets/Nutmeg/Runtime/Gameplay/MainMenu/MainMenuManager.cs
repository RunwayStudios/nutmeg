using System;
using System.Collections;
using Cinemachine;
using Nutmeg.External.UDictionary;
using Nutmeg.Runtime.Core.GameManager;
using Nutmeg.Runtime.Core.Networking.Steam;
using Nutmeg.Runtime.Gameplay.MainMenu.Pedestal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Nutmeg.Runtime.Gameplay.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private PlayerCharacter.PlayerCharacter defaultCharacter;
        [SerializeField] private CinemachineBrain mainCameraBrain;
        [SerializeField] private UDictionary<MenuTabTag, MenuTab> menuTabs;

        public static Action<MenuTabTag> onMenuTabEnableStarted;
        public static Action<MenuTabTag> onMenuTabEnableFinished;
        public static Action<MenuTabTag> onMenuTabDisableStarted;
        public static Action<MenuTabTag> onMenuTabDisableFinished;
        public static MainMenuManager Main { get; private set; }

        private MenuTabTag currentMenuTabTag;
        private void Start()
        {
            Main = this;

            MainMenuPedestalManager.Main.AddPedestal(SteamManager.Id, GameManager.selectedCharacter != null
                ? GameManager.selectedCharacter
                : defaultCharacter);
        }

        public void ChangeMenuTab(MenuTabTag tag)
        {
            if (tag == currentMenuTabTag || mainCameraBrain.IsBlending)
                return;

            StartCoroutine(EnableMenuTab(tag));
            StartCoroutine(DisableMenuTab(currentMenuTabTag));

            currentMenuTabTag = tag;
        }

        private IEnumerator EnableMenuTab(MenuTabTag tag)
        {
            MenuTab tab = menuTabs[tag];
            
            tab.vCamera.enabled = true;
            if (tag != MenuTabTag.Default)
                tab.GameObject.SetActive(true);
            yield return null;
            
            onMenuTabEnableStarted?.Invoke(tag);
            while (mainCameraBrain.IsBlending)
                yield return null;
            
            onMenuTabEnableFinished?.Invoke(tag);
        }

        private IEnumerator DisableMenuTab(MenuTabTag tag)
        {
            onMenuTabDisableStarted?.Invoke(tag);
            
            MenuTab tab = menuTabs[tag];
            tab.vCamera.enabled = false;

            if (tag == MenuTabTag.Default)
                yield break;

            yield return null;
            while (mainCameraBrain.IsBlending)
                yield return null;
            
            tab.GameObject.SetActive(false);
            onMenuTabDisableFinished?.Invoke(tag);
        }
    }

    [Serializable]
    public class MenuTab
    {
        [SerializeField] private GameObject gameObject;

        public GameObject GameObject
        {
            get
            {
                if (!gameObject.scene.IsValid())
                    gameObject = Object.Instantiate(gameObject, worldTransform);

                return gameObject;
            }
        }

        public Transform worldTransform;
        public CinemachineVirtualCamera vCamera;
    }

    [Serializable]
    public enum MenuTabTag
    {
        Default,
        Play,
        Settings,
        CharacterSelection
    }
    
    public enum MenuTabDirection
    {
        Forward,
        Left,
        Right
    }
}