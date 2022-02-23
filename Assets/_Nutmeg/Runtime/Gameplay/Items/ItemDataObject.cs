using UnityEngine;

namespace Gameplay.Items
{
    [CreateAssetMenu(fileName = "new ItemDataObject", menuName = "Item/Generic")]
    public class ItemDataObject : ScriptableObject
    {
        public string name;
        [TextArea]
        public string description;
        public GameObject prefab;
        public Sprite previewImage;
    }
}