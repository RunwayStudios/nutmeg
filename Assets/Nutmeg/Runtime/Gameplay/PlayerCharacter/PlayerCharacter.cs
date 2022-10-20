using Unity.Netcode;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.PlayerCharacter
{
    [CreateAssetMenu(fileName = "new PlayerCharacter", menuName = "Player character")]
    public class PlayerCharacter : ScriptableObject
    {
        public string codename;
        [TextArea]
        public string description;

        public GameObject prefab;

        public bool selected;
        
     
    }
}