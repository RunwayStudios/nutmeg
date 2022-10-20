using System;
using Nutmeg.Runtime.Gameplay.MainMenu;
using UnityEngine;

namespace Nutmeg.Runtime.UI.MainMenu
{
    public class MainMenuUIHandler : MonoBehaviour
    {
        [SerializeField] private GameObject playerCharacterSelectionUI;

        private void Start()
        {
            MainMenuManager.onMenuTabEnableStarted += OnMenuTabEnableStarted;
            MainMenuManager.onMenuTabEnableFinished += OnMenuTabEnableFinished;
            MainMenuManager.onMenuTabDisableStarted += OnMenuTabDisableStarted;
            MainMenuManager.onMenuTabDisableFinished += OnMenuTabDisableFinished;
        }

        private void OnDestroy()
        {
            MainMenuManager.onMenuTabEnableStarted -= OnMenuTabEnableStarted;
            MainMenuManager.onMenuTabEnableFinished -= OnMenuTabEnableFinished;
            MainMenuManager.onMenuTabDisableStarted -= OnMenuTabDisableStarted;
            MainMenuManager.onMenuTabDisableFinished -= OnMenuTabDisableFinished;
        }

        private void OnMenuTabEnableFinished(MenuTabTag tag)
        {
        }

        private void OnMenuTabEnableStarted(MenuTabTag tag)
        {
        }

        private void OnMenuTabDisableStarted(MenuTabTag tag)
        {
        }

        private void OnMenuTabDisableFinished(MenuTabTag tag)
        {
        }
    }
}