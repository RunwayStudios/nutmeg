using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.MainMenu.Pedestal
{
    public class MainMenuPedestal : MonoBehaviour
    {
        [SerializeField] private GameObject pedestalPrefab;
        [SerializeField] private float characterPositionOffset;
        private GameObject characterGameObject;

        public PlayerCharacter.PlayerCharacter character;

        public void Initialize(PlayerCharacter.PlayerCharacter character)
        {
            Instantiate(pedestalPrefab, transform);

            if (character != null)
               SpawnPlayerCharacter(character);
        }
        
        public void UpdatePlayerCharacter(PlayerCharacter.PlayerCharacter character)
        {
            if(characterGameObject != null)
                Destroy(characterGameObject);
            
            SpawnPlayerCharacter(character);
        }

        private void SpawnPlayerCharacter(PlayerCharacter.PlayerCharacter character)
        {
            this.character = character;
            characterGameObject = Instantiate(character.prefab, transform);
            characterGameObject.transform.position += Vector3.up * characterPositionOffset;            
        }
    }
}