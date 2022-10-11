using System;
using Cinemachine;
using Nutmeg.External.UDictionary;
using Nutmeg.Runtime.Gameplay.MainMenu;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Nutmeg.Runtime.Gameplay.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        public UDictionary<MenuTabTag, MenuTab> menuTabs;

        private MenuTabTag currentMenuTabTag;

        public static MainMenuManager Main { get; private set; }
        
        private void Start()
        {
            Main = this;
        }

        public void ChangeMenuTab(MenuTabTag tag)
        {
            UpdateCurrentMenuTabState(false);
            currentMenuTabTag = tag;
            UpdateCurrentMenuTabState(true);
        }

        private void UpdateCurrentMenuTabState(bool active)
        {
            MenuTab tab = menuTabs[currentMenuTabTag];

            if (currentMenuTabTag != MenuTabTag.Default)
                tab.gameObject.SetActive(active);
            tab.vCamera.enabled = active;
        }
    }

    [Serializable]
    public class MenuTab
    {
        public GameObject gameObject
        {
            get
            {
                if (gameObject.activeInHierarchy)
                    gameObject = Object.Instantiate(gameObject, worldTransform);

                return gameObject;
            }
            private set => gameObject = value;
        }

        public Transform worldTransform;
        public CinemachineVirtualCamera vCamera;

        private bool instantiated = false;
    }

    [Serializable]
    public enum MenuTabTag
    {
        Default,
        Play,
        Settings,
    }
}