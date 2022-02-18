using UnityEngine;

namespace Gameplay.Items
{
    [CreateAssetMenu(fileName = "new ItemInfo", menuName = "Item/Generic")]
    public class ItemData : ScriptableObject
    {
        public string name;
        [TextArea]
        public string description;
        public GameObject prefab;
        public Sprite previewImage;
    }
}