using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.MainMenu.Pedestal
{
    public class MainMenuPedestal : MonoBehaviour
    {
        [SerializeField] private GameObject pedestalPrefab;
        [SerializeField] private float characterPositionOffset;
        private GameObject characterGameObject;

        public void Initialize(PlayerCharacter.PlayerCharacter character)
        {
            Instantiate(pedestalPrefab, transform);
            SpawnPlayerCharacter(character);
        }
        
        public void UpdatePlayerCharacter(PlayerCharacter.PlayerCharacter character)
        {
            Destroy(characterGameObject);
            SpawnPlayerCharacter(character);
        }

        private void SpawnPlayerCharacter(PlayerCharacter.PlayerCharacter character)
        {
            characterGameObject = Instantiate(character.prefab, transform);
            characterGameObject.transform.position += Vector3.up * characterPositionOffset;            
        }
    }
}