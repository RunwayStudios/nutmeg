using System;
using Steamworks.Data;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.MainMenu.CharacterSelection
{
    public class MainMenuCharacterSelectionManager : MonoBehaviour
    {
        [SerializeField] private Transform spawnLocation;
        [SerializeField] private PlayerCharacter.PlayerCharacter[] characters;

        public static Action<PlayerCharacter.PlayerCharacter> onPlayerCharacterSelected;
        public static Action<PlayerCharacter.PlayerCharacter> onNewPlayerCharacterShown;
        public static MainMenuCharacterSelectionManager Main { get; private set; }

        private PlayerCharacter.PlayerCharacter currentPlayerCharacter;
        private GameObject currentPlayerCharacterGameObject;

        public int characterIndex;

        private int CharacterIndex
        {
            set => characterIndex = value > characters.Length - 1 ? 0 : value < 0 ? characters.Length - 1 : value;
            get => characterIndex;
        }

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
                ShowPlayerCharacter(characters[CharacterIndex]);
        }
        
        public PlayerCharacter.PlayerCharacter GetNextPlayerCharacter()
        {
            CharacterIndex++;
            return characters[CharacterIndex];
        }

        public PlayerCharacter.PlayerCharacter GetPreviousPlayerCharacter()
        {
            CharacterIndex--;
            return characters[CharacterIndex];
        }

        public bool TryGetPlayerCharacter(int index, out PlayerCharacter.PlayerCharacter character)
        {
            character = characters[0];
            
            if(index < 0 || index > characters.Length - 1)
                return false;

            character = characters[index];
            return true;
        }

        public void ShowNextPlayerCharacter() => ShowPlayerCharacter(GetNextPlayerCharacter());

        public void ShowPreviousPlayerCharacter() => ShowPlayerCharacter(GetPreviousPlayerCharacter());

        public void ShowPlayerCharacter(PlayerCharacter.PlayerCharacter character)
        {
            if (currentPlayerCharacterGameObject != null)
                Destroy(currentPlayerCharacterGameObject);

            onNewPlayerCharacterShown?.Invoke(character);
            currentPlayerCharacterGameObject = Instantiate(character.prefab, spawnLocation);
            currentPlayerCharacter = character;
        }

        public void SelectCurrentPlayerCharacter() => SelectPlayerCharacter(currentPlayerCharacter);

        public void SelectPlayerCharacter(PlayerCharacter.PlayerCharacter character)
        {
            onPlayerCharacterSelected?.Invoke(character);
        }
    }
}