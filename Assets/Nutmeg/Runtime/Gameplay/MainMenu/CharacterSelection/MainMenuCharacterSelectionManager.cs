using System;
using Nutmeg.Runtime.Gameplay.PlayerCharacter;
using Steamworks.Data;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.MainMenu.CharacterSelection
{
    public class MainMenuCharacterSelectionManager : MonoBehaviour
    {
        [SerializeField] private Transform spawnLocation;

        public static Action<PlayerCharacter.PlayerCharacter> onPlayerCharacterSelected;
        public static Action<PlayerCharacter.PlayerCharacter> onNewPlayerCharacterShown;
        public static MainMenuCharacterSelectionManager Main { get; private set; }

        private GameObject currentPlayerCharacterGameObject;

        

        private void Start()
        {
            Main = this;

            MainMenuManager.onMenuTabEnableStarted += OnMenuTabEnableStarted;
        }

        private void OnDestroy()
        {
            MainMenuManager.onMenuTabEnableStarted += OnMenuTabEnableStarted;
        }

        private void OnMenuTabEnableStarted(MenuTabTag tag)
        {
            if (tag == MenuTabTag.CharacterSelection)
                ShowPlayerCharacter(PlayerCharacterManager.Main.GetPlayerCharacter(PlayerCharacterManager.Main.CharacterIndex));
        }

        public PlayerCharacter.PlayerCharacter GetNextPlayerCharacter()
        {
            PlayerCharacterManager.Main.CharacterIndex++;
            return PlayerCharacterManager.Main.GetPlayerCharacter(PlayerCharacterManager.Main.CharacterIndex);
        }

        public PlayerCharacter.PlayerCharacter GetPreviousPlayerCharacter()
        {
            PlayerCharacterManager.Main.CharacterIndex--;
            return PlayerCharacterManager.Main.GetPlayerCharacter(PlayerCharacterManager.Main.CharacterIndex);
        }

        public void ShowNextPlayerCharacter() => ShowPlayerCharacter(GetNextPlayerCharacter());

        public void ShowPreviousPlayerCharacter() => ShowPlayerCharacter(GetPreviousPlayerCharacter());

        public void ShowPlayerCharacter(PlayerCharacter.PlayerCharacter character)
        {
            if (currentPlayerCharacterGameObject != null)
                Destroy(currentPlayerCharacterGameObject);

            onNewPlayerCharacterShown?.Invoke(character);
            currentPlayerCharacterGameObject = Instantiate(character.prefab, spawnLocation);
            PlayerCharacterManager.Main.SetCurrentPlayerCharacter(character);
        }

        public void SelectCurrentPlayerCharacter() => SelectPlayerCharacter(PlayerCharacterManager.Main.CurrentPlayerCharacter);

        public void SelectPlayerCharacter(PlayerCharacter.PlayerCharacter character)
        {
            onPlayerCharacterSelected?.Invoke(character);
        }
    }
}