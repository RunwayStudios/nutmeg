using Nutmeg.Runtime.Gameplay.MainMenu.CharacterSelection;
using Nutmeg.Runtime.Gameplay.PlayerCharacter;
using TMPro;
using UnityEngine;

namespace Nutmeg.Runtime.UI.MainMenu.MenuTabUI
{
    public class MainMenuCharacterSelectionTabUIHandler : MainMenuTabUIHandler
    {
        [SerializeField] private TMP_Text characterName;

        protected override void Start()
        {
            base.Start();
            MainMenuCharacterSelectionManager.onNewPlayerCharacterShown += OnNewPlayerCharacterShown;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            MainMenuCharacterSelectionManager.onNewPlayerCharacterShown -= OnNewPlayerCharacterShown;
        }

        public void NextPlayerCharacter() => MainMenuCharacterSelectionManager.Main.ShowNextPlayerCharacter();

        public void PreviousPlayerCharacter() =>
            MainMenuCharacterSelectionManager.Main.ShowPreviousPlayerCharacter();

        public void SelectPlayerCharacter() => MainMenuCharacterSelectionManager.Main.SelectCurrentPlayerCharacter();

        private void OnNewPlayerCharacterShown(PlayerCharacter character)
        {
            characterName.SetText(character.codename);
        }
    }
}