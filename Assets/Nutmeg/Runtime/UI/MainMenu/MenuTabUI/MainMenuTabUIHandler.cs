using Nutmeg.Runtime.Gameplay.MainMenu;
using UnityEngine;

namespace Nutmeg.Runtime.UI.MainMenu.MenuTabUI
{
    public abstract class MainMenuTabUIHandler : MonoBehaviour
    {
        [SerializeField] protected GameObject uiContainer;
        [SerializeField] private MenuTabTag menuTabTag;
        
        
        protected virtual void Start()
        {
            MainMenuManager.onMenuTabEnableFinished += OnMenuTabEnableFinished;
            MainMenuManager.onMenuTabDisableStarted += OnMenuTabDisableStarted;
        }
        
        protected virtual void OnDestroy()
        {
            MainMenuManager.onMenuTabEnableFinished -= OnMenuTabEnableFinished;
            MainMenuManager.onMenuTabDisableStarted -= OnMenuTabDisableStarted;
        }
        
        protected virtual void OnMenuTabEnableFinished(MenuTabTag tag)
        {
            if (tag == menuTabTag)
                uiContainer.SetActive(true);
        }

        protected virtual void OnMenuTabDisableStarted(MenuTabTag tag)
        {
            if (tag == menuTabTag)
                uiContainer.SetActive(false);
        }
    }
}