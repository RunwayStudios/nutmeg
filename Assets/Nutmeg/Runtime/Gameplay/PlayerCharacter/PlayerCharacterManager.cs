using System;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.PlayerCharacter
{
    public class PlayerCharacterManager : MonoBehaviour
    {
        [SerializeField] private PlayerCharacter[] characters;
        [SerializeField] private PlayerCharacter defaultPlayerCharacter;

        public PlayerCharacter DefaultPlayerCharacter => defaultPlayerCharacter;

        public PlayerCharacter CurrentPlayerCharacter => internalCurrentPlayerCharacter ?? defaultPlayerCharacter;

        public int PlayerCharactersAmount => characters.Length;

        public static PlayerCharacterManager Main { get; private set; }
        public static Action<PlayerCharacter> onNewCurrentPlayerCharacter;
        
        public int CharacterIndex
        {
            set => characterIndex = value > PlayerCharactersAmount - 1 ? 0 :
                value < 0 ? PlayerCharactersAmount - 1 : value;
            get => characterIndex;
        }
        
        private int characterIndex;

        private PlayerCharacter internalCurrentPlayerCharacter;
        
        private void Awake()
        {
            Main = this;
        }

        public bool TryGetPlayerCharacter(int index, out PlayerCharacter character)
        {
            character = characters[0];

            if (index < 0 || index > characters.Length - 1)
                return false;
            
            character = characters[index];
            return true;
        }

        public PlayerCharacter GetPlayerCharacter(int index)
        {
            characterIndex = index;
            
            return characters[characterIndex];
        }

        public int GetPlayerCharacterIndex(PlayerCharacter character)
        {
            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i] == character)
                    return i;
            }

            Debug.LogError($"The PlayerCharacter {character.codename} does not exist in the characters array");
            return -1;
        }

        public void SetCurrentPlayerCharacter(PlayerCharacter character)
        {
            internalCurrentPlayerCharacter = character;
            onNewCurrentPlayerCharacter?.Invoke(character);
        }

        public void SetCurrentPlayerCharacter(int index)
        {
            SetCurrentPlayerCharacter(
                TryGetPlayerCharacter(index, out var character)
                    ? character
                    : defaultPlayerCharacter
            );
        }
    }
}